using UnityEngine;
using UnityEngine.UI;

public class SignalStrengthView : MonoBehaviour
{
    [SerializeField]
    private MindReaderAsset _mindReader;
    [SerializeField]
    private Texture2D[] _textures;
    [SerializeField]
    private RawImage _image;
    
    private SignalStrength _currentSignalStrength;
    
    private void Update()
    {
        var strength = _mindReader.SignalStrength;
        if (strength != _currentSignalStrength)
        {
            _image.texture = _textures[(int) strength];
            _currentSignalStrength = strength;
        }
    }
}