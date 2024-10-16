using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using NetMQ;
using OpenCV.Net;

[Combinator]
[Description("Get Int from Byte[]")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class ByteArrayToInt
{
    public IObservable<int> Process(IObservable<Byte[]> source)
    {
        return source.Select(value => 
            BitConverter.ToInt32(value, 0));
    }
}
