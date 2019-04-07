using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using File = System.IO.File;
using Object = UnityEngine.Object;

namespace Common
{
    [MessageHeader(Id = 5)]
    public class TextureMessage : IMessage, IDisposable
    {
        public Texture2D Texture
        {
            get 
            {
                if (_texture == null)
                {
                    _texture = new Texture2D(1, 1);
                    _texture.LoadImage(_textureData);
                }

                return _texture;
            }
        }

        private static bool _fileWritten;


        private byte[] _textureData;
        private Texture2D _texture;
        private Vector2Int _textureSize;

        public TextureMessage(Texture2D texture)
        {
            _textureSize = new Vector2Int(texture.width, texture.height);
            _textureData = texture.EncodeToJPG();
        }

        public TextureMessage(Color32[] colorBuffer, Vector2Int textureSize)
        {
            _textureSize = textureSize;
            _textureData = ColorsToBytes(colorBuffer);
        }

        private byte[] ColorsToBytes(Color32[] colorBuffer)
        {
            var map = new Dictionary<byte, List<int>>();
            for (int i = 0; i < colorBuffer.Length; i++)
            {
                var clientIndex = colorBuffer[i].r;
                if (clientIndex > 0)
                {
                    Add(map, clientIndex, i);
                }
            }
            
            return Serialization.ToBinary(writer =>
            {
                writer.Write(_textureSize.x);
                writer.Write(_textureSize.y);
                foreach (var entry in map)
                {
                    writer.Write(entry.Key);
                    foreach (var index in entry.Value)
                    {
                        writer.Write(index);
                    }

                    writer.Write(-1);
                }
            });
        }

        private Color32[] BytesToColors(byte[] bytes)
        {
            var map = new Dictionary<byte, List<int>>();
            Serialization.FromBinary(bytes, reader =>
            {
                _textureSize.x = reader.ReadInt32();
                _textureSize.y = reader.ReadInt32();
                
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    var clientIndex = reader.ReadByte();
                    var index = reader.ReadInt32();
                    while(index > -1)
                    {
                        Add(map, clientIndex, index);
                        index = reader.ReadInt32();
                    } 
                }
            });

            var colorBuffer = new Color32[_textureSize.x * _textureSize.y];
            foreach (var entry in map)
            {
                foreach (var index in entry.Value)
                {
                    colorBuffer[index] = new Color32(entry.Key, 0, 0, 1);
                }
            }

            return colorBuffer;
        }

        private void Add(Dictionary<byte, List<int>> map, byte clientIndex, int arrayIndex)
        {
            List<int> list;
            if (map.TryGetValue(clientIndex, out list))
            {
                list.Add(arrayIndex);
            }
            else
            {
                map.Add(clientIndex, new List<int> {arrayIndex});
            }
        }

        public TextureMessage()
        {
            
        }

        public byte[] ToBinary()
        {
            return _textureData;
        }

        public void FromBinary(byte[] bytes)
        {
            _textureData = bytes;
        }

        public void Dispose()
        {
            if (_texture != null)
            {
                Object.Destroy(_texture);
            }
        }
    }
}