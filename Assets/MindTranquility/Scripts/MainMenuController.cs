using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField]
    private BlurOptimized _blur;
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
        var enableBlur = new BlurTask(this, _blur);
        var popup = Instantiate(_popupPrefab);
//        popup.Init(enableBlur);

        return popup;
    }
}
