using Bonsai;
using System;
using System.ComponentModel;
using System.Reactive.Linq;

[Combinator]
[Description("Ensure that all strings in the sequence are not null or empty.")]
[WorkflowElementCategory(ElementCategory.Combinator)]
public class EnsureIdentifier
{
    [Description("The message to display in case of validation errors.")]
    public string Message { get; set; }

    public IObservable<string> Process(IObservable<string> source)
    {
        return source.Select(value =>
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new InvalidOperationException(Message);
            }

            return value.Replace(" ", string.Empty);
        });
    }
}
