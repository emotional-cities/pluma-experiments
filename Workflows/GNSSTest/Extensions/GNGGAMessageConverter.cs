using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

[Combinator]
[Description("")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class GNGGAMessageConverter
{
    public IObservable<GNGGAMessage> Process(IObservable<string[]> source)
    {
        return source.Select(value => 
        {     
            //Latitude
            double val1 = double.Parse(value[2]);
            val1 = CoordinateHelper.CoordinateConversion(val1);
            val1 *= (value[3] == "N") ? 1 : -1;
            //Longitude 
            double val2 = double.Parse(value[4]);
            val2 = CoordinateHelper.CoordinateConversion(val2);
            val2 *= (value[5] == "E") ? 1 : -1;
            //Status
            int val3 = int.Parse(value[6]);

            //Satellite number
            int val4 = int.Parse(value[7]);
            //Hdop - Horizontal Dilution Of Precision
            double val5 = double.Parse(value[8]);
            //Altitude above sea level
            double val6 = double.Parse(value[9]);
            
            return new GNGGAMessage()
            {
                Latitude= val1,
                Longitude=val2,
                GpsStatus= (GpsStatus)val3,
                SatelliteNumber = val4,
                Hdop = val5,
                Altitude = val6
            };
        }
        );
    }
}

public enum GpsStatus
{
    Invalid = 0,
    TwoD_ThreeD=1,
    DGNSS = 2,
    FixedRTK = 4,
    FloatRTK = 5,
    DeadReckoning = 6
};

public struct GNGGAMessage
{
    public double Latitude;
    public double Longitude;
    public GpsStatus GpsStatus;
    public int SatelliteNumber;
    public double Hdop;
    public double Altitude;

}