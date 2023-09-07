using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

[Combinator]
[Description("")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class DecodePupilGaze
{
    public IObservable<PupilGaze> Process(IObservable<byte[]> source)
    {
        return source.Select(value => new PupilGaze {
            X = BitConverter.ToSingle(value, 0),
            Y = BitConverter.ToSingle(value, 4)
        });
    }

    public struct PupilGaze {
        public float X;
        public float Y;
    }
}
