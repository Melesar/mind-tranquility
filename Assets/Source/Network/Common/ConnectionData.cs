using System;
using System.IO;
using System.Net;
using UnityEngine;

namespace Common
{
    public struct ConnectionData
    {
        public Color[] colors;
        public IPEndPoint address;

        public void Deserialize(BinaryReader reader)
        {
            try
            {
                var count = reader.ReadInt32();
                colors = new Color[count];
                for (int i = 0; i < count; i++)
                {
                    var r = (float) reader.ReadByte() / 255;
                    var g = (float) reader.ReadByte() / 255;
                    var b = (float) reader.ReadByte() / 255;
                    var a = (float) reader.ReadByte() / 255;

                    colors[i] = new Color(r, g, b, a);
                }
            }
            catch (IOException)
            {
                colors = Array.Empty<Color>();
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            if (colors == null || colors.Length == 0)
            {
                return;
            }

            writer.Write(colors.Length);
            foreach (var color in colors)
            {
                writer.Write((byte)(color.r * 255));
                writer.Write((byte)(color.g * 255));
                writer.Write((byte)(color.b * 255));
                writer.Write((byte)(color.a * 255));
            }
        }
    }
}