using Bonsai;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

[Combinator]
[Description("Returns the value of an environment variable.")]
[WorkflowElementCategory(ElementCategory.Source)]
public class GetEnvironmentVariable
{
    private string variable;
    public string Variable
    {
        get { return variable; }
        set { variable = value; }
    }

    public IObservable<string> Process()
    {
        return Observable.Return(Environment.GetEnvironmentVariable(Variable));
    }

    public IObservable<string> Process<TSource>(IObservable<TSource> source)
    {
        return source.Select(x => {return Environment.GetEnvironmentVariable(Variable);});
    }

}
