using Framework.References;
using UnityEngine;

public class TimeMindReader : IMindReader
{
    public FloatReference Meditation
    {
        get
        {
            var value = (1 + Mathf.Sin(Time.time / 10f)) / 2f;
            _meditation.Value = value;
            return _meditation;
        }
    }

    public FloatReference Focus
    {
        get
        {
            var value = (1 + Mathf.Cos(Time.time / 10f)) / 2f;
            _focus.Value = value;
            return _focus;
        }
    }

    public SignalStrength SignalStrength => SignalStrength.GoodSignal;

    private readonly FloatReference _meditation;
    private readonly FloatReference _focus;

    public TimeMindReader()
    {
        _meditation = new FloatReference();
        _focus = new FloatReference();
    }

    public void Dispose()
    {
        
    }
}