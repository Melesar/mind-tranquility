using System.Collections;
using Framework.References;
using UnityEngine;

public class FakeCoroutineMindReader : IMindReader
{
    private readonly MonoBehaviour _holder;
    private readonly float _updateInterval;
    private readonly Coroutine _coroutine;

    private IEnumerator MindReaderCoroutine()
    {
        while (true)
        {
            Meditation.Value = (1 + Mathf.Sin(Time.time * Mathf.PI)) / 2f;
            Focus.Value = (1 + Mathf.Cos(Time.time * Mathf.PI)) / 2f;

            yield return new WaitForSeconds(_updateInterval);
        }
    }
    
    public FakeCoroutineMindReader(MonoBehaviour holder, float updateInterval)
    {
        _holder = holder;
        _updateInterval = updateInterval;
        
        Meditation = new FloatReference {useConstantValue = true};
        Focus = new FloatReference {useConstantValue = true};
        SignalStrength = SignalStrength.GoodSignal;
        
        _coroutine = _holder.StartCoroutine(MindReaderCoroutine());
    }

    public void Dispose()
    {
        if (_holder != null)
        {
            _holder.StopCoroutine(_coroutine);
        }
    }

    public FloatReference Meditation { get; }
    public FloatReference Focus { get; }
    public SignalStrength SignalStrength { get; }
}