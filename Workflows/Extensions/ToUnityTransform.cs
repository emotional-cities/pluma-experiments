using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using NetMQ;
using OpenCV.Net;

[Combinator]
[Description("Get Transform Vector From ZeroMQ VR with position and forward vector")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class ToUnityTransform
{
    public IObservable<UnityTransform> Process(IObservable<NetMQMessage> source)
    {
        return source.Select(value => new UnityTransform
        {
            SystemTimeMicroseconds = BitConverter.ToInt64(value[1].Buffer, 0),
            Position = new Scalar(BitConverter.ToSingle(value[2].Buffer, 0),
                 BitConverter.ToSingle(value[2].Buffer, sizeof(float)),
                 BitConverter.ToSingle(value[2].Buffer, sizeof(float) * 2)),
            ForwardVector = new Scalar(BitConverter.ToSingle(value[2].Buffer, sizeof(float) * 3),
                 BitConverter.ToSingle(value[2].Buffer, sizeof(float) * 4),
                 BitConverter.ToSingle(value[2].Buffer, sizeof(float) * 5))
        });
    }
}
public class UnityTransform
{
    public long SystemTimeMicroseconds;
    public Scalar Position;
    public Scalar ForwardVector;
}