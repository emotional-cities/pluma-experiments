using System;
using NetMQ;
public class OmniceptGliaTimestamp
{
    public long HardwareTimeMicroSeconds;
    public long OmniceptTimeMicroSeconds;
    public long SystemTimeMicroSeconds;
    public OmniceptGliaTimestamp(NetMQFrame frame)
    {
        HardwareTimeMicroSeconds = BitConverter.ToInt64(frame.Buffer, 0);
        OmniceptTimeMicroSeconds = BitConverter.ToInt64(frame.Buffer,sizeof(Int64));
        SystemTimeMicroSeconds = BitConverter.ToInt64(frame.Buffer, sizeof(Int64)*2);
    }
}