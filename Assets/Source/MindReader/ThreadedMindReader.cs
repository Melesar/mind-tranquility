using System.Threading;
using Framework.References;

public class ThreadedMindReader : MindReader
{
    private readonly int _sleepInterval;
    private readonly Thread _updateThread;
    private readonly object _lock = new object();

    public ThreadedMindReader(float updateInterval) : base(updateInterval)
    {
        _sleepInterval = (int) (updateInterval * 1000); 
        _updateThread = new Thread(ThreadUpdate);
        _updateThread.Start();
    }

    public override FloatReference Meditation
    {
        get
        {
            lock (_lock)
            {
                return base.Meditation;
            }
        }
    }

    public override FloatReference Focus
    {
        get
        {
            lock (_lock)
            {
                return base.Focus;
            }
        }
    }

    public override SignalStrength SignalStrength
    {
        get
        {
            lock (_lock)
            {
                return base.SignalStrength;
            }
        }
    }

    protected override void SetMeditation(int meditation)
    {
        lock (_lock)
        {
            base.SetMeditation(meditation);
        }
    }

    protected override void SetFocus(int focus)
    {
        lock (_lock)
        {
            base.SetFocus(focus);
        }
    }

    protected override void SetSignalStrength(int signalStrength)
    {
        lock (_lock)
        {
            base.SetSignalStrength(signalStrength);
        }
    }

    public override void Dispose()
    {
        base.Dispose();
        _updateThread.Abort();
    }

    private void ThreadUpdate()
    {
        while (true)
        {
            Update();
            Thread.Sleep(_sleepInterval);
        }
    }
}