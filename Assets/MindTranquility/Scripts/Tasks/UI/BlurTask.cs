using System.Collections;
using DG.Tweening;
using UnityEngine;

public class BlurTask : Task
{
    private readonly BlurOptimized _blur;
    
    public BlurTask(MonoBehaviour coroutineHolder, BlurOptimized blur) : base(coroutineHolder)
    {
        _blur = blur;
    }

    protected override IEnumerator ExecuteCoroutine()
    {
        _blur.enabled = true;
        _blur.SwitchTarget();
        yield return new WaitForEndOfFrame();
        _blur.RestoreTarget();
        _blur.enabled = false;
    }
}