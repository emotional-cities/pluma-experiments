using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using NetMQ;

[Combinator]
[Description("Extract Timestamp from  Glia ZeroMQ messages")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class GetOmniceptGliaTimestamp
{
    public IObservable<long[]> Process(IObservable<NetMQMessage> source)
    {
        return source.Select(value =>
        {
            var timestamp = new OmniceptGliaTimestamp(value[1]);
            long[] result = new long[3]{
                timestamp.HardwareTimeMicroSeconds, 
                timestamp.OmniceptTimeMicroSeconds, 
                timestamp.SystemTimeMicroSeconds};
            return result;
        });
    }
}
