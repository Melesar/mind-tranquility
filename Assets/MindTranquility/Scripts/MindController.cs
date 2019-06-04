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

    private void OnMeditationChanged(float oldValue, float newValue)
    {
        _synchronizationData.myMeditation.Value = newValue;
    }

    private void SubscribeToMeditation()
    {
        _mindReader.Meditation.valueChanged += OnMeditationChanged;
    }

    private void OnDestroy()
    {
        _mindReader.Dispose();
    }

    private void Start()
    {
//        _mindReader.Init(new FakeCoroutineMindReader(this, 0.01f));
        _mindReader.Init(new MindReader());
    }

    public void OnSessionStart()
    {
        _gameLoadedHandle.AddTask(
            new ActionTask(SubscribeToMeditation),
            TaskHandle.PRIORITY_UI_EFFECTS
        );
    }

    public void OnSessionEnd()
    {
        _mindReader.Meditation.valueChanged -= OnMeditationChanged;
    }
}