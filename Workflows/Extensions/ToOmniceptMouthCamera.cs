using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using NetMQ;
using OpenCV.Net;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Activation;
using Bonsai.Vision;
using System.Drawing;
using System.Xml.Schema;

[Combinator]
[Description("Convert Glia mouth camera ZeroMQ messages into bonsai compliant data structures")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class ToOmniceptMouthCamera
{
    public IObservable<OmniceptMouthCamera> Process(IObservable<NetMQMessage> source)
    {
        return source.Select(value => 
        {
            var width =  BitConverter.ToUInt32(value[2].Buffer, 0);
            var height = BitConverter.ToUInt32(value[2].Buffer, sizeof(UInt32));
            int lenght = (int) (width*height);
            byte[] imageBuffer = new byte[lenght];
            Buffer.BlockCopy(value[2].Buffer, sizeof(UInt32)*2, imageBuffer, 0, lenght);
            IplImage image = (IplImage)Mat.FromArray( imageBuffer, (int)height ,(int)width, Depth.U8, 1);
            return new OmniceptMouthCamera {
                TimeStamp = new OmniceptGliaTimestamp(value[1]),
                Image = image
            };
        });
    }

    public class OmniceptMouthCamera 
    {
        public OmniceptGliaTimestamp TimeStamp;
        public IplImage Image;
    }
}
