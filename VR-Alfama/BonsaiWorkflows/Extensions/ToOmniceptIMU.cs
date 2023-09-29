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
public class ToOmniceptIMU
{
    public IObservable<OmniceptIMU> Process(IObservable<NetMQMessage> source)
    {
        return source.Select(value => new OmniceptIMU {
            HardwareTimeMicroseconds = BitConverter.ToInt64(value[1].Buffer, 0),
            OmniceptTimeMicroseconds = BitConverter.ToInt64(value[2].Buffer, 0),
            SystemTimeMicroseconds = BitConverter.ToInt64(value[3].Buffer, 0),
            AccelerationX = BitConverter.ToSingle(value[4].Buffer, 0),
            AccelerationY = BitConverter.ToSingle(value[5].Buffer, 0),
            AccelerationZ = BitConverter.ToSingle(value[6].Buffer, 0)
        });
    }

    public class OmniceptIMU : OmniceptSensorDataPoint {
        public float AccelerationX;
        public float AccelerationY;
        public float AccelerationZ;
    }
}
