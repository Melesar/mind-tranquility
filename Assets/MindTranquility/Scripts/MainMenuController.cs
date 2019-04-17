using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField]
    private ConnectionPopup _popupPrefab;
    
    public void OnNewGameClicked()
    {
        SpawnPopup().SetHost();
    }

    public void OnConnectClicked()
    {
        SpawnPopup().SetClient();
    }

    private ConnectionPopup SpawnPopup()
    {
        return Instantiate(_popupPrefab);
    }
}
