using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    [SerializeField] private GameObject _pauseUI;
    [SerializeField] private Button _pauseButton;
    [SerializeField] private Button _exitButton;
    private AudioSource _audioSource;
    private bool _pauseIsActive;

    private void Awake() 
    {
        _pauseIsActive = false;
        _pauseUI.SetActive(_pauseIsActive);
        _audioSource = MusicManager.Instance.GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause(!_pauseIsActive);
        }
    }

    private void Pause(bool isActive)
    {
        _pauseUI.SetActive(isActive);

        if (isActive)
        {
            _pauseButton.onClick.RemoveAllListeners();
            _exitButton.onClick.RemoveAllListeners();
            _pauseButton.onClick.AddListener(Unpause);
            _exitButton.onClick.AddListener(ExitToMenu);
        }
        
        Time.timeScale = isActive ? 0f : 1f;
        _pauseIsActive = isActive;

        if (isActive)
        {
            _audioSource.volume = SettingsManager.Instance.Settings.Volume / 3;
        }
        else
        {
            _audioSource.volume = SettingsManager.Instance.Settings.Volume;
        }
    }

    private void Unpause()
    {
        Pause(false);
    }

    private void ExitToMenu()
    {
        Time.timeScale = 1f;
        SceneLoader.Instance.LoadMenu();
    }
}