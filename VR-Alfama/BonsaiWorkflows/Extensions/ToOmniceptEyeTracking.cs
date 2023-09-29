using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using NetMQ;
using System.Runtime.CompilerServices;

[Combinator]
[Description("")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class ToOmniceptEyeTracking
{
    public IObservable<OmniceptEyeTracking> Process(IObservable<NetMQMessage> source)
    {
        return source.Select(value => new OmniceptEyeTracking {
            HardwareTimeMicroseconds = BitConverter.ToInt64(value[1].Buffer, 0),
            OmniceptTimeMicroseconds = BitConverter.ToInt64(value[2].Buffer, 0),
            SystemTimeMicroseconds = BitConverter.ToInt64(value[3].Buffer, 0),
            X = BitConverter.ToSingle(value[4].Buffer, 0),
            Y = BitConverter.ToSingle(value[5].Buffer, 0),
            Z = BitConverter.ToSingle(value[6].Buffer, 0)
        });
    }

    public class OmniceptEyeTracking : OmniceptSensorDataPoint {
        public float X;
        public float Y;
        public float Z;
    }
}

public class OmniceptSensorDataPoint {
    public long HardwareTimeMicroseconds;
    public long OmniceptTimeMicroseconds;
    public long SystemTimeMicroseconds;
}
