using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using OpenCV.Net;

[Combinator]
[Description("")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class DrawGaze
{
    public int Radius { get; set; }
    public Scalar Color { get; set; }
    public int Thickness { get; set; }
    public IObservable<IplImage> Process(IObservable<Tuple<IplImage, Tuple<float, float>>> source)
    {
        return source.Select(value => 
        {
            var image = value.Item1;
            Point center = new Point((int)value.Item2.Item1, (int)value.Item2.Item2);
            OpenCV.Net.CV.Circle(image,center, Radius, Color, Thickness);
            return image;
        });
    }
}
