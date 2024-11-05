using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using NetMQ;
using OpenCV.Net;

[Combinator]
[Description("Get Session StartStop. From ZeroMQ Unity Session Message")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class ToUnitySession
{
    public IObservable<UnitySession> Process(IObservable<NetMQMessage> source)
    {
        return source.Select(value => new UnitySession
        {
            SystemTimeMicroseconds = BitConverter.ToInt64(value[1].Buffer, 0),
            StartStop = BitConverter.ToInt32(value[2].Buffer, 0) == 0,
        });
    }
}
public class UnitySession
{
    public long SystemTimeMicroseconds;
    public bool StartStop;

}