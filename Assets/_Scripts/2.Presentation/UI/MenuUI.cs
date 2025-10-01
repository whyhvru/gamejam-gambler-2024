using UnityEngine;
using UnityEngine.UI;
using Module.Core;

namespace Module.Presentation.UI
{
    public class MenuUI : MonoBehaviour
    {
        [SerializeField] private GameObject _startGameWindow;
        [SerializeField] private GameObject _newGameWindow;

        [Header("Main Menu")]
        [SerializeField] private Button _startGameButton;
        [SerializeField] private Button _quitGameButton;

        [Header("Play Window")]
        [SerializeField] private Button _continueGameButton;
        [SerializeField] private Button _newGameButton;

        [Header("New Game Window")]
        [SerializeField] private Button _confirmNewGameButton;
        [SerializeField] private Button _cancelNewGameButton;

        private IGameFlowService _gameFlowService;

        private void Awake()
        {
            _gameFlowService = ServiceRegistry.Get<IGameFlowService>();

            _startGameButton.onClick.AddListener(() => _startGameWindow.SetActive(true));
            _quitGameButton.onClick.AddListener(_gameFlowService.QuitGame);

            _continueGameButton.onClick.AddListener(_gameFlowService.LoadGame);
            _newGameButton.onClick.AddListener(() => _newGameWindow.SetActive(true));

            _confirmNewGameButton.onClick.AddListener(_gameFlowService.LoadNewGame);
            _cancelNewGameButton.onClick.AddListener(() => _newGameWindow.SetActive(false));
        }
    }
}