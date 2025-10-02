using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    private readonly string _menuScene = "Menu";
    private readonly string _gameScene = "Game";
    private readonly string _tenetScene = "Tenet";
    private readonly string _endScene = "End";

    private string _currentScene;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            _currentScene = _menuScene;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void StartScene(string sceneName)
    {
        _currentScene = sceneName;
        SceneManager.LoadScene(_currentScene);
    }

    public void LoadMenu() => StartScene(_menuScene);
    public void LoadGame() => StartScene(_gameScene);
    public void LoadTenet() => StartScene(_tenetScene);
    public void LoadEnd() => StartScene(_endScene);
}