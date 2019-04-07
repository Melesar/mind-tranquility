using System;

namespace Common
{
    public interface IMessage
    {
        byte[] ToBinary();
        void FromBinary(byte[] bytes);
    }
}