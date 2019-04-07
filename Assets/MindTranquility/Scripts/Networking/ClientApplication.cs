using System;
using System.Net;
using Common;
using UnityEngine;
using Ping = Common.Ping;

public class ClientApplication : ConnectionHandler
{
    public event Action connected;
    
    private readonly SynchronizationData _synchronizationData;
    
    private bool _isConnected;
    private IPEndPoint _pendingConnection;
    private IPEndPoint _connectedTo;
    private float _connectionTimer;
    private NotifyReadyTask _notificationTask;

    private const float CONNECTION_TIMEOUT = 2.5f;

    public ClientApplication(UDPConnection connection, SynchronizationData synchronizationData) : base (connection, new NetworkLogger(true))
    {
        _synchronizationData = synchronizationData;
        _synchronizationData.myMeditation.valueChanged += OnMeditationChanged;
        
        SetMessageHandler<Ping>(OnPing);
        SetMessageHandler<ConnectionResponse>(OnConnectionResponse);
        SetMessageHandler<PlayerData>(OnOpponentUpdate);
    }

    private IUnityTask OnPing(Ping ping, IPEndPoint senderAddress)
    {
        if (_isConnected || _pendingConnection != null)
        {
            return EmptyTask;
        }

        _pendingConnection = senderAddress;
        
        var connectionMessage = new ConnectionRequest(_synchronizationData.myName);
        var sendMessage = new Common.SendMessageTask(_connection, connectionMessage, senderAddress);
        var debug = new DebugTask($"Started waiting for connection", _logger);
        return new BatchTask(debug, sendMessage);
    }

    private IUnityTask OnConnectionResponse(ConnectionResponse response, IPEndPoint senderAddress)
    {
        if (_isConnected || (_pendingConnection != null && !senderAddress.Equals(_pendingConnection)))
        {
            return EmptyTask;
        }

        _isConnected = true;
        _connectedTo = senderAddress;
        _pendingConnection = null;
        _notificationTask.SetAddress(_connectedTo);
//        _connection.Connect(senderAddress);
        var debug = new DebugTask($"Connected to {_connectedTo}", _logger);

        var connectedEvent = new FireEventTask(connected);
        if (response.Opponent != null)
        {
            var updateOpponent = new UpdateOpponentData(_synchronizationData, response.Opponent);
            return new BatchTask(debug, connectedEvent, updateOpponent);
        }

        return new BatchTask(debug, connectedEvent);
    }

    private IUnityTask OnOpponentUpdate(PlayerData opponentData, IPEndPoint senderAddress)
    {
        if (!_isConnected || !senderAddress.Equals(_connectedTo))
        {
            return EmptyTask;
        }

        return new UpdateOpponentData(_synchronizationData, opponentData);
    }

    private void OnMeditationChanged(float oldValue, float newValue)
    {
        if (_isConnected)
        {
            _connection.Send(new MeditationMessage(newValue));
        }
    }

    public ITask NotifyClientIsReady()
    {
        _notificationTask = new NotifyReadyTask(_connection);
        return _notificationTask;
    }
    
    public override void Update(float delta)
    {
        base.Update(delta);

        if (_pendingConnection == null)
        {
            return;
        }

        if (_connectionTimer >= CONNECTION_TIMEOUT)
        {
            _logger.Log($"Connection timeout for {_pendingConnection} exceeded");
            _pendingConnection = null;
            return;
        }

        _connectionTimer += delta;
    }

    public override void Dispose()
    {
        base.Dispose();
        _synchronizationData.myMeditation.valueChanged -= OnMeditationChanged;
    }

    public override void OnDisconnect(IPEndPoint clientAddress)
    {
        
    }

    #region Tasks

    private class UpdateOpponentData : IUnityTask
    {
        private readonly SynchronizationData _synchronizationData;
        private readonly PlayerData _opponentData;
        
        public void Execute()
        {
            _synchronizationData.opponentName.Value = _opponentData.Name;
            _synchronizationData.opponentMeditation.Value = _opponentData.Meditation;
        }

        public UpdateOpponentData(SynchronizationData synchronizationData, PlayerData opponentData)
        {
            _synchronizationData = synchronizationData;
            _opponentData = opponentData;
        }
    }
    
    private class NotifyReadyTask : ITask
    {
        private IPEndPoint _address;

        private readonly UDPConnection _connection;
        
        public void Execute()
        {
            _connection.Send(new ReadyNotification(), _address);
            onFinish?.Invoke();
        }

        public event Action onFinish;
        public bool IsExecuting => false;

        public void SetAddress(IPEndPoint address)
        {
            _address = address;
        }

        public NotifyReadyTask(UDPConnection connection)
        {
            _connection = connection;
        }
    }

    #endregion
}