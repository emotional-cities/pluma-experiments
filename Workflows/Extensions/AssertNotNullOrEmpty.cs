using Bonsai;
using System;
using System.ComponentModel;
using System.Reactive.Linq;

[Combinator]
[Description("Ensure that all strings in the sequence are not null or empty.")]
[WorkflowElementCategory(ElementCategory.Combinator)]
public class AssertNotNullOrEmpty
{
    public string Message { get; set; }

    public IObservable<string> Process(IObservable<string> source)
    {
        return source.Do(value =>
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new InvalidOperationException(Message);
            }
        });
    }
}
