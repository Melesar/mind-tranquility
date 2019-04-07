using System;

namespace Common
{
    [MessageHeader(Id = 1)]
    public class Ping : IMessage
    {
        public byte[] ToBinary()
        {
            return Array.Empty<byte>();
        }

        public void FromBinary(byte[] bytes)
        {
        }
    }
}