using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using NetMQ;
using System.Runtime.CompilerServices;
using Bonsai.Reactive;
using OpenCV.Net;

[Combinator]
[Description("Convert Glia Eye ZeroMQ messages into bonsai compliant data structures")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class ToOmniceptEyeTracking
{
    public IObservable<OmniceptEyeTracking> Process(IObservable<NetMQMessage> source)
    {
        return source.Select(value => new OmniceptEyeTracking 
        {
            Timestamp = new OmniceptGliaTimestamp(value[1]),
            // HardwareTimeMicroseconds = BitConverter.ToInt64(value[1].Buffer, 0),
            // OmniceptTimeMicroseconds = BitConverter.ToInt64(value[2].Buffer, 0),
            // SystemTimeMicroseconds = BitConverter.ToInt64(value[3].Buffer, 0),
            Position = new Scalar( BitConverter.ToSingle(value[2].Buffer,0), 
                 BitConverter.ToSingle(value[2].Buffer, sizeof(float)),
                 BitConverter.ToSingle(value[2].Buffer, sizeof(float)*2)), 
            LeftEye = new Eye(value[2].Buffer, sizeof(float)*3),
            RightEye = new Eye(value[2].Buffer, sizeof(float)*9)

        });
    }

    public class OmniceptEyeTracking
    {
        public OmniceptGliaTimestamp Timestamp;
        public Scalar Position;
        public Eye LeftEye;
        public Eye RightEye;
    }
    public class Eye
    {
        public float Openness;
        public float OpennessConfidence;
        public float PupilDilation;
        public float PupilDilationConfidence;
        public Point2f PupilPosition;

        public Eye(byte[] buffer, int startIndex)
        {
            Openness = BitConverter.ToSingle(buffer,startIndex);
            OpennessConfidence  = BitConverter.ToSingle(buffer,startIndex + sizeof(float));
            PupilDilation = BitConverter.ToSingle(buffer,startIndex + sizeof(float) * 2);
            PupilDilationConfidence = BitConverter.ToSingle(buffer,startIndex + sizeof(float) * 3);
            PupilPosition = new  Point2f(
                BitConverter.ToSingle(buffer,startIndex + sizeof(float) * 4),
                BitConverter.ToSingle(buffer,startIndex + sizeof(float) * 5)
            );
        }

    }

}


