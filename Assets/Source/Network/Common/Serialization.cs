using System;
using System.IO;

namespace Common
{
    public static class Serialization
    {
        public static byte[] ToBinary(Action<BinaryWriter> action)
        {
            var stream = new MemoryStream();
            using (var writer = new BinaryWriter(stream))
            {
                action?.Invoke(writer);
                return stream.ToArray();
            }
        }

        public static void FromBinary(byte[] bytes, Action<BinaryReader> action)
        {
            using (var reader = new BinaryReader(new MemoryStream(bytes)))
            {
                action?.Invoke(reader);
            }
        }
    }
}