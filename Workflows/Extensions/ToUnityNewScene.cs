using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using NetMQ;
using OpenCV.Net;

[Combinator]
[Description("Get SpawnID, SceneType and SecondsDuration. From ZeroMQ with NewScene info")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class ToUnityNewScene
{
    public IObservable<UnityNewScene> Process(IObservable<NetMQMessage> source)
    {
        return source.Select(value => new UnityNewScene
        {
            SystemTimeMicroseconds = BitConverter.ToInt64(value[1].Buffer, 0),
            SceneType = BitConverter.ToInt32(value[2].Buffer, 0),
            SpawnID = BitConverter.ToInt32(value[2].Buffer, sizeof(int)),
            SecondsDuration = BitConverter.ToInt32(value[2].Buffer, sizeof(int) * 2)
        });
    }
}
public class UnityNewScene
{
    public long SystemTimeMicroseconds;
    public int SpawnID;
    public int SceneType;
    public int SecondsDuration;
}