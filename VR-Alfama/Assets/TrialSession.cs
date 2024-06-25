using NetMQ;
using Newtonsoft.Json.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.XR;

public class TrialSession : DataPublisher
{
    [System.Serializable]
    public class Trial
    {
        public Vector3 InitialPosition;
        public Vector3 InitialRotation;
        public SceneType SceneType;
        public int SecondsDuration;
        public Texture2D Map;
    }
    
    public enum SceneType { Adverse, Optimistic }

    public float SecondsInterTrialInterval = 3;
    public float SecondsPrimeMap = 3;

    public VrInteractionController InteractionSource;
    public UiManager UiManager;
    public Camera MapCamera;

    public Trial[] TrialList;
    private int CurrentTrialIndex = 0;

    private delegate void UpdateAction();
    private UpdateAction updateAction = () => { };

    private float Timer = 0f;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        //updateAction = InitialState;

        StartCoroutine(Session());
    }

    // Update is called once per frame
    void Update()
    {
        //updateAction();
    }

    IEnumerator Session()
    {
        // Initial state
        UiManager.OpenMessagePanel("AlfamaVR", "Adjust the headset and pick up the controllers. Press the right trigger to continue.");
        UiManager.CloseImagePanel();

        foreach (Trial currentTrial in TrialList)
        {
            // Start state
            while (!InteractionSource.RightInteractionState) { yield return null; }

            // Intertrial interval
            UiManager.OpenMessagePanel("AlfamaVR", "Prepare to explore the space.");
            yield return new WaitForSeconds(SecondsInterTrialInterval);
            LogInterTrialInterval();

            // Prime map
            UiManager.CloseMessagePanel();
            InteractionSource.transform.position = currentTrial.InitialPosition;
            InteractionSource.transform.rotation = Quaternion.Euler(currentTrial.InitialRotation);
            // TODO adverse vs. optimistic

            Texture2D cameraTexture = VrUtilities.TextureFromCamera(MapCamera);
            UiManager.OpenImagePanel("AlfamaVr", cameraTexture, "Note your starting location on the map (red).");
            yield return new WaitForSeconds(SecondsPrimeMap);

            // Spatial sampling
            UiManager.CloseImagePanel();
            LogNewScene();
            yield return new WaitForSeconds(currentTrial.SecondsDuration);

            CurrentTrialIndex++;
        }

        yield return null;
    }

    void InitialState()
    {
        // Open initial menu
        UiManager.OpenMessagePanel("AlfamaVR", "Adjust the headset and pick up the controllers. Press the right trigger to continue." );
        UiManager.CloseImagePanel();

        updateAction = StartState;
    }

    void StartState()
    {
        if (InteractionSource.RightInteractionState)
        {
            Timer = SecondsInterTrialInterval;
            UiManager.CloseMessagePanel();
            updateAction = InterTrialIntervalState;
        }
    }

    void InterTrialIntervalState()
    {
        UiManager.OpenMessagePanel("AlfamaVR", String.Format("Prepare to explore the space in" + System.Environment.NewLine + "{0}", Math.Ceiling(Timer)));

        Timer -= Time.deltaTime;

        if (Timer <= 0)
        {
            LogInterTrialInterval();

            UiManager.CloseMessagePanel();

            // Spawn player at spawn point
            InteractionSource.transform.position = TrialList[CurrentTrialIndex].InitialPosition;
            InteractionSource.transform.rotation = Quaternion.Euler(TrialList[CurrentTrialIndex].InitialRotation);

            Texture2D cameraTexture = VrUtilities.TextureFromCamera(MapCamera);
            UiManager.OpenImagePanel("AlfamaVr", cameraTexture, "Note your starting location on the map (red).");
            Timer = SecondsPrimeMap;
            updateAction = PrimeMapState;
        }
    }

    void PrimeMapState()
    {
        Timer -= Time.deltaTime;

        if (Timer <= 0 )
        {
            UiManager.CloseImagePanel();
            Timer = TrialList[CurrentTrialIndex].SecondsDuration;
            updateAction = SpatialSampling;
        }
    }

    void SpatialSampling()
    {
        Timer -= Time.deltaTime;

        if (Timer <= 0)
        {
            CurrentTrialIndex++;
            updateAction = InterTrialIntervalState;
        }
    }

    private void LogInterTrialInterval()
    {
        long timestamp = DateTime.Now.Ticks / (TimeSpan.TicksPerMillisecond / 1000);
        PubSocket.SendMoreFrame("ITI")
           .SendMoreFrame(BitConverter.GetBytes(timestamp))
           .SendFrame(BitConverter.GetBytes(SecondsInterTrialInterval));
    }

    private void LogNewScene()
    {
        long timestamp = DateTime.Now.Ticks / (TimeSpan.TicksPerMillisecond / 1000);
        byte[] scene = Encoding.ASCII.GetBytes(CurrentTrialIndex.ToString() + '\0');
        byte[] spatialSample = Encoding.ASCII.GetBytes(CurrentTrialIndex.ToString() + '\0');

        PubSocket.SendMoreFrame("NewScene")
            .SendMoreFrame(BitConverter.GetBytes(timestamp))
            .SendMoreFrame(scene)
            .SendMoreFrame(spatialSample)
            .SendFrame(BitConverter.GetBytes(TrialList[CurrentTrialIndex].SecondsDuration));
    }
}
