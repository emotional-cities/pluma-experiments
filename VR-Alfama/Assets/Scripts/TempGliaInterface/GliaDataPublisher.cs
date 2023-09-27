using HP.Omnicept.Messaging.Messages;
using NetMQ;
using NetMQ.Sockets;
using System;
using UnityEngine;

public class GliaDataPublisher : MonoBehaviour
{
    public string PublisherBindAddress;
    private PublisherSocket PubSocket;

    private void Awake()
    {
        AsyncIO.ForceDotNet.Force();
    }

    private void Start()
    {
        PubSocket = new PublisherSocket();
        PubSocket.Bind(PublisherBindAddress);
    }

    public void HandleHeartRate(HeartRate hr)
    {
        if (hr != null)
        {
            PubSocket.SendMoreFrame("HeartRate")
                .SendMoreFrame(BitConverter.GetBytes(hr.Timestamp.HardwareTimeMicroSeconds))
                .SendMoreFrame(BitConverter.GetBytes(hr.Timestamp.OmniceptTimeMicroSeconds))
                .SendMoreFrame(BitConverter.GetBytes(hr.Timestamp.SystemTimeMicroSeconds))
                .SendFrame(BitConverter.GetBytes(hr.Rate));
        }
    }

    public void HandleMouthCameraImage(CameraImage camImage)
    {
        if (camImage != null)
        {
            if (camImage.SensorInfo.Location == "Mouth")
            {
                byte[] imageData = camImage.ImageData;
                var width = camImage.Width;
                var height = camImage.Height;
                var channels = 1;
                var hardwareTime = camImage.Timestamp.HardwareTimeMicroSeconds;
                var omniceptTime = camImage.Timestamp.OmniceptTimeMicroSeconds;
                var systemTime = camImage.Timestamp.SystemTimeMicroSeconds;

                PubSocket.SendMoreFrame("Mouth")
                    .SendMoreFrame(BitConverter.GetBytes(hardwareTime))
                    .SendMoreFrame(BitConverter.GetBytes(omniceptTime))
                    .SendMoreFrame(BitConverter.GetBytes(systemTime))
                    .SendMoreFrame(imageData)
                    .SendMoreFrame(BitConverter.GetBytes(width))
                    .SendMoreFrame(BitConverter.GetBytes(height))
                    .SendFrame(BitConverter.GetBytes(channels));
            }
        }
    }

    public void HandleEyeTracking(EyeTracking et)
    {
        if (et != null)
        {
            PubSocket.SendMoreFrame("EyeTracking")
                .SendMoreFrame(BitConverter.GetBytes(et.Timestamp.HardwareTimeMicroSeconds))
                .SendMoreFrame(BitConverter.GetBytes(et.Timestamp.OmniceptTimeMicroSeconds))
                .SendMoreFrame(BitConverter.GetBytes(et.Timestamp.SystemTimeMicroSeconds))
                .SendMoreFrame(BitConverter.GetBytes(et.CombinedGaze.X))
                .SendMoreFrame(BitConverter.GetBytes(et.CombinedGaze.Y))
                .SendFrame(BitConverter.GetBytes(et.CombinedGaze.Z));
        }
    }

    public void HandleIMU(IMUFrame imu)
    {
        if (imu != null)
        {
            PubSocket.SendMoreFrame("IMU")
                .SendMoreFrame(BitConverter.GetBytes(imu.Timestamp.HardwareTimeMicroSeconds))
                .SendMoreFrame(BitConverter.GetBytes(imu.Timestamp.OmniceptTimeMicroSeconds))
                .SendMoreFrame(BitConverter.GetBytes(imu.Timestamp.SystemTimeMicroSeconds))
                .SendMoreFrame(BitConverter.GetBytes(imu.Data[0].Acc.X))
                .SendMoreFrame(BitConverter.GetBytes(imu.Data[0].Acc.Y))
                .SendFrame(BitConverter.GetBytes(imu.Data[0].Acc.Z));
        }
    }

    private void OnApplicationQuit()
    {
        if (PubSocket != null)
        {
            PubSocket.Dispose();
        }
        NetMQConfig.Cleanup(false);
    }
}