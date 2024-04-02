using Bonsai;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Xml;
using System.Xml.Serialization;

[Combinator]
[TypeVisualizer(typeof(StreamingStatusVisualizer))]
[Description("Generates a sequence of boolean values indicating whether the sequence is emitting notifications within a specified interval.")]
[WorkflowElementCategory(ElementCategory.Combinator)]
public class StreamingStatus
{
    [Description("The text in the status display label.")]
    public string Name { get; set; }

    [XmlIgnore]
    [Description("The expected maximum interval between notifications emitted by the source sequence.")]
    public TimeSpan Interval { get; set; }

    [Browsable(false)]
    [XmlElement("Interval")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string IntervalXml
    {
        get { return XmlConvert.ToString(Interval); }
        set { Interval = XmlConvert.ToTimeSpan(value); }
    }

    public IObservable<bool> Process<TSource>(IObservable<TSource> source)
    {
        return source.Publish(ps =>
            ps.Timeout(Interval)
              .Select(_ => true)
              .Catch<bool, TimeoutException>(ex =>
                Observable.Return(false)
                          .Concat(Observable.Throw<bool>(ex)))
              .Retry());
    }
}
