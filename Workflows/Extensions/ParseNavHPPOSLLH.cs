using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.Remoting.Proxies;

/*
https://docs.ros.org/en/noetic/api/ublox_msgs/html/msg/NavHPPOSLLH.html

uint8 CLASS_ID = 1
uint8 MESSAGE_ID = 20

uint8 version
uint8[2] reserved1
int8 invalid_llh

uint32 iTOW             # GPS Millisecond Time of Week [ms]

int32 lon               # Longitude [deg / 1e-7]
int32 lat               # Latitude [deg / 1e-7]
int32 height            # Height above Ellipsoid [mm]
int32 hMSL              # Height above mean sea level [mm]
int8 lonHp              # Longitude [deg / 1e-9, range -99 to +99]
int8 latHp              # Latitude [deg / 1e-9, range -99 to +99]
int8 heightHp          # Height above Ellipsoid [mm / 0.1, range -9 to +9]
int8 hMSLHp            # Height above mean sea level [mm / 0.1, range -9 to +9]
uint32 hAcc             # Horizontal Accuracy Estimate [mm / 0.1]
uint32 vAcc             # Vertical Accuracy Estimate [mm / 0.1]

*/


[Combinator]
[Description("")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class ParseNavHPPOSLLH
{
    public int Offset { get; set; }
    public IObservable<HPPosLLH> Process(IObservable<byte[]> source)
    {
        return source.Select(value => new HPPosLLH {
            Version = value[0+Offset],
            Invalid_llh = value[3+Offset],
            ITOW = BitConverter.ToUInt32(value,4+Offset),
            Lon = BitConverter.ToInt32(value, 8+Offset)*0.0000001,
            Lat = BitConverter.ToInt32(value, 12+Offset)*0.0000001,
            Height = BitConverter.ToInt32(value, 16+Offset),
            HMSL = BitConverter.ToInt32(value, 20+Offset),
            LonHp = value[24+Offset]*0.000000001,
            latHp = value[25+Offset]*0.000000001,
            heightHp = value[26+Offset]*0.1,
            HMSLHp = value[27+Offset]*0.1,
            HAcc = BitConverter.ToUInt32(value, 28+Offset)*0.1,
            VAcc = BitConverter.ToUInt32(value, 32+Offset)*0.1

        });
    }
}
/// <summary>
/// High Precision Geodetic Position Solution
/// </summary>
public struct HPPosLLH
{
    public byte Version;
    // private byte reserved1;
    // private byte reserved2;

    public byte MESSAGE_ID;
    public byte Invalid_llh;
    /// <summary>
    /// GPS Millisecond Time of Week [ms]
    /// </summary>
    public UInt32 ITOW;
    /// <summary>
    /// Longitude [deg / 1e-7]
    /// </summary>
    public double Lon;
    /// <summary>
    /// Latitude [deg / 1e-7]
    /// </summary>
    public double Lat;
    /// <summary>
    /// Height above Ellipsoid [mm]
    /// </summary>
    public Int32 Height;
    /// <summary>
    /// Height above mean sea level [mm]
    /// </summary>
    public Int32 HMSL;
    /// <summary>
    /// Longitude [deg / 1e-9, range -99 to +99]
    /// </summary>
    public double LonHp;
    /// <summary>
    /// Latitude [deg / 1e-9, range -99 to +99]
    /// </summary>
    public double latHp;
    /// <summary>
    /// Height above Ellipsoid [mm / 0.1, range -9 to +9]
    /// </summary>
    public double heightHp;
    /// <summary>
    /// Height above mean sea level [mm / 0.1, range -9 to +9]
    /// </summary>
    public double HMSLHp;
    /// <summary>
    /// Horizontal Accuracy Estimate [mm / 0.1]
    /// </summary>
    public double HAcc;
    /// <summary>
    /// Vertical Accuracy Estimate [mm / 0.1]
    /// </summary>
    public double VAcc;

}
