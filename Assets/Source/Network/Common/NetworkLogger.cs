using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Common;
using UnityEngine;

namespace Common
{
    public enum LogType  {Log, Warning, Error}
    
    public class NetworkLogger
    {
        private readonly bool _isClient;
    
        private static readonly HashSet<Type> _ignoreMessages = new HashSet<Type>
        {
            typeof(Ping)
        };

        private const string CLIENT_PREFIX = "<color=red>Client</color>";
        private const string SERVER_PREFIX = "<color=green>Server</color>";

        private string Prefix => _isClient ? CLIENT_PREFIX : SERVER_PREFIX;
    
        public void Log(string message, LogType type = LogType.Log, IMessage context = null)
        {
            if (context != null && _ignoreMessages.Contains(context.GetType()))
            {
                return;
            }
            
            message = $"{Prefix} {message}";
            switch (type)
            {
                case LogType.Log:
                    Debug.Log(message);
                    break;
                case LogType.Warning:
                    Debug.LogWarning(message);
                    break;
                case LogType.Error:
                    Debug.LogError(message);
                    break;
            }
        }

        public void LogSendMessage(IMessage message, IPEndPoint address = null)
        {
            var sb = new StringBuilder();
            sb.Append("Sent ").Append(message.GetType());
            if (address != null)
            {
                sb.Append(" to ").Append(address);
            }

            Log(sb.ToString(), context: message);
        }
        
        public NetworkLogger(bool isClient)
        {
            _isClient = isClient;
        }
    }
}
