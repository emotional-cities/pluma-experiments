using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Microsoft.Win32;
using Bonsai.Expressions;


[Combinator]
[Description("This node returns the distance in meters between two points given by earth coordinates ")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class LatLongDistance
{
    /// <summary>
    /// Latitude of origin of path
    /// </summary>
    public double Lat { get; set; }
    /// <summary>
    /// Longitude of origin of path
    /// </summary>
    public double Lon { get; set; }
    
    public IObservable<double> Process(IObservable<HPPosLLH> source)
    {
        return source.Select(value =>
        {
            var lat1 = value.Lat;
            var lon1 = value.Lon;
            var lat2 = Lat;
            var lon2 = Lon;
            return Math.Acos(Math.Sin(lat1)*Math.Sin(lat2)+Math.Cos(lat1)*Math.Cos(lat2)*Math.Cos(lon2-lon1))*6371000;
        });
    }
}
