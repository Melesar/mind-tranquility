using System.Collections;
using UnityEngine;

public class CoroutineMindReader : MindReader
{
    private readonly MonoBehaviour _coroutineHolder;
    private readonly Coroutine _coroutine;
    
    public CoroutineMindReader(MonoBehaviour coroutineHolder, float updateInterval) : base(updateInterval)
    {
        _coroutineHolder = coroutineHolder;
        _coroutine = _coroutineHolder.StartCoroutine(UpdateCoroutine());
    }

    private IEnumerator UpdateCoroutine()
    {
        while (true)
        {
            Update();
            yield return new WaitForSeconds(_updateInterval);
        }
    }

    public override void Dispose()
    {
        base.Dispose();
        _coroutineHolder.StopCoroutine(_coroutine);
    }
}