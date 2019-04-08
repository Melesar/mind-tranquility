using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Common
{
    public class UDPConnection : IDisposable
    {
        private Thread _listeningThread;
        private readonly UdpClient _client;

        private const int SERVER_PORT = 1444;
        private const int CLIENT_PORT = 1440;

        private static readonly IPAddress _multicastAddress = IPAddress.Parse("224.0.0.251");
        private static readonly IPEndPoint _multicastEndPoint = new IPEndPoint(_multicastAddress, CLIENT_PORT);
        private NetworkLogger _logger;

        public static UDPConnection CreateClient()
        {
            var client = new UdpClient(CLIENT_PORT);
            client.JoinMulticastGroup(_multicastAddress);
            
            var connection = new UDPConnection(client) {_logger = new NetworkLogger(true)};
            return connection;
        }

        public static UDPConnection CreateServer()
        {
            var client = new UdpClient(SERVER_PORT);
            var connection = new UDPConnection(client) {_logger = new NetworkLogger(false)};
            return connection;
        }

        public void Send(IMessage message)
        {
            try
            {
                var messageBytes = MessageSerializer.EncodeMessage(message);
                lock (_client)
                {
                    var sent = _client.Send(messageBytes, messageBytes.Length);
                    _logger.LogSendMessage(message);
                }
            }
            catch (Exception e)
            {
                _logger.Log(e.ToString(), LogType.Error);
            }
        }

        public void Send(IMessage message, IPEndPoint address)
        {
            try
            {
                var messageBytes = MessageSerializer.EncodeMessage(message);
                lock (_client)
                {
                    var sent = _client.Send(messageBytes, messageBytes.Length, address);
                    _logger.LogSendMessage(message, address);
                }
            }
            catch (Exception e)
            {
                _logger.Log(e.ToString(), LogType.Error);
            }
        }

        public void Connect(IPEndPoint address)
        {
            _client.Connect(address.Address, address.Port);
        }

        public void Multicast(IMessage message)
        {
            Send(message, _multicastEndPoint);
        }

        public void Listen(IConnectionListener listener)
        {
            _listener = listener;
            
            _listeningThread?.Abort();
            _listeningThread = new Thread(ReceiveTask);
            _listeningThread.Start();
        }

        private IConnectionListener _listener;
        
        private void ReceiveTask()
        {
            var senderAddress = new IPEndPoint(IPAddress.Any, 0);
            try
            {
                while (true)
                {
                    var data = _client.Receive(ref senderAddress);
                    var message = MessageSerializer.GetMessage(data);
                    if (message != null)
                    {
                        _listener.OnMessageReceived(message, senderAddress);
                    }

                    senderAddress = new IPEndPoint(IPAddress.Any, 0);
                }
            }
            catch (SocketException e)
            {
                switch (e.SocketErrorCode)
                {
                    case SocketError.ConnectionReset:
                        _listener.OnDisconnect(senderAddress);
                        break;
                    default:
                        throw e;
                }
            }
        }

        private UDPConnection(UdpClient client)
        {
            _client = client;
        }

        public void Dispose()
        {
            _client?.Dispose();
            _listeningThread?.Abort();
        }
    }
}