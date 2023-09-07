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
public class CopyZeroMq
{
    public IObservable<NetMQMessage> Process(IObservable<NetMQMessage> source)
    {
        return source.Select(value => {
            List<byte[]> copied = new List<byte[]> {
                (byte[])value[0].Buffer.Clone(),
                (byte[])value[1].Buffer.Clone(),
                (byte[])value[2].Buffer.Clone()
            };

            var msg = new NetMQMessage(copied);
            return msg;
        });
    }
}
