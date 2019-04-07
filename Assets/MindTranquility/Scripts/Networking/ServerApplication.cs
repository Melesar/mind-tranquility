using System;
using System.Collections.Generic;
using System.Net;
using Common;

public class ServerApplication : ConnectionHandler
{
    public event Action matchCreated;

    private bool IsFull => _connectedPlayers.Count == 2;

    private ClientsReadyTask _clientsReadyTask; 
    
    private readonly HashSet<IPEndPoint> _playersReady = new HashSet<IPEndPoint>();
    private readonly Dictionary<IPEndPoint, PlayerData> _connectedPlayers = new Dictionary<IPEndPoint, PlayerData>();

    public override void OnDisconnect(IPEndPoint clientAddress)
    {
        _connectedPlayers.Remove(clientAddress);
        
        //TODO invoke an event
    }

    public override void Update(float delta)
    {
        base.Update(delta);
        if (!IsFull)
        {
            _connection.Multicast(new Ping());
        }
    }

    private IUnityTask OnConnectionRequest(ConnectionRequest connectionRequest, IPEndPoint senderAddress)
    {
        if (IsPlayerConnected(senderAddress) || IsFull)
        {
            return EmptyTask;
        }

        //TODO (Optimization) Send whole player data only once and then send only meditation value
        var newPlayer = new PlayerData {Meditation = 0f, Name = connectionRequest.PlayerName};
        _connectedPlayers.Add(senderAddress, newPlayer);

        if (!IsFull)
        {
            var response = new ConnectionResponse();
            var responseTask = new Common.SendMessageTask(_connection, response, senderAddress);
            return responseTask;
        }

        var opponentData = GetAnotherPlayer(newPlayer);

        var connectionResponse = new ConnectionResponse(opponentData.Value);
        var sendToPlayer = new Common.SendMessageTask(_connection, connectionResponse, senderAddress);
        var sendToOpponent = new Common.SendMessageTask(_connection, newPlayer, opponentData.Key);
        var fireEvent = new FireEventTask(matchCreated);

        return new BatchTask(sendToOpponent, sendToPlayer, fireEvent);
    }

    private IUnityTask OnMeditationMessage(MeditationMessage message, IPEndPoint senderAddress)
    {
        if (!IsPlayerConnected(senderAddress))
        {
            return EmptyTask;
        }

        var meditation = message.MeditationValue;
        var player = _connectedPlayers[senderAddress];
        player.Meditation = meditation;
        var opponent = GetAnotherPlayer(player);

        return new Common.SendMessageTask(_connection, player, opponent.Key);
    }

    private IUnityTask OnReadyNotification(ReadyNotification message, IPEndPoint address)
    {
        if (!IsPlayerConnected(address))
        {
            return EmptyTask;
        }

        if (_playersReady.Add(address) && _playersReady.Count == 2)
        {
            _playersReady.Clear();
            _clientsReadyTask.SetReady();
            _clientsReadyTask = null;
        }

        return EmptyTask;
    }

    public ITask WaitForClientsToBeReady()
    {
        _clientsReadyTask = new ClientsReadyTask();
        return _clientsReadyTask;
    }

    private bool IsPlayerConnected(IPEndPoint senderAddress)
    {
        return _connectedPlayers.ContainsKey(senderAddress);
    }

    private KeyValuePair<IPEndPoint, PlayerData> GetAnotherPlayer(PlayerData player)
    {
        foreach (var data in _connectedPlayers)
        {
            if (data.Value != player)
            {
                return data;
            }            
        }
        
        return new KeyValuePair<IPEndPoint, PlayerData>();
    }

    private void RegisterFakeClient()
    {
        var fakeClient = new PlayerData {Meditation = 0.2f, Name = "Mahatma Gandi"};
        var fakeAddress = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1440);
        _connectedPlayers.Add(fakeAddress, fakeClient);
        _playersReady.Add(fakeAddress);
    }

    public ServerApplication(UDPConnection connection, bool registerFakeClient) : base(connection, new NetworkLogger(false))
    {
        if (registerFakeClient)
        {
            RegisterFakeClient();    
        }
        
        SetMessageHandler<ConnectionRequest>(OnConnectionRequest);
        SetMessageHandler<MeditationMessage>(OnMeditationMessage);
        SetMessageHandler<ReadyNotification>(OnReadyNotification);
        SetMessageHandler<PlayerData>(PDH);
    }

    private IUnityTask PDH(PlayerData message, IPEndPoint address)
    {
        return EmptyTask;
    }
}