using System;
using Common;
using UnityEngine;

public class FireEventTask : IUnityTask
{
    private readonly Action _event;
        
    public void Execute()
    {
        _event?.Invoke();    
    }

    public FireEventTask(Action @event)
    {
        _event = @event;
    }
}