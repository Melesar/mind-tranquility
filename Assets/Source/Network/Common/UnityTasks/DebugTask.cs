using System;
using UnityEngine;

namespace Common
{
    public class DebugTask : IUnityTask
    {
        private readonly string _message;
        private readonly LogType _type;
        private readonly NetworkLogger _logger;

        public void Execute()
        {
            _logger.Log(_message, _type);
        }

        public DebugTask(string message, bool isClient,  LogType type = LogType.Log)
        {
            _message = message;
            _type = type;
            _logger = new NetworkLogger(isClient);
        }

        public DebugTask(string message, NetworkLogger logger, LogType type = LogType.Log)
        {
            _message = message;
            _logger = logger;
            _type = type;
        }
    }
}