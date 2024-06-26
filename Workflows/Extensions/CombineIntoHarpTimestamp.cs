using Bonsai;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using Bonsai.Harp;


[Combinator]
[Description("Creates a timestamped structure from a value-timestamp pair.")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class CombineIntoHarpTimestamp
{
    public IObservable<Timestamped<TSource>> Process<TSource>(IObservable<Tuple<TSource, double>> source)
    {
        return source.Select(value => Timestamped.Create(value.Item1, value.Item2));
    }
}
