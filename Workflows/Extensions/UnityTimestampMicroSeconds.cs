using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using NetMQ;
using OpenCV.Net;

[Combinator]
[Description("Get Timestamp from unity ZeroMQ Messages")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class UnityTimestampMicroSeconds
{
    public IObservable<long> Process(IObservable<NetMQMessage> source)
    {
        return source.Select(value => 
            BitConverter.ToInt64(value[1].Buffer, 0));
    }
}
