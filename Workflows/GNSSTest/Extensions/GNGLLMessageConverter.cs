using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

[Combinator]
[Description("")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class GNGLLMessageConverter
{
    
    public IObservable<Tuple<double, double>> Process(IObservable<string[]> source)
    {   
    

        return source.Select(value =>
        {     

            double val1 = double.Parse(value[1]);
            val1 = CoordinateHelper.CoordinateConversion(val1);
            val1 *= (value[2] == "N") ? 1 : -1;
            
            double val2 = double.Parse(value[3]);
            val2 = CoordinateHelper.CoordinateConversion(val2);
            val2 *= (value[4] == "E") ? 1 : -1;
            return Tuple.Create(val1, val2);
        }
        );
    }
}
