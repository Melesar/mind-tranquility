using System;
using UnityEngine;

public class MindReaderProxy : MonoBehaviour
{
    public event Action<int> meditationChanged;
    public event Action<int> focusChanged;
    public event Action<int> signalStrengthChanged; 
    
    void receivePoorSignal(string value)
    {
        signalStrengthChanged?.Invoke(int.Parse(value));
    }
    
    void receiveMeditation(string value)
    {
        meditationChanged?.Invoke(int.Parse(value));
    }
    
    void receiveAttention(string value)
    {
        focusChanged?.Invoke(int.Parse(value));
    }
}