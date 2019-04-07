using System;
using System.Net;

namespace Common
{
    public delegate IUnityTask MessageHandlerDelegate<T>(T message, IPEndPoint address);

    abstract class MessageHandler
    {
        public abstract IUnityTask Invoke(IMessage message, IPEndPoint address);
    }
    
    class MessageHandler<T> : MessageHandler where T : IMessage
    {
        private readonly MessageHandlerDelegate<T> _handler;
        private readonly NetworkLogger _logger;

        public override IUnityTask Invoke(IMessage message, IPEndPoint address)
        {
            if (!(message is T))
            {
                return null;
            }

            _logger.Log($"Received {message.GetType().Name} from {address}", context: message);              
            return _handler?.Invoke((T) message, address);

        }

        public MessageHandler(MessageHandlerDelegate<T> handler, NetworkLogger logger)
        {
            _handler = handler;
            _logger = logger;
        }
    }
}