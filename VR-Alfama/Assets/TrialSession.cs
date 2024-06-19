using NetMQ;
using Newtonsoft.Json.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class TrialSession : DataPublisher
{
    [System.Serializable]
    public class Trial
    {
        public int SecondsDuration;
        public Texture2D Map;
    }

    public float SecondsInterTrialInterval = 3;

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
        updateAction = InitialState;   
    }

    // Update is called once per frame
    void Update()
    {
        updateAction();
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

            // TODO spawn player at spawn point

            Texture2D cameraTexture = VrUtilities.TextureFromCamera(MapCamera);
            UiManager.OpenImagePanel("AlfamaVr", cameraTexture);
            updateAction = PrimeMapState;
        }
    }

    void PrimeMapState()
    {

    }

    private void LogInterTrialInterval()
    {
        long timestamp = DateTime.Now.Ticks / (TimeSpan.TicksPerMillisecond / 1000);
        PubSocket.SendMoreFrame("ITI")
           .SendMoreFrame(BitConverter.GetBytes(timestamp))
           .SendFrame(BitConverter.GetBytes(SecondsInterTrialInterval));
    }
}
