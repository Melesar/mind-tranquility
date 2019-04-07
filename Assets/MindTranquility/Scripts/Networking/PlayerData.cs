using Common;

[MessageHeader(Id = 12)]
public class PlayerData : IMessage
{
    public float Meditation { get; set; }
    public string Name { get; set; }

    public PlayerData(byte[] binary)
    {
        FromBinary(binary);
    }

    public PlayerData()
    {
        
    }
    
    public byte[] ToBinary()
    {
        return Serialization.ToBinary(writer =>
        {
            writer.Write(Meditation);
            writer.Write(Name);
        });
    }

    public void FromBinary(byte[] bytes)
    {
        if (bytes.Length == 0)
        {
            return;
        }
        
        Serialization.FromBinary(bytes, reader =>
        {
            Meditation = reader.ReadSingle();
            Name = reader.ReadString();
        });
    }
}