using System;
using System.Collections;
using UnityEngine;

public abstract class Task : ITask
{
    private readonly MonoBehaviour _coroutineHolder;
    
    public void Execute()
    {
        IsExecuting = true;
        _coroutineHolder.StartCoroutine(ExecuteCoroutineInternal());
    }

    protected Task(MonoBehaviour coroutineHolder)
    {
        _coroutineHolder = coroutineHolder;
    }

    private IEnumerator ExecuteCoroutineInternal()
    {
        yield return _coroutineHolder.StartCoroutine(ExecuteCoroutine());
        IsExecuting = false;
        OnFinish();
        onFinish?.Invoke();
    }

    protected virtual void OnFinish()
    {
        
    }
    
    protected abstract IEnumerator ExecuteCoroutine();

    public event Action onFinish;
    public bool IsExecuting { get; private set; }
}