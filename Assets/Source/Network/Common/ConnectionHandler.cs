using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace Common
{
    public abstract class ConnectionHandler : IConnectionListener, IDisposable
    {
        private readonly ITaskQueue _taskQueue = new TaskQueue();
        private readonly Dictionary<Type, MessageHandler> _messageHandlers = new Dictionary<Type, MessageHandler>();

        protected static readonly IUnityTask EmptyTask = new EmptyTask();
        protected readonly UDPConnection _connection;
        protected readonly NetworkLogger _logger;

        public virtual void OnMessageReceived(IMessage message, IPEndPoint senderAddress)
        {
            AddTask(message, senderAddress);
        }

        public abstract void OnDisconnect(IPEndPoint clientAddress);
        
        public virtual void Update(float delta)
        {
            _taskQueue.ExecuteNextTask();
        }

        public virtual void Dispose()
        {
            _connection.Dispose();
            _taskQueue.Clear();
        }

        protected void SetMessageHandler<T>(MessageHandlerDelegate<T> handler) where T : IMessage
        {
            _messageHandlers.Add(typeof(T), new MessageHandler<T>(handler, _logger));
        }

        private IUnityTask HandleMessage(IMessage message, IPEndPoint address)
        {
            MessageHandler handler;
            if (!_messageHandlers.TryGetValue(message.GetType(), out handler))
            {
                return EmptyTask;
            }
            
            return handler.Invoke(message, address) ?? EmptyTask;
        }

        protected ConnectionHandler(UDPConnection connection, NetworkLogger logger)
        {
            _logger = logger;
            _connection = connection;
            _connection.Listen(this);
        }

        private void AddTask(IMessage message, IPEndPoint senderAddress)
        {
            var task = HandleMessage(message, senderAddress);
            _taskQueue.AddTask(task);
        }

        protected void AddTask(IUnityTask task)
        {
            _taskQueue.AddTask(task);
        }
    }
}