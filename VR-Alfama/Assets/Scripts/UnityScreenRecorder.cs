#if UNITY_EDITOR

using NetMQ;
using System;
using System.Collections;
using UnityEngine;

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Input;
using NetMQ.Sockets;
using System.Linq;

/// <summary>
/// This script is used to record videos directly from unity.
/// It has impact on framerate so it should only be used when no headset is being used.
/// </summary>
public class UnityScreenRecorder : DataPublisher
{
    /// <summary>
    /// Base Path where the videos should be recorded 
    /// </summary>
    [Tooltip("Only used if SubscriberAddress is kept blank")]
    public string BasePath;
    /// <summary>
    /// VideoFilename to be created 
    /// </summary>
    [Tooltip("Only used if SubscriberAddress is kept blank")]
    public string Filename;
    private bool recording;
    private RecorderController recorderController;

    /// <summary>
    /// The address of the socket being published by bonsai that contains File Info to record the movie
    /// Leave it null if you want to record without bonsai telling it to...
    /// </summary>
    [Tooltip("The address of the socket being published by bonsai that contains File Info to record the movie. Leave it null if you want to record without bonsai telling it to...")]
    public string SubscriberAddress;
    private SubscriberSocket SubSocket;

    [Tooltip("Only used if SubscriberAddress is kept blank")]
    public KeyCode StartRecordingKey = KeyCode.Space;
    [Tooltip("Only used if SubscriberAddress is kept blank")]
    public KeyCode StopRecordingKey = KeyCode.LeftShift;

    private bool useSubSocket = false;

    protected override void Start()
    {
        base.Start();
        recording = false;
        if (SubscriberAddress != null && SubscriberAddress.Length > 0)
        {
            SubSocket = new SubscriberSocket();
            SubSocket.Connect(SubscriberAddress);
            SubSocket.Subscribe("Path");
            useSubSocket = true;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (useSubSocket && SubSocket.HasIn)
        {
            var topic = SubSocket.ReceiveFrameString();
            var message = SubSocket.ReceiveFrameString();
            Debug.Log("Received Topic:" + topic + " Msg:" + message);
            BasePath = message;
        }
        if (!recording && Input.GetKeyDown(StartRecordingKey))
        {
            var date = DateTime.Now.ToString("yyyy_MM_dd__HH_mm_ss");
            StartRecording(BasePath + this.Filename + "_" + date);
            SendRecordingStatus("StartRecording");
            recording = true;
        }
        if (recording && Input.GetKeyDown(StopRecordingKey))
        {
            recorderController.StopRecording();
            SendRecordingStatus("StopRecording");
            recording = false;
        }
    }

    private void StartRecording(string path)
    {
        var controllerSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
        recorderController = new RecorderController(controllerSettings);

        var recorderSettings = ScriptableObject.CreateInstance<MovieRecorderSettings>();
        recorderSettings.name = "Video Recorder";
        recorderSettings.Enabled = true;

        recorderSettings.OutputFormat = MovieRecorderSettings.VideoRecorderOutputFormat.MP4;
        recorderSettings.VideoBitRateMode = VideoBitrateMode.High;

        recorderSettings.ImageInputSettings = new GameViewInputSettings
        {
            OutputWidth = 1280,
            OutputHeight = 720
        };

        recorderSettings.AudioInputSettings.PreserveAudio = false;

        recorderSettings.OutputFile = path;

        // Setup Recording
        controllerSettings.AddRecorderSettings(recorderSettings);
        controllerSettings.SetRecordModeToManual();
        controllerSettings.FrameRate = 30.0f;

        RecorderOptions.VerboseMode = false;
        recorderController.PrepareRecording();
        recorderController.StartRecording();
    }
    private void SendRecordingStatus(string status)
    {
        long timestamp = DateTime.Now.Ticks / (TimeSpan.TicksPerMillisecond / 1000);

       PubSocket.SendMoreFrame("RecordingStatus")
            .SendMoreFrame(BitConverter.GetBytes(timestamp))
            .SendFrame(status);
    }

    protected override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
        if (SubSocket != null)
        {
            SubSocket.Dispose();
        }
        NetMQConfig.Cleanup(false);
    }
    void OnDisable()
    {
        if(recording)
            recorderController.StopRecording();
    }
}
#endif