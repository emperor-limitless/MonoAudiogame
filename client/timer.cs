using System.Diagnostics;

public class Timer
{
    private Stopwatch stp;
    public Timer()
    {
        stp = new();
        stp.Start();
    }
    public void Reset()
    {
        stp.Reset();
    }
    public void Restart()
    {
        stp.Restart();
    }
    public long Elapsed
    {
        get
        {
            return stp.ElapsedMilliseconds;
        }
    }
}
