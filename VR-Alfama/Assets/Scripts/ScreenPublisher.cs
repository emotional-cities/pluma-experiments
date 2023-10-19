using NetMQ;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class ScreenPublisher : DataPublisher
{
    protected override void Start()
    {
        base.Start();

        StartCoroutine(PublishScreenGrab());
    }

    IEnumerator PublishScreenGrab()
    {
        yield return new WaitForSeconds(2);

        while (true)
        {
            yield return new WaitForEndOfFrame();
            long timestamp = DateTime.Now.Ticks / (TimeSpan.TicksPerMillisecond / 1000);
           

            // Do publishing
            

            //byte[] imagetoSend = ScreenCapture.CaptureScreenshotAsTexture().EncodeToJPG();
            //
            Texture2D screenShot = new Texture2D(Screen.width, Screen.height);
            screenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            screenShot.Apply();
            byte[] bytes = screenShot.EncodeToJPG();
            UnityEngine.Object.Destroy(screenShot);
            PubSocket.SendMoreFrame("ScreenShot")
                .SendMoreFrame(BitConverter.GetBytes(timestamp))
                .SendFrame(bytes);
            yield return new WaitForSecondsRealtime(0.05f);
            //for (int i = 0; i < 15; i++)
            //    yield return null;
            yield return null;
        }


    }
}
