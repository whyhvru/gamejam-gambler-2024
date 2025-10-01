using UnityEngine;
using UnityEngine.UI;
using Module.Core;

namespace Module.Presentation.UI
{
    public class PauseUI : MonoBehaviour
    {
        private const string VolumeKey = nameof(VolumeKey);

        [SerializeField] private GameObject _pauseUI;
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _exitButton;

        private IGameFlowService _gameFlowService;
        private ISettingsService _settings;
        private bool _pauseIsActive;

        private void Awake()
        {
            _gameFlowService = ServiceRegistry.Get<IGameFlowService>();
            _settings = ServiceRegistry.Get<ISettingsService>();

            _pauseUI.SetActive(false);
            _pauseIsActive = false;

            _resumeButton.onClick.AddListener(Unpause);
            _exitButton.onClick.AddListener(ExitToMenu);
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
            _pauseIsActive = isActive;

            Time.timeScale = isActive ? 0f : 1f;

            _settings.Volume = isActive
                ? _settings.Volume * 0.33f
                : PlayerPrefs.GetFloat(VolumeKey, 1.0f);
        }

        private void Unpause()
        {
            Pause(false);
        }

        private void ExitToMenu()
        {
            Unpause();
            _gameFlowService.LoadMainMenu();
        }
    }
}