using System.Net;

namespace Common
{
    public interface IConnectionListener
    {
        void OnMessageReceived(IMessage message, IPEndPoint senderAddress);
        void OnDisconnect(IPEndPoint clientAddres);
    }
}