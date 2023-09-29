using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using NetMQ;

[Combinator]
[Description("")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class ToUnityVector3
{
    public IObservable<UnityVector3> Process(IObservable<NetMQMessage> source)
    {
        return source.Select(value => new UnityVector3 {
            SystemTimeMicroseconds = BitConverter.ToInt64(value[1].Buffer, 0),
            X = BitConverter.ToSingle(value[2].Buffer, 0),
            Y = BitConverter.ToSingle(value[3].Buffer, 0),
            Z = BitConverter.ToSingle(value[4].Buffer, 0)
        });
    }

    public class UnityVector3 {
        public long SystemTimeMicroseconds;
        public float X;
        public float Y;
        public float Z;
    }
}
