using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class MenuUI : MonoBehaviour
{
    [Header("Play")]
    [SerializeField] private Button _playButton;
    [SerializeField] private GameObject _playWindow;

    [Header("Continue")]
    [SerializeField] private Button _continueButton;

    [Header("Other")]
    [SerializeField] private Button _newGameButton;
    [SerializeField] private GameObject _newGameWindow;
    [SerializeField] private Button _confirmButton;
    [SerializeField] private Button _cancelButton;

    [Header("Exit")]
    [SerializeField] private Button _exitButton;
    private string _saveFilePath;

    private void Start()
    {
        _saveFilePath = Path.Combine(Application.persistentDataPath, "SaveData.json");
        AddButtonListener(_playButton, Play);
        AddButtonListener(_exitButton, Quit);
    }

    private void AddButtonListener(Button button, UnityEngine.Events.UnityAction action)
    {
        if (button != null)
        {
            button.onClick.AddListener(action);
        }
    }

    private void Play()
    {
        _playWindow.SetActive(true);
        AddButtonListener(_continueButton, Continue);
        AddButtonListener(_newGameButton, NewGameWindow);
    }

    private void Continue() => SceneLoader.Instance.LoadGame();

    private void NewGameWindow()
    {
        _newGameWindow.SetActive(true);
        _playWindow.SetActive(false);
        AddButtonListener(_confirmButton, NewGame);
        AddButtonListener(_cancelButton, CancelNewGame);
    }

    private void CancelNewGame()
    {
        _playWindow.SetActive(true);
        _newGameWindow.SetActive(false);
    }

    private void Quit() => Application.Quit();

    private void NewGame()
    {
        PlayerPrefs.DeleteAll();

        if (File.Exists(_saveFilePath))
            File.Delete(_saveFilePath);

        DataManager.Instance.Load();
        DataManager.Instance.Save();

        Continue();
    }
}