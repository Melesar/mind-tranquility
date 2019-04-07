using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Meditation/ConnectionHandler")]
public class ConnectionHandle : TaskHandle
{
    public string PlayerName { get; set; }
    public bool IsHost { get; set; }

    public event Action readyToConnect;
    public event Action abortConnection;
    
    public void ReadyToConnect()
    {
        readyToConnect?.Invoke();
    }

    public void AbortConnection()
    {
        abortConnection?.Invoke();
    }
}