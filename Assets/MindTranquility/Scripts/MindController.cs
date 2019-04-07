using UnityEngine;
using UnityEngine.Serialization;

public class MindController : MonoBehaviour, ISessionListener
{
    [SerializeField]
    private MindReaderAsset _mindReader;
    [SerializeField]
    private SynchronizationData _synchronizationData;
    [SerializeField]
    private TaskHandle _gameLoadedHandle;
    [SerializeField]
    private float _updateInterval = 0.1f;

    private void OnMeditationChanged(float oldValue, float newValue)
    {
        _synchronizationData.myMeditation.Value = newValue;
    }
    
    private void Start()
    {
        _mindReader.Init(new FakeCoroutineMindReader(this, _updateInterval));
        _gameLoadedHandle.AddTask(
            new ActionTask(this, SubscribeToMeditation),
            TaskHandle.PRIORITY_UI_EFFECTS
            );
    }

    private void SubscribeToMeditation()
    {
        _mindReader.Meditation.valueChanged += OnMeditationChanged;
    }

    private void OnDestroy()
    {
        _mindReader.Dispose();
    }

    public void OnSessionStart()
    {
    }

    public void OnSessionEnd()
    {
        _mindReader.Meditation.valueChanged -= OnMeditationChanged;
    }
}