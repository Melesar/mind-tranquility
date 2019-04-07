using System.Net;

namespace Common
{
    public class SendMessageTask : IUnityTask
    {
        private readonly UDPConnection _connection;
        private readonly IMessage _message;
        private readonly IPEndPoint _address;
        
        public void Execute()
        {
            if (_address != null)
            {
                _connection.Send(_message, _address);
            }
            else
            {
                _connection.Send(_message);
            }
        }

        public SendMessageTask(UDPConnection connection, IMessage message, IPEndPoint address)
        {
            _connection = connection;
            _message = message;
            _address = address;
        }
        
        public SendMessageTask(UDPConnection connection, IMessage message)
        {
            _connection = connection;
            _message = message;
            _address = null;
        }
    }
}