using Module.Core;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PauseUI : MonoBehaviour
{
    [SerializeField] private GameObject _pauseUI;
    [SerializeField] private Button _pauseButton;
    [SerializeField] private Button _exitButton;

    private IAppFlowService _appFlowService;
    private bool _pauseIsActive;

    [Inject]
    public void Construct(IAppFlowService appFlow) => _appFlowService = appFlow;

    private void Awake()
    {
        _pauseIsActive = false;
        _pauseUI.SetActive(_pauseIsActive);
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
    }

    private void Unpause() => Pause(false);

    private void ExitToMenu()
    {
        Time.timeScale = 1f;
        _appFlowService.LoadMenu();
    }
}