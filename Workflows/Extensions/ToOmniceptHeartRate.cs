using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using NetMQ;

[Combinator]
[Description("Convert Glia heart rate ZeroMQ messages into bonsai compliant data structures")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class ToOmniceptHeartRate
{
    public IObservable<OmniceptHeartRate> Process(IObservable<NetMQMessage> source)
    {
        return source.Select(value => new OmniceptHeartRate {
            Timestamp = new OmniceptGliaTimestamp(value[1]),
            // HardwareTimeMicroseconds = BitConverter.ToInt64(value[1].Buffer, 0),
            // OmniceptTimeMicroseconds = BitConverter.ToInt64(value[2].Buffer, 0),
            // SystemTimeMicroseconds = BitConverter.ToInt64(value[3].Buffer, 0),
            Rate = BitConverter.ToUInt32(value[2].Buffer, 0)
        });
    }

    public class OmniceptHeartRate
    {
        public OmniceptGliaTimestamp Timestamp;
     
        public uint Rate;
    }
}
