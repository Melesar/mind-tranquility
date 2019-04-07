using UnityEngine;

namespace Common
{
    [MessageHeader(Id = 3)]
    public class ClientInputMessage : IMessage
    {
        public ClientInputMessage(Vector2 inputCoords)
        {
            InputCoords = inputCoords;
        }

        public ClientInputMessage()
        {
            
        }

        public Vector2 InputCoords { get; set; }

        public byte[] ToBinary()
        {
            return Serialization.ToBinary(writer =>
            {
                writer.Write(InputCoords.x);
                writer.Write(InputCoords.y);
            });
        }

        public void FromBinary(byte[] bytes)
        {
            Serialization.FromBinary(bytes, reader =>
            {
                var coords = new Vector2
                {
                    x = reader.ReadSingle(),
                    y = reader.ReadSingle()
                };

                InputCoords = coords;
            });
        }
    }
}