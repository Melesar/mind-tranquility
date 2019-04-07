using System;
using Framework.EventListeners;
using Framework.Events;
using UnityEngine;
using UnityEngine.Events;

public class WinnerEventListener : GameEventListener<Winner>
{
    [SerializeField]
    private WinnerEvent _evt;
    [SerializeField]
    private WinnerUnityEvent _onRaised;

    protected override GameEvent<Winner> GameEvent => _evt;
    protected override UnityEvent<Winner> OnRaised => _onRaised;

    [Serializable]
    public class WinnerUnityEvent : UnityEvent<Winner>
    {
    }
}