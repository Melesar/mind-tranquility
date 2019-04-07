using System;
using Framework.References;

public interface IMindReader : IDisposable
{
    FloatReference Meditation { get; }
    FloatReference Focus { get; }
    SignalStrength SignalStrength { get; }
}