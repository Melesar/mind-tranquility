using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class SceneController : MonoBehaviour, ISessionListener
{
    [FormerlySerializedAs("_connectionHandler")]
    [SerializeField]
    private ConnectionHandle _connectionHandle;
    [SerializeField]
    private TaskHandle _gameLoadedHandle;

    private const string MAIN_MENU_SCENE = "MainMenu";
    private const string GAME_SCENE = "GameScene";

    private void LoadGameScene()
    {
        var loadingOperation = SceneManager.LoadSceneAsync(GAME_SCENE, LoadSceneMode.Additive);
        loadingOperation.completed += OnGameSceneLoaded;
    }

    private void OnGameSceneLoaded(AsyncOperation op)
    {
        _connectionHandle.ExecuteTasks(() =>
        {
            var operation = SceneManager.UnloadSceneAsync(MAIN_MENU_SCENE);
            operation.completed += o =>
            {
                _gameLoadedHandle.ExecuteTasks();
            };
        });
    }

    private void LoadMainMenu()
    {
        SceneManager.LoadScene(MAIN_MENU_SCENE, LoadSceneMode.Additive);
    }

    private void BackToMenu()
    {
        var loadingOperation = SceneManager.LoadSceneAsync(MAIN_MENU_SCENE, LoadSceneMode.Additive);
        loadingOperation.completed += op => SceneManager.UnloadSceneAsync(GAME_SCENE);
    }

    private void Start()
    {
        _gameLoadedHandle.SetCoroutineHolder(this);
        
        LoadMainMenu();
    }

    private void OnDestroy()
    {
    }

    public void OnSessionStart()
    {
        LoadGameScene();
    }

    public void OnSessionEnd()
    {
        Invoke(nameof(BackToMenu), 3f);
    }
}