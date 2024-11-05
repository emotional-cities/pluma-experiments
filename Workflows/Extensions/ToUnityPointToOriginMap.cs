using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using NetMQ;
using OpenCV.Net;
/*
private void LogPointToMap(int state, Vector3 playerPositionGuess)
{
    Vector3 subjectPositionMap = InteractionSource.transform.position;
    var originPosition = TrialList[CurrentTrialIndex].InitialPosition;

    long timestamp = DateTime.Now.Ticks / (TimeSpan.TicksPerMillisecond / 1000);
    byte[] originPositionData = BitConverter.GetBytes(originPosition.x)
        .Concat(BitConverter.GetBytes(originPosition.y))
        .Concat(BitConverter.GetBytes(originPosition.z))
        .ToArray();
    byte[] subjectPositionData = BitConverter.GetBytes(subjectPositionMap.x)
        .Concat(BitConverter.GetBytes(subjectPositionMap.y))
        .Concat(BitConverter.GetBytes(subjectPositionMap.z))
        .ToArray();
    byte[] pointPositionData = BitConverter.GetBytes(playerPositionGuess.x)
        .Concat(BitConverter.GetBytes(playerPositionGuess.y))
        .Concat(BitConverter.GetBytes(playerPositionGuess.z))
        .ToArray();
    byte[] allData = originPositionData.Concat(subjectPositionData).Concat(pointPositionData).ToArray();
    PubSocket.SendMoreFrame("PointToOriginMap")
       .SendMoreFrame(BitConverter.GetBytes(timestamp))
       .SendMoreFrame(allData)
       .SendFrame(BitConverter.GetBytes(state));
}*/
[Combinator]
[Description("Get PointToOriginMap data, From ZeroMQ Unity with PointToOriginMap info.")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class ToUnityPointToOriginMap
{
    public IObservable<UnityPointToOriginMap> Process(IObservable<NetMQMessage> source)
    {
        return source.Select(value => new UnityPointToOriginMap
        {
            SystemTimeMicroseconds = BitConverter.ToInt64(value[1].Buffer, 0),
            OriginPositionData = new Scalar(BitConverter.ToSingle(value[2].Buffer,0), 
                 BitConverter.ToSingle(value[2].Buffer, sizeof(float)),
                 BitConverter.ToSingle(value[2].Buffer, sizeof(float)*2)),
            SubjectPositionData = new Scalar(BitConverter.ToSingle(value[2].Buffer,sizeof(float)*3), 
                 BitConverter.ToSingle(value[2].Buffer, sizeof(float)*4),
                 BitConverter.ToSingle(value[2].Buffer, sizeof(float)*5)),
            PointPositionData = new Scalar(BitConverter.ToSingle(value[2].Buffer,sizeof(float)*6), 
                 BitConverter.ToSingle(value[2].Buffer, sizeof(float)*7),
                 BitConverter.ToSingle(value[2].Buffer, sizeof(float)*8)),
            StartStop = BitConverter.ToInt32(value[3].Buffer, 0)
        });
    }
}
public class UnityPointToOriginMap
{
    public long SystemTimeMicroseconds;
    public Scalar OriginPositionData;
    public Scalar SubjectPositionData;

    public Scalar PointPositionData;

    public int StartStop;
    
}