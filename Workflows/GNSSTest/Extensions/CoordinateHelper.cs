
using System;


public static class CoordinateHelper

{
    public static double CoordinateConversion(double val)
    { 
        //3836.67665765
        var value = 0.01f*val;
        //38.366765765
        int degrees = (int)value;
        // 38
        value -= degrees;
        //0.366765765
        //0.534535
        value *= 100;
        //36.6765765
        double minutes = value;
        // Console.WriteLine("minutes "+minutes);
        //36
        // value -=minutes;
        // var seconds  = value*100; //seconds
        // Console.WriteLine("seconds "+seconds);
        
        val = degrees+(minutes/60);//+ (seconds/3600);

        // value *= Math.Pow(10, 9);
        // //Console.WriteLine(val);
        // value = value / 60f;
        // //Console.WriteLine(val);
        // int digits = (int)Math.Floor(Math.Log10(val) + 1);
        // //Console.WriteLine(digits);
        // //Console.WriteLine("**************************");
        // value /= Math.Pow(10, digits);
        // value += tmp;
        return val;
    }
}