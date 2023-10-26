#if UNITY_EDITOR

using System.ComponentModel;
using System.IO;
using NetMQ;
using NetMQ.Sockets;
using UnityEditor;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Input;

namespace UnityEngine.Recorder.Examples
{
    /// <summary>
    /// This example shows how to set up a recording session via script, for an MP4 file.
    /// To use this example, add the MovieRecorderExample component to a GameObject.
    ///
    /// Enter the Play Mode to start the recording.
    /// The recording automatically stops when you exit the Play Mode or when you disable the component.
    ///
    /// This script saves the recording outputs in [Project Folder]/SampleRecordings.
    /// </summary>
    public class MovieRecorder : MonoBehaviour
    {

        public string SubscriberAddress;
        public string DataPath;
        public bool m_RecordAudio = true;
        
        internal MovieRecorderSettings m_Settings = null;

        private SubscriberSocket SubSocket;
        private bool recording = false;
        private RecorderController m_RecorderController;
        public FileInfo OutputFile
        {
            get
            {
                var fileName = m_Settings.OutputFile + ".mp4";
                return new FileInfo(fileName);
            }
        }
        private void Start()
        {
            SubSocket = new SubscriberSocket();
            SubSocket.Connect(SubscriberAddress);
            SubSocket.Subscribe("Path");
        }
        private void Update()
        {
           // SubSocket.Poll();
           if(!recording)
           {
                if (DataPath != "" )
                    RecordVideo(DataPath);
                else if (SubSocket.HasIn)
                {
                    var topic = SubSocket.ReceiveFrameString();
                    var message = SubSocket.ReceiveFrameString();
                    Debug.Log("Received Topic:" + topic + " Msg:" + message);
                    RecordVideo(message);
                }
            }
        }
        private void RecordVideo(string path)
        {
            var controllerSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
            m_RecorderController = new RecorderController(controllerSettings);

            //  var mediaOutputFolder = new DirectoryInfo(Path.Combine(Application.dataPath, "..", "SampleRecordings"));

            // Video
            m_Settings = ScriptableObject.CreateInstance<MovieRecorderSettings>();
            m_Settings.name = "My Video Recorder";
            m_Settings.Enabled = true;

            // This example performs an MP4 recording
            m_Settings.OutputFormat = MovieRecorderSettings.VideoRecorderOutputFormat.MP4;
            m_Settings.VideoBitRateMode = VideoBitrateMode.High;

            m_Settings.ImageInputSettings = new GameViewInputSettings
            {
                OutputWidth = 1920,
                OutputHeight = 1080
            };

            m_Settings.AudioInputSettings.PreserveAudio = m_RecordAudio;

            m_Settings.OutputFile = path + "\\" + "video";

            // Setup Recording
            controllerSettings.AddRecorderSettings(m_Settings);
            controllerSettings.SetRecordModeToManual();
            controllerSettings.FrameRate = 30.0f;

            RecorderOptions.VerboseMode = false;
            m_RecorderController.PrepareRecording();
            m_RecorderController.StartRecording();

            Debug.Log($"Started recording for file {OutputFile.FullName}");
            recording = true;
        }
        void OnDisable()
        {
            m_RecorderController.StopRecording();
        }
        protected virtual void OnApplicationQuit()
        {
            if (SubSocket != null)
            {
                SubSocket.Dispose();
            }
            NetMQConfig.Cleanup(false);
        }
    }
}

#endif
