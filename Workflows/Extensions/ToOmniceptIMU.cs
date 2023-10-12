using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using NetMQ;
using OpenCV.Net;

[Combinator]
[Description("Convert Glia Eye ZeroMQ messages into bonsai compliant data structures")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class ToOmniceptIMU
{
    public IObservable<OmniceptIMU> Process(IObservable<NetMQMessage> source)
    {
        return source.Select(value => new OmniceptIMU {
            Timestamp = new OmniceptGliaTimestamp(value[1]),
            
            Acceleration = new Scalar(BitConverter.ToSingle(value[2].Buffer,0), 
                 BitConverter.ToSingle(value[2].Buffer, sizeof(float)),
                 BitConverter.ToSingle(value[2].Buffer, sizeof(float)*2)),
            Gyroscope = new Scalar(BitConverter.ToSingle(value[2].Buffer,sizeof(float)*3), 
                 BitConverter.ToSingle(value[2].Buffer, sizeof(float)*4),
                 BitConverter.ToSingle(value[2].Buffer, sizeof(float)*5))
        });
    }

    public class OmniceptIMU 
    {
        public OmniceptGliaTimestamp Timestamp;
        public Scalar Acceleration;
        public Scalar Gyroscope;
    }
}
