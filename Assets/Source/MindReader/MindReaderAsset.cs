using System;
using Framework.References;
using UnityEngine;

[CreateAssetMenu(menuName = "Meditation/Mind reader")]
public class MindReaderAsset : ScriptableObject, IMindReader
{
    [SerializeField]
    private FloatReference _fakeMeditation;
    [SerializeField]
    private FloatReference _fakeFocus;
    
    private IMindReader _mindReader;

    public bool IsFake => _mindReader == null;
    
    public void Init(IMindReader mindReader)
    {
        _mindReader = mindReader;
    }

    private void OnEnable()
    {
        _fakeFocus.useConstantValue = true;
        _fakeFocus.useConstantValue = true;
    }

    public void Dispose()
    {
        _mindReader?.Dispose();
    }
    
    public FloatReference Meditation => _mindReader?.Meditation ?? _fakeMeditation;
    public FloatReference Focus => _mindReader?.Focus ?? _fakeFocus;
    public SignalStrength SignalStrength => _mindReader?.SignalStrength ?? SignalStrength.NoSignal;
}