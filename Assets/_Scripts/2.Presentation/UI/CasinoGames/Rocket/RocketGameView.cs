using Module.Core;
using Module.Gameplay;
using Module.Presentation.Visual;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Module.Presentation.Games.Rocket
{
    public sealed class RocketGameView : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject _betWindow;
        [SerializeField] private Button _startButton;
        [SerializeField] private TextMeshProUGUI _multiplierText;
        [SerializeField] private TextMeshProUGUI _betText;
        [SerializeField] private Button _increaseBetButton;
        [SerializeField] private Button _decreaseBetButton;
        [SerializeField] private Button _confirmBetButton;
        [SerializeField] private Button _cancelBetButton;
        [SerializeField] private Button _maxBetButton;
        [SerializeField] private Button _minBetButton;

        [Header("References")]
        [SerializeField] private MessageManager _messageManager;
        [SerializeField] private Animator _rocketAnimator;
        [SerializeField] private RocketBackgroundView _backgroundView;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _explosionSound;
        [SerializeField] private AudioClip _youWonSound;

        private IDataService _dataService;

        private RocketGameService _game;

        private bool _isIncreasing;
        private bool _isDecreasing;

        private const float HoldDelay = 0.1f;
        private const float MinBalanceForLossMessage = 10f;

        private float CurrentBalance => _dataService.SaveData.balance;

        [Inject]
        public void Construct(IDataService dataService, RocketGameService game)
        {
            _dataService = dataService;
            _game = game;
        }

        private void Awake()
        {
            _game.OnMultiplierChanged += HandleMultiplierChanged;
            _game.OnRoundStarted += HandleRoundStarted;
            _game.OnRoundEnded += HandleRoundEnded;
            _game.OnClaimedWinnings += HandleClaimed;

            BindUI();
        }

        private void OnDestroy()
        {
            if (_game == null) return;

            _game.OnMultiplierChanged -= HandleMultiplierChanged;
            _game.OnRoundStarted -= HandleRoundStarted;
            _game.OnRoundEnded -= HandleRoundEnded;
            _game.OnClaimedWinnings -= HandleClaimed;
        }

        private void Start()
        {
            _betWindow.SetActive(false);

            _game.SetBet(RocketGameConfig.minBet);
            RefreshBetText();

            InvokeRepeating(nameof(UpdateBetHold), HoldDelay, HoldDelay);

            UpdateUIFromState();
            HandleMultiplierChanged(_game.State.CurrentMultiplier);
        }

        #region UI Binding
        private void BindUI()
        {
            _startButton.onClick.AddListener(OnStartButtonClick);
            _confirmBetButton.onClick.AddListener(OnConfirmBetClick);
            _cancelBetButton.onClick.AddListener(() => _betWindow.SetActive(false));

            AddHoldListener(_increaseBetButton, () => _isIncreasing = true, () => _isIncreasing = false);
            AddHoldListener(_decreaseBetButton, () => _isDecreasing = true, () => _isDecreasing = false);

            _maxBetButton.onClick.AddListener(SetMaxBet);
            _minBetButton.onClick.AddListener(SetminBet);
        }

        private void AddHoldListener(Button button, System.Action onPress, System.Action onRelease)
        {
            var trigger = button.gameObject.AddComponent<EventTrigger>();
            AddEvent(trigger, EventTriggerType.PointerDown, onPress);
            AddEvent(trigger, EventTriggerType.PointerUp, onRelease);
        }

        private static void AddEvent(EventTrigger trigger, EventTriggerType type, System.Action callback)
        {
            EventTrigger.Entry entry = new() { eventID = type };
            entry.callback.AddListener(_ => callback());
            trigger.triggers.Add(entry);
        }
        #endregion

        #region Betting UI
        private void UpdateBetHold()
        {
            if (_isIncreasing) ChangeBet(+RocketGameConfig.betStep);
            if (_isDecreasing) ChangeBet(-RocketGameConfig.betStep);
        }

        private void ChangeBet(float delta)
        {
            if (_game.State.Phase == ERocketRoundPhase.Running) return;

            float max = Mathf.Floor(CurrentBalance);
            float newBet = Mathf.Clamp(_game.State.CurrentBet + delta, RocketGameConfig.minBet, max);

            newBet = Mathf.Floor(newBet / RocketGameConfig.betStep) * RocketGameConfig.betStep;

            _game.SetBet(newBet);
            RefreshBetText();
            UpdateUIFromState();
        }

        private void SetMaxBet()
        {
            float max = Mathf.Floor(CurrentBalance / RocketGameConfig.betStep) * RocketGameConfig.betStep;
            _game.SetBet(max);
            RefreshBetText();
            UpdateUIFromState();
        }

        private void SetminBet()
        {
            _game.SetBet(RocketGameConfig.minBet);
            RefreshBetText();
            UpdateUIFromState();
        }

        private void OnConfirmBetClick()
        {
            float bet = _game.State.CurrentBet;
            if (bet < RocketGameConfig.minBet || bet > CurrentBalance)
            {
                Debug.LogWarning($"Некорректная ставка. Ставка должна быть в пределах от {RocketGameConfig.minBet} до {CurrentBalance}.");
                return;
            }

            _game.ConfirmBet(CurrentBalance);
            _betWindow.SetActive(false);

            UpdateUIFromState();
        }

        private void RefreshBetText()
        {
            if (_betText != null)
                _betText.text = $"{_game.State.CurrentBet:F0}";
        }
        #endregion

        #region Game Flow UI
        private void OnStartButtonClick()
        {
            if (_game.State.Phase != ERocketRoundPhase.Running)
            {
                if (!_game.State.BetConfirmed)
                {
                    OpenBetWindow();
                    return;
                }

                if (_game.CanStartRound(CurrentBalance))
                {
                    _dataService.ChangeBalance(-_game.State.CurrentBet);
                    _game.StartRound();
                }
            }
            else
            {
                if (_game.CanClaim())
                {
                    float winnings = _game.Claim();
                    if (winnings > 0f)
                    {
                        _dataService.ChangeBalance(winnings);
                    }
                }
            }

            UpdateUIFromState();
        }

        private void OpenBetWindow()
        {
            RefreshBetText();
            _betWindow.SetActive(true);
        }

        private void HandleRoundStarted()
        {
            _rocketAnimator.Play("Fly");
            _backgroundView.StartScrolling();

            if (_multiplierText != null)
                _multiplierText.color = Color.white;

            _startButton.interactable = true;
        }

        private void HandleClaimed(float winnings)
        {
            PlaySound(_youWonSound, 1f);

            if (winnings > _game.State.CurrentBet * RocketGameConfig.bigWinMultiplier)
                _messageManager.AddBigWinMessage();

            _startButton.interactable = false;
        }

        private void HandleRoundEnded(float lastMultiplier)
        {
            _rocketAnimator.Play("GameOver");
            _backgroundView.StopScrolling();
            PlaySound(_explosionSound, 0.7f);

            if (!_game.State.HasClaimed && CurrentBalance < MinBalanceForLossMessage)
                _messageManager.AddBigLossMessage();

            if (_multiplierText != null)
            {
                _multiplierText.text = $"{lastMultiplier:F2}x";
                _multiplierText.color = Color.red;
            }

            _startButton.interactable = true;
        }
        #endregion

        private void HandleMultiplierChanged(float multiplier)
        {
            if (_multiplierText != null)
                _multiplierText.text = $"{multiplier:F2}x";

            _backgroundView.SetMultiplier(multiplier);
        }

        private void UpdateUIFromState()
        {
            _startButton.interactable = CurrentBalance >= RocketGameConfig.minBet;

            if (_game.State.Phase != ERocketRoundPhase.Running)
            {
                if (_multiplierText != null)
                    _multiplierText.color = Color.red;
            }
        }

        private void PlaySound(AudioClip clip, float volume = 0.7f)
        {
            if (clip == null) return;
            _audioSource.PlayOneShot(clip, volume);
        }
    }
}
