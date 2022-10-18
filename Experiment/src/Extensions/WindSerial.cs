using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.IO.Ports;
using System.Threading;
using Bonsai.IO;
using System.Threading.Tasks;
using System.Text;

[Combinator]
[Description("")]
[WorkflowElementCategory(ElementCategory.Source)]
public class WindSerial
{
    static readonly Encoding Iso8859 = Encoding.GetEncoding("iso-8859-1");

    [TypeConverter(typeof(SerialPortNameConverter))]
    public string PortName { get; set; }

    public string RequestString { get; set; }

    public int PollingInterval { get; set; }
    public int Timeout { get; set; }

    public IObservable<string> Process()
    {
        return Observable.Create<string>((observer, cancellationToken) =>
        {
            return Task.Factory.StartNew(() =>
            {
                using (var serialPort = new SerialPort(PortName, 9600))
                using (var notification = cancellationToken.Register(serialPort.Close))
                {
                    serialPort.Encoding = Iso8859;
                    serialPort.DtrEnable = true;
                    serialPort.RtsEnable = true;
                    serialPort.ReadTimeout = Timeout;
                    serialPort.Open();
                    Thread.Sleep(2000);

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        serialPort.Write(RequestString);
                        string read = null;
                        try
                        {
                            read = serialPort.ReadLine();
                        }
                        catch (TimeoutException)
                        {
                            Console.WriteLine("ATMOS22 SerialTimout");
                            continue;
                        }
                        var response = read.Split('\t');
                        if (response.Length < 2)
                        {
                            Console.WriteLine("ATMOS22 Bad String no TAB");
                            continue;
                        }
                        //var value = response[1];
                        var values = response[1].Split('\r');
                        if (values.Length < 2 || values[1].Length < 3)
                        {
                            Console.WriteLine("ATMOS22 Bad String no CRC, Checksum or SensorType in it");
                            continue;
                        }
                        var checksums = Iso8859.GetBytes(values[1]);
                        if (!LegacyChecksum('\t'+response[1], checksums[1]))
                        {
                            Console.WriteLine("ATMOS22 LegacyChecksum error");
                            continue;
                        }

                        // value = "\t0.26 1.27 0.37 23.1 3.2 4.8 0\r\\Hg";

                        if (!CRC6_Offset('\t'+response[1], checksums[2]))
                        {
                            Console.WriteLine("ATMOS22 CRC6_Offset error");
                            continue;
                        }

                        observer.OnNext(values[0]);
                        Thread.Sleep(PollingInterval);
                    }
                }
            },
            cancellationToken,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default);
        });
    }
    private bool LegacyChecksum(string response, byte checksum)
    {
        var bytes = Iso8859.GetBytes(response);
        ushort sum = 0;
        // Finding the length of the response string
        // Adding characters in the response together
        for (var count = 0; count < bytes.Length; count++)
        {
            sum += bytes[count];
            // check the beginning of the metadata section of the response

            if (bytes[count] == '\r')
            {
                if (count + 1 < bytes.Length)
                {
                    sum += bytes[count + 1];
                    // Console.WriteLine( "LegacyChecksum result "+(char)(sum % 64 + 32)+" vs "+(char)checksum);
                    // Console.WriteLine((sum % 64 + 32)+" vs "+checksum);
                    return (byte)(sum % 64 + 32) == checksum;
                }
                break;
            }
        }
        return false;
    }
    private bool CRC6_Offset(string response, byte value)
    {
        var buffer = Iso8859.GetBytes(response);
        byte crc = 0xfc; // Set upper 6 bits to 1’s
                        // Calculate total message length—updated once the metadata section is found

        var bytes = buffer.Length;
        // Loop through all the bytes in the buffer
        for (int i = 0; i < bytes; i++)
        {
            // Get the next byte in the buffer and XOR it with the crc
            crc ^= buffer[i];
            // Loop through all the bits in the current byte
            for (int bit = 8; bit > 0; bit--)
            {
                // If the uppermost bit is a 1...
                if ((crc & 0x80) != 0)
                {
                    // Shift to the next bit and XOR it with a polynomial
                    //crc = (byte)(((int)crc << 1) ^ 0x9c);
                    crc<<=1;
                    crc^=0x9c;
                }
                else
                {
                    // Shift to the next bit
                    crc <<= 1;
                }
            }
            if (buffer[i] == '\r')
            {
                // Found the beginning of the metadata section of the response
                // both sensor type and legacy checksum are part of the crc6
                // this requires only two more iterations of the loop so reset
                // "bytes"
                // bytes is incremented at the beginning of the loop, so 3 is added
                //Console.WriteLine("Old Bytes:"+bytes+" new bytes are:"+(i+3));
                bytes = i + 3;
            }
        }
        // Shift upper 6 bits down for crc
        crc >>=  2;
        // Console.WriteLine( "CRC6 result "+(char)(crc + 48)+" vs "+(char)value);
        // Console.WriteLine((crc+48)+" vs "+value);
        // Add 48 to shift crc to printable character avoiding \r \n and !
        return (crc + 48) == value;
    }

}
