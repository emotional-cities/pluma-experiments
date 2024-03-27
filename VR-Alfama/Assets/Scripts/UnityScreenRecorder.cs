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

/// <summary>
/// This script is used to record videos directly from unity.
/// It has impact on framerate so it should only be used when no headset is being used.
/// </summary>
public class UnityScreenRecorder : MonoBehaviour
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
    private bool recording = false;
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

    private void Start()
    {
        if (SubscriberAddress != null)
        {
            SubSocket = new SubscriberSocket();
            SubSocket.Connect(SubscriberAddress);
            SubSocket.Subscribe("Path");
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (SubscriberAddress != null)
        {
            if (!recording && Input.GetKeyDown(StartRecordingKey))
            {
                var date = DateTime.Now.ToString("yyyy_MM_dd__HH_mm_ss");
                StartRecording(BasePath + "\\" + this.Filename + "_" + date);
            }
            if (recording && Input.GetKeyDown(StopRecordingKey))
            {
                StopRecording();
            }
        }
        else if (SubSocket.HasIn)
        {
            var topic = SubSocket.ReceiveFrameString();
            var message = SubSocket.ReceiveFrameString();
            Debug.Log("Received Topic:" + topic + " Msg:" + message);
            StartRecording(message);
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

        recording = true;
    }
    void StopRecording()
    {
        if (recording)
        {
            recorderController.StopRecording();
            recording = false;
        }
    }

    protected virtual void OnApplicationQuit()
    {
        if (SubSocket != null)
        {
            SubSocket.Dispose();
        }
        NetMQConfig.Cleanup(false);
    }
    void OnDisable()
    {
        StopRecording();
    }
}
