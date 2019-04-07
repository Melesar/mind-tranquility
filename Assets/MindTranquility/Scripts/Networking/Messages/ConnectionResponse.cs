
using Common;

[MessageHeader(Id = 19)]
public class ConnectionResponse : IMessage
{
    public PlayerData Opponent { get; private set; }

    public ConnectionResponse(PlayerData opponent)
    {
        Opponent = opponent;
    }

    public ConnectionResponse()
    {
    }

    public byte[] ToBinary()
    {
        return Opponent != null ? Opponent.ToBinary() : new byte[0];
    }

    public void FromBinary(byte[] bytes)
    {
        Opponent = bytes.Length > 0 ? new PlayerData(bytes) : null;
    }
}