using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using NetMQ;

[Combinator]
[Description("Convert a ZeroMQ multipart message into a byte array adding a string terminator to the topic")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class ZeroMQMessageToByteArray
{
    public IObservable<byte[]> Process(IObservable<NetMQMessage> source)
    {
        return source.Select(value =>
        {
            var messageBytes = value.SelectMany(x => x.Buffer).ToArray();
            byte[] result = new byte[messageBytes.Count()+1];
            Buffer.BlockCopy(messageBytes, 0, result,0, value[0].BufferSize);
            messageBytes[value[0].BufferSize] = (byte)'\0'; // added to find the end of the topic string when desirializing
            Buffer.BlockCopy(messageBytes, value[0].BufferSize, result,value[0].BufferSize+1, messageBytes.Length - value[0].BufferSize);
            return result;
        });
    }
}
