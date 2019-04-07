using System;
using System.Collections;
using UnityEngine;

public class ActionTask : Task
{
    private readonly Action _action;

    public ActionTask(MonoBehaviour coroutineHolder, Action action) : base(coroutineHolder)
    {
        _action = action;
    }

    protected override IEnumerator ExecuteCoroutine()
    {
        _action?.Invoke();
        yield break;
    }
}