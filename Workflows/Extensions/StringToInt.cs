using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

[Combinator]
[Description("")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class StringToInt
{
   
    public string[] Topic{ get; set;} 
    
    
    public IObservable<int> Process(IObservable<string> source)
    {
        return source.Select(value =>
        {
            return Array.IndexOf(Topic,value);
        });
    }
}
