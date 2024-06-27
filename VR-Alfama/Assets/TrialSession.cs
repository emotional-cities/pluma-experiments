using NetMQ;
using Newtonsoft.Json.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        StartCoroutine(Session());
    }

    IEnumerator Session()
    {
        // Initial state
        UiManager.OpenMessagePanel("AlfamaVR", "Adjust the headset and pick up the controllers. Press the right trigger to continue.");
        UiManager.CloseImagePanel();

        foreach (Trial currentTrial in TrialList)
        {
            // Start state
            InteractionSource.SetPointerActive(false);
            Debug.Log(InteractionSource.RightInteractionState);
            while (!InteractionSource.RightInteractionState) { yield return null; }

            //// Pointer debug
            //Texture2D cameraTexture = VrUtilities.TextureFromCamera(MapCamera);
            //UiManager.OpenImagePanel("AlfamaVr", cameraTexture, "DEBUG");
            //yield return new WaitForSeconds(0.5f);
            //InteractionSource.SetPointerActive(true);

            //while (!InteractionSource.RightInteractionState) {
            //    RaycastHit hit = InteractionSource.GetPointedObject(LayerMask.GetMask("UI"));
            //    if (hit.transform != null)
            //    {
            //        Vector3 localHit = hit.collider.GetComponent<RectTransform>().InverseTransformPoint(hit.point);

            //        //UiManager.OpenImagePanel("AlfamaVr", cameraTexture, localHit.x.ToString());
            //        //UiManager.OpenImagePanel("AlfamaVr", cameraTexture, (UiManager.ImagePanel.Image.rectTransform.rect.height * localHit.y).ToString());
            //        UiManager.OpenImagePanel("AlfamaVr", cameraTexture, (localHit.x).ToString());

            //        UiManager.SetImageCursorPosition(localHit);
            //    }

            //    //yield return null; 
            //    yield return null;
            //}

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

            // Point to origin
            UiManager.OpenMessagePanel("AlfamaVr", "Point to your starting position and press the right trigger.");
            InteractionSource.SetPointerActive(true);

            LogPointToOriginWorld(0);
            while (!InteractionSource.RightInteractionState) {
                LogPointToOriginWorld(1);
                yield return null; 
            }
            LogPointToOriginWorld(2);

            // Point on map
            InteractionSource.SetPointerActive(false);
            UiManager.OpenImagePanel("AlfamaVr", cameraTexture, "Point to your current location and press the right trigger.");
            yield return new WaitForSeconds(0.5f);
            InteractionSource.SetPointerActive(true);

            while (!InteractionSource.RightInteractionState)
            {
                RaycastHit hit = InteractionSource.GetPointedObject(LayerMask.GetMask("UI"));
                if (hit.transform != null)
                {
                    Vector3 localHit = hit.collider.GetComponent<RectTransform>().InverseTransformPoint(hit.point);

                    UiManager.SetImageCursorPosition(localHit);
                }
                yield return null;
            }

            CurrentTrialIndex++;
        }

        yield return null;
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

    private void LogPointToOriginWorld(int state)
    {
        var originPosition = TrialList[CurrentTrialIndex].InitialPosition;
        var handTransform = InteractionSource.GetRightControllerTransform();
        var handPosition = handTransform.position;
        var handRotation = handTransform.rotation;
        var handAxisAngle = -handTransform.up;

        var originAxisAngle = (originPosition - handTransform.position).normalized;

        long timestamp = DateTime.Now.Ticks / (TimeSpan.TicksPerMillisecond / 1000);
        byte[] originPositionData = BitConverter.GetBytes(originPosition.x)
            .Concat(BitConverter.GetBytes(originPosition.y))
            .Concat(BitConverter.GetBytes(originPosition.z))
            .ToArray();
        byte[] handPositionData = BitConverter.GetBytes(handPosition.x)
            .Concat(BitConverter.GetBytes(handPosition.y))
            .Concat(BitConverter.GetBytes(handPosition.z))
            .ToArray();
        byte[] handAxisAngleData = BitConverter.GetBytes(handAxisAngle.x)
            .Concat(BitConverter.GetBytes(handAxisAngle.y))
            .Concat(BitConverter.GetBytes(handAxisAngle.z))
            .ToArray();
        byte[] originAxisAngleData = BitConverter.GetBytes(originAxisAngle.x)
            .Concat(BitConverter.GetBytes(originAxisAngle.y))
            .Concat(BitConverter.GetBytes(originAxisAngle.z))
            .ToArray();
        byte[] allData = originPositionData.Concat(handPositionData).Concat(handAxisAngleData).Concat(originAxisAngleData).ToArray();
        PubSocket.SendMoreFrame("PointToOriginWorld")
           .SendMoreFrame(BitConverter.GetBytes(timestamp))
           .SendMoreFrame(allData)
           .SendFrame(BitConverter.GetBytes(state)); ;

    }
}
