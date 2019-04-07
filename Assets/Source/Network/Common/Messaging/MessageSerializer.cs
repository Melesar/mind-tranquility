using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Common
{
    public static class MessageSerializer
    {
        private static readonly Dictionary<int, Type> _messageTypes = new Dictionary<int, Type>();

        public static byte[] EncodeMessage(IMessage message)
        {
            var type = message.GetType();
            var attr = type.GetCustomAttribute<MessageHeaderAttribute>();
            if (attr == null)
            {
                Debug.LogError($"Got message of type {type} without MessageHeader attribute");
                return null;
            }
            
            return Serialization.ToBinary(writer =>
            {
                writer.Write(attr.Id);
                writer.Write(message.ToBinary());
            });
        }
        
        public static IMessage GetMessage(byte[] bytes)
        {
            var stream = new MemoryStream(bytes);
            var reader = new BinaryReader(stream);
            try
            {
                var msgId = reader.ReadInt32();
                Type msgType;
                if (!_messageTypes.TryGetValue(msgId, out msgType))
                {
                    return null;
                }

                var msgBytes = reader.ReadBytes((int) (stream.Length - stream.Position));
                var msgInstance = (IMessage) Activator.CreateInstance(msgType);
                msgInstance.FromBinary(msgBytes);
                return msgInstance;
            }
            catch (IOException)
            {
                Debug.LogError("Got malformed message");
                return null;
            }
            finally
            {
                reader.Dispose();
                stream.Dispose();
            }
        }

        static MessageSerializer()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(ass => ass.GetTypes()).ToList();
            foreach (var type in types)
            {
                var attr = type.GetCustomAttribute<MessageHeaderAttribute>();
                if (attr != null)
                {
                    _messageTypes.Add(attr.Id, type);
                }
            }
        }
    }
}