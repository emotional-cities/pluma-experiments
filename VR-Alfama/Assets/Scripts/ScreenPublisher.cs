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

            PubSocket.SendMoreFrame("ScreenShot")
                .SendFrame(BitConverter.GetBytes(timestamp));
        }


    }
}
