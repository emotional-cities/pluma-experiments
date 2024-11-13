using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using NetMQ;
using OpenCV.Net;

[Combinator]
[Description("Get PointToOriginWorld data, From ZeroMQ Unity with PointToOriginWorld info.")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class ToUnityPointToOriginWorld
{
    public IObservable<UnityPointToOriginWorld> Process(IObservable<NetMQMessage> source)
    {
        return source.Select(value => new UnityPointToOriginWorld
        {
            SystemTimeMicroseconds = BitConverter.ToInt64(value[1].Buffer, 0),
            OriginPositionData = new Scalar(BitConverter.ToSingle(value[2].Buffer,0), 
                 BitConverter.ToSingle(value[2].Buffer, sizeof(float)),
                 BitConverter.ToSingle(value[2].Buffer, sizeof(float)*2)),
            HandPositionData = new Scalar(BitConverter.ToSingle(value[2].Buffer,sizeof(float)*3), 
                 BitConverter.ToSingle(value[2].Buffer, sizeof(float)*4),
                 BitConverter.ToSingle(value[2].Buffer, sizeof(float)*5)),
            HandAxisAngleData = new Scalar(BitConverter.ToSingle(value[2].Buffer,sizeof(float)*6), 
                 BitConverter.ToSingle(value[2].Buffer, sizeof(float)*7),
                 BitConverter.ToSingle(value[2].Buffer, sizeof(float)*8)),
            originAxisAngleData = new Scalar(BitConverter.ToSingle(value[2].Buffer,sizeof(float)*9), 
                 BitConverter.ToSingle(value[2].Buffer, sizeof(float)*10),
                 BitConverter.ToSingle(value[2].Buffer, sizeof(float)*11)),
            StartStop = BitConverter.ToInt32(value[3].Buffer, 0)
        });
    }
}
public class UnityPointToOriginWorld
{
    public long SystemTimeMicroseconds;
    public Scalar OriginPositionData;
    public Scalar HandPositionData;

    public Scalar HandAxisAngleData;

    public Scalar originAxisAngleData;

    public int StartStop;
    
}