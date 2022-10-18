using Bonsai;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using OpenCV.Net;

[Combinator]
[Description("")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class ParseAccelerometerString
{
    public IObservable<AccelerometerData> Process(IObservable<string> source)
    {
        return source.Select(value => {
            var output = new AccelerometerData();
            var data_split = value.Split(';');
            //Orientation
            var Orientation = data_split[0].Split(':')[1].Split(',');
            output.Orientation = new Point3f(float.Parse(Orientation[0]),float.Parse(Orientation[1]) , float.Parse(Orientation[2]));
            //Gyroscope
            var Gyroscope = data_split[1].Split(':')[1].Split(',');
            output.Gyroscope = new Point3f(float.Parse(Gyroscope[0]),float.Parse(Gyroscope[1]) , float.Parse(Gyroscope[2]));
            //LinearAcc
            var LinearAccl = data_split[2].Split(':')[1].Split(',');
            output.LinearAccl = new Point3f(float.Parse(LinearAccl[0]),float.Parse(LinearAccl[1]) , float.Parse(LinearAccl[2]));
            //Magnet
            var Magnetometer = data_split[3].Split(':')[1].Split(',');
            output.Magnetometer = new Point3f(float.Parse(Magnetometer[0]),float.Parse(Magnetometer[1]) , float.Parse(Magnetometer[2]));
            //Accl
            var Accl = data_split[4].Split(':')[1].Split(',');
            output.Accl = new Point3f(float.Parse(Accl[0]),float.Parse(Accl[1]) , float.Parse(Accl[2]));
            //Gravity
            var Gravitiy = data_split[5].Split(':')[1].Split(',');
            output.Gravitiy = new Point3f(float.Parse(Gravitiy[0]),float.Parse(Gravitiy[1]) , float.Parse(Gravitiy[2]));
            //Calibration
            var Calibration = data_split[6].Split(':')[1].Split(',');
            output.SysCalib = int.Parse(Calibration[0].Split('=')[1]) == 1;
            output.GyroCalib = int.Parse(Calibration[1].Split('=')[1]) == 1;
            output.AcclCalib = int.Parse(Calibration[2].Split('=')[1]) == 1;
            output.MagCalib = int.Parse(Calibration[3].Split('=')[1]) == 1;
            //Temperature
            var Temperature = float.Parse(data_split[7].Split('=')[1].Split(';')[0]);


            return output;
        });
    }
//Orient:359.94,-3.06,-109.31;Gyro1:-0.00,0.00,-0.00;Linear:0.00,0.00,0.00;Mag:48.19,-47.75,2.06;Accl:-0.52,9.24,-3.24;Gravity:-0.52,9.24,-3.24;Calibration:Sys=0,Gyro=1,Accel=0,Mag=0;Temp:value=37;

}
public struct AccelerometerData{
    public Point3f Orientation;
    public Point3f Gyroscope;
    public Point3f LinearAccl;
    public Point3f Magnetometer;
    public Point3f Accl;
    public Point3f Gravitiy;
    public bool SysCalib;
    public bool GyroCalib;
    public bool AcclCalib;
    public bool MagCalib;
    public float Temperature;
}