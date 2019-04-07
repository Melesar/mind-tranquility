using UnityEngine.EventSystems;

public interface ISessionListener : IEventSystemHandler
{
    void OnSessionStart();
    void OnSessionEnd();
}