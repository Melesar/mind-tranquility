using System;
using System.Collections;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Framework.References;
using Jayrock.Json.Conversion;
using UnityEngine;

public class MindReader : IMindReader
{
    public virtual FloatReference Meditation => _meditation;
    public virtual FloatReference Focus => _focus;

    public virtual SignalStrength SignalStrength => _signalStrengthLevel;

    private SignalStrength _signalStrengthLevel;

    private readonly FloatReference _meditation;
    private readonly FloatReference _focus;
    private readonly GameObject _mindReaderProxy;

    public MindReader()
    {
        _meditation = new FloatReference {useConstantValue = true};
        _focus = new FloatReference {useConstantValue = true};
        
        UnityThinkGear.Init(true);
        UnityThinkGear.StartStream();

        _mindReaderProxy = SetupProxy();
    }

    private GameObject SetupProxy()
    {
        var proxyObj = new GameObject("ThinkGear");
        var proxy = proxyObj.AddComponent<MindReaderProxy>();
        proxy.meditationChanged += SetMeditation;
        proxy.signalStrengthChanged += SetSignalStrength;

        return proxyObj;
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
    
    public virtual void Dispose()
    {
        UnityEngine.Object.Destroy(_mindReaderProxy);
        
        UnityThinkGear.StopStream();
        UnityThinkGear.Close();
    }
}