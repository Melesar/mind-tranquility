using System;
using Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class NetworkController : MonoBehaviour
{
    [FormerlySerializedAs("_connectionHandler")]
    [SerializeField]
    private ConnectionHandle _connectionHandle;
    [SerializeField]
    private SynchronizationData _synchronizationData;
    [SerializeField]
    private bool _localSession;

    private ServerApplication _serverApplication;
    private ClientApplication _clientApplication;

    private void StartSession()
    {
        _synchronizationData.myName.Value = _connectionHandle.PlayerName;

        var clientConnection = UDPConnection.CreateClient();
        _clientApplication = new ClientApplication(clientConnection, _synchronizationData);
        
        if (_connectionHandle.IsHost)
        {
            _serverApplication = new ServerApplication(UDPConnection.CreateServer(), _localSession);
            _serverApplication.matchCreated += OnSessionStart;
            
            _connectionHandle.AddTask(_serverApplication.WaitForClientsToBeReady(), TaskHandle.PRIORITY_NETWORK);
        }
        else
        {
            _clientApplication.connected += OnSessionStart;
        }
        
        _connectionHandle.AddTask(_clientApplication.NotifyClientIsReady(), TaskHandle.PRIORITY_NETWORK);
    }

    private void StopSession()
    {
        if (_clientApplication == null)
        {
            return;
        }
        
        _serverApplication?.Dispose();
        _clientApplication?.Dispose();

        _serverApplication = null;
        _clientApplication = null;
     
        _synchronizationData.Clear();
        
        OnSessionEnd();
    }

    private void OnSessionStart()
    {
        ExecuteEvents.Execute<ISessionListener>(gameObject, null, (handler, data) => handler.OnSessionStart());
    }

    private void OnSessionEnd()
    {
        ExecuteEvents.Execute<ISessionListener>(gameObject, null, (handler, data) => handler.OnSessionEnd());
    }

    public void OnGameOver(Winner winner)
    {
        StopSession();
    }

    private void Update()
    {
        var delta = Time.deltaTime;
        _serverApplication?.Update(delta);
        _clientApplication?.Update(delta);
    }

    private void Start()
    {
        _connectionHandle.SetCoroutineHolder(this);
        _connectionHandle.readyToConnect += StartSession;
        _connectionHandle.abortConnection += StopSession;
    }

    private void OnDestroy()
    {
        _connectionHandle.readyToConnect -= StartSession;
        _connectionHandle.abortConnection -= StopSession;
        StopSession();
    }
}