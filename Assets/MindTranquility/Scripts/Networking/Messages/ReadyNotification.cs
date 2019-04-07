using System;
using Common;

[MessageHeader(Id = 13)]
public class ReadyNotification : IMessage
{
    public byte[] ToBinary()
    {
        return Array.Empty<byte>();
    }

    public void FromBinary(byte[] bytes)
    {
    }
}