using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using NetMQ;
using OpenCV.Net;

[Combinator]
[Description("Get SecondsInterTrialInterval. From ZeroMQ Unity ITI Message")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class ToUnityItI
{
    public IObservable<UnityIti> Process(IObservable<NetMQMessage> source)
    {
        return source.Select(value => new UnityIti
        {
            SystemTimeMicroseconds = BitConverter.ToInt64(value[1].Buffer, 0),
            SecondsInterTrialInterval = BitConverter.ToSingle(value[2].Buffer, 0),
        });
    }
}
public class UnityIti
{
    public long SystemTimeMicroseconds;
    public float SecondsInterTrialInterval;

}