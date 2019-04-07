using Common;

[MessageHeader(Id = 10)]
public class ConnectionRequest : IMessage
{
    public string PlayerName { get; private set; }

    
    public ConnectionRequest(string playerName)
    {
        PlayerName = playerName;
    }

    public ConnectionRequest()
    {
        
    }

    public byte[] ToBinary()
    {
        return Serialization.ToBinary(writer => writer.Write(PlayerName));
    }

    public void FromBinary(byte[] bytes)
    {
        Serialization.FromBinary(bytes, reader => PlayerName = reader.ReadString());
    }
}