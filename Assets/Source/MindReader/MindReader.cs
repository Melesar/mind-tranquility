using System;
using System.Collections;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Framework.References;
using Jayrock.Json.Conversion;
using UnityEngine;

public abstract class MindReader : IMindReader
{
    public virtual FloatReference Meditation => _meditation;
    public virtual FloatReference Focus => _focus;

    public virtual SignalStrength SignalStrength => _signalStrengthLevel;

    private SignalStrength _signalStrengthLevel;
    private TcpClient _client;
    private Stream _stream;
    private byte[] _buffer;

    private readonly FloatReference _meditation;
    private readonly FloatReference _focus;
    
    protected readonly float _updateInterval;

    protected MindReader(float updateInterval)
    {
        InitConnection();
        
        _updateInterval = updateInterval;
        _meditation = new FloatReference {useConstantValue = true};
        _focus = new FloatReference {useConstantValue = true};
    }

    protected void Update()
    {
        if (!_stream.CanRead)
        {
            return;
        }

        var bytesRead = _stream.Read(_buffer, 0, _buffer.Length);
        var packets = Encoding.ASCII.GetString(_buffer, 0, bytesRead).Split('\r');

        foreach (string packet in packets)
        {
            if (packet.Length == 0)
                continue;


            Debug.Log(packet);
            try
            {
                var primary = (IDictionary) JsonConvert.Import(typeof(IDictionary), packet);
                if (primary.Contains("poorSignalLevel"))
                {
                    SetSignalStrength(int.Parse(primary["poorSignalLevel"].ToString()));                    
                }
                
                if (primary.Contains("eSense"))
                {
                    var eSense = (IDictionary) primary["eSense"];
                    SetFocus(int.Parse(eSense["attention"].ToString()));
                    SetMeditation(int.Parse(eSense["meditation"].ToString()));
                }
            }
            catch (Exception)
            {
            }
        }
    }

    protected virtual void SetMeditation(int meditation)
    {
        _meditation.Value = meditation / 100f;
    }

    protected virtual void SetFocus(int focus)
    {
        _focus.Value = focus / 100f;
    }

    protected virtual void SetSignalStrength(int signalStrength)
    {
        if (signalStrength > 107)
        {
            _signalStrengthLevel = SignalStrength.NoSignal;
        }
        else if (signalStrength >= 78 && signalStrength < 107)
        {
            _signalStrengthLevel = SignalStrength.WeakSignal;
        }
        else if (signalStrength >= 51 && signalStrength < 78)
        {
            _signalStrengthLevel = SignalStrength.MediocreSignal;
        }
        else
        {
            _signalStrengthLevel = SignalStrength.GoodSignal;
        }
    }

    private void InitConnection()
    {
        _client = new TcpClient("127.0.0.1", 13854);
        _stream = _client.GetStream();
        _buffer = new byte[1024];
        byte[] myWriteBuffer = Encoding.ASCII.GetBytes(@"{""enableRawOutput"": true, ""format"": ""Json""}");
        _stream.Write(myWriteBuffer, 0, myWriteBuffer.Length);
    }
    
    public virtual void Dispose()
    {
        _client.Dispose();
        _stream.Close();
    }
}