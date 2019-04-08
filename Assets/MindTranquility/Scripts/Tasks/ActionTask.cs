using System;
using System.Collections;
using UnityEngine;

public class ActionTask : ITask
{
    private readonly Action _action;

    public ActionTask(Action action)
    {
        _action = action;
    }

    public void Execute() 
    {
        _action?.Invoke();
        onFinish?.Invoke();
    }

    public event Action onFinish;
    public bool IsExecuting => false;
}