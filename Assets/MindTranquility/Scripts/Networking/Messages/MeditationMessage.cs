using Common;

[MessageHeader(Id = 15)]
public class MeditationMessage : IMessage
{
    public float MeditationValue { get; private set; }

    public MeditationMessage(float meditationValue)
    {
        MeditationValue = meditationValue;
    }

    public MeditationMessage()
    {
        
    }

    public byte[] ToBinary()
    {
        return Serialization.ToBinary(writer => writer.Write(MeditationValue));
    }

    public void FromBinary(byte[] bytes)
    {
        Serialization.FromBinary(bytes, reader => MeditationValue = reader.ReadSingle());
    }
}