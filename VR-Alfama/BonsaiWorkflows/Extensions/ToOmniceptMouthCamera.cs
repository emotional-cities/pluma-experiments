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
public class ToOmniceptMouthCamera
{
    public IObservable<OmniceptMouthCamera> Process(IObservable<NetMQMessage> source)
    {
        return source.Select(value => new OmniceptMouthCamera {
            HardwareTimeMicroseconds = BitConverter.ToInt64(value[1].Buffer, 0),
            OmniceptTimeMicroseconds = BitConverter.ToInt64(value[2].Buffer, 0),
            SystemTimeMicroseconds = BitConverter.ToInt64(value[3].Buffer, 0),
            ImageData = value[4].Buffer,
            Width = BitConverter.ToUInt32(value[5].Buffer, 0),
            Height = BitConverter.ToUInt32(value[6].Buffer, 0),
            Channels = BitConverter.ToInt32(value[7].Buffer, 0)
        });
    }

    public class OmniceptMouthCamera : OmniceptSensorDataPoint {
        public byte[] ImageData;
        public uint Width;
        public uint Height;
        public int Channels;
    }
}
