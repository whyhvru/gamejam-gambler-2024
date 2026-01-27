using System.Collections;
using Module.Core;
using Module.Gameplay;
using Module.Presentation.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Module.Presentation.UI
{
    public sealed class SlotMachineView : MonoBehaviour
    {
        [Header("Reels")]
        [SerializeField] private SlotReelView _reel1;
        [SerializeField] private SlotReelView _reel2;
        [SerializeField] private SlotReelView _reel3;
        [SerializeField] private float _reel1StopAfter = 2.0f;
        [SerializeField] private float _reel2StopAfter = 2.6f;
        [SerializeField] private float _reel3StopAfter = 3.2f;

        [Header("UI Elements")]
        [SerializeField] private Button _startButton;
        [SerializeField] private GameObject _betWindow;
        [SerializeField] private TextMeshProUGUI _betText;
        [SerializeField] private Button _increaseBetButton;
        [SerializeField] private Button _decreaseBetButton;
        [SerializeField] private Button _confirmBetButton;
        [SerializeField] private Button _cancelBetButton;
        [SerializeField] private Button _maxBetButton;
        [SerializeField] private Button _minBetButton;

        [Header("Dependencies")]
        [SerializeField] private SlotMachineAudioView _audioView;

        private IDataService _dataService;
        private MessageService _messageService;
        private SlotMachineService _game;

        private float _betChangeDelta;

        private const float HoldDelay = 0.1f;

        private float CurrentBalance => _dataService.SaveData.balance;

        [Inject]
        public void Construct(IDataService dataService, SlotMachineService game, MessageService message)
        {
            _dataService = dataService;
            _game = game;
            _messageService = message;
        }

        private void Awake()
        {
            _game.OnStateChanged += UpdateUIFromState;
            BindUI();
        }

        private void OnDestroy()
        {
            if (_game != null)
                _game.OnStateChanged -= UpdateUIFromState;
        }

        private void OnEnable()
        {
            _game.SetBet(Mathf.Max(_game.State.CurrentBet, SlotMachineConfig.MinBet));
            RefreshBetText();
            UpdateUIFromState();
        }

        private void Start()
        {
            _betWindow.SetActive(false);
            InvokeRepeating(nameof(UpdateBetHold), HoldDelay, HoldDelay);

            RefreshBetText();
            UpdateUIFromState();
        }

        #region UI binding
        private void BindUI()
        {
            _startButton.onClick.AddListener(OnStartButtonClick);
            _confirmBetButton.onClick.AddListener(OnConfirmBetClick);
            _cancelBetButton.onClick.AddListener(() => _betWindow.SetActive(false));

            AddHoldListener(_increaseBetButton, () => _betChangeDelta = +SlotMachineConfig.BetStep, () => _betChangeDelta = 0f);
            AddHoldListener(_decreaseBetButton, () => _betChangeDelta = -SlotMachineConfig.BetStep, () => _betChangeDelta = 0f);

            _maxBetButton.onClick.AddListener(SetMaxBet);
            _minBetButton.onClick.AddListener(SetMinBet);
        }

        private void AddHoldListener(Button button, System.Action onPress, System.Action onRelease)
        {
            var trigger = button.gameObject.AddComponent<EventTrigger>();
            trigger.triggers.Add(CreateEvent(EventTriggerType.PointerDown, _ => onPress()));
            trigger.triggers.Add(CreateEvent(EventTriggerType.PointerUp, _ => onRelease()));
        }

        private static EventTrigger.Entry CreateEvent(EventTriggerType type, UnityEngine.Events.UnityAction<BaseEventData> callback)
        {
            EventTrigger.Entry entry = new()
            {
                eventID = type,
                callback = new EventTrigger.TriggerEvent()
            };
            entry.callback.AddListener(callback);
            return entry;
        }
        #endregion

        private void UpdateBetHold()
        {
            if (_betChangeDelta == 0f) return;
            ChangeBet(_betChangeDelta);
        }

        private void ChangeBet(float delta)
        {
            if (_game.State.IsSpinning) return;

            float max = Mathf.Floor(CurrentBalance);
            float bet = Mathf.Clamp(_game.State.CurrentBet + delta, SlotMachineConfig.MinBet, max);
            bet = Mathf.Floor(bet / SlotMachineConfig.BetStep) * SlotMachineConfig.BetStep;

            _game.SetBet(bet);
            RefreshBetText();
        }

        private void SetMaxBet()
        {
            float max = Mathf.Floor(CurrentBalance / SlotMachineConfig.BetStep) * SlotMachineConfig.BetStep;
            _game.SetBet(max);
            RefreshBetText();
        }

        private void SetMinBet()
        {
            _game.SetBet(SlotMachineConfig.MinBet);
            RefreshBetText();
        }

        private void OnConfirmBetClick()
        {
            if (!_game.CanConfirmBet(CurrentBalance))
            {
                Debug.LogWarning($"Некорректная ставка. Ставка должна быть от {SlotMachineConfig.MinBet} до {CurrentBalance}.");
                return;
            }

            _game.ConfirmBet(CurrentBalance);
            _betWindow.SetActive(false);
        }

        private void OnStartButtonClick()
        {
            if (_game.State.IsSpinning) return;

            if (!_game.State.BetConfirmed)
            {
                OpenBetWindow();
                return;
            }

            if (!_game.CanStartSpin(CurrentBalance))
                return;

            // списываем ставку
            _dataService.ChangeBalance(-_game.State.CurrentBet);

            _startButton.interactable = false;
            _game.StartSpin();

            _audioView.PlaySpinSound();
            StartCoroutine(SpinRoutine());
        }

        private IEnumerator SpinRoutine()
        {
            int i1 = -1, i2 = -1, i3 = -1;

            bool d1 = false, d2 = false, d3 = false;

            _reel1.Spin(this, delay: 0f, stopAfter: _reel1StopAfter, idx => { i1 = idx; d1 = true; });
            _reel2.Spin(this, delay: 0f, stopAfter: _reel2StopAfter, idx => { i2 = idx; d2 = true; });
            _reel3.Spin(this, delay: 0f, stopAfter: _reel3StopAfter, idx => { i3 = idx; d3 = true; });

            while (!d1 || !d2 || !d3)
                yield return null;

            int multiplier = _game.FinishSpin(i1, i2, i3);

            if (multiplier > 0)
            {
                float win = _game.State.CurrentBet * multiplier;
                _dataService.ChangeBalance(win);
                _audioView.PlayWinByMultiplier(multiplier);

                if (multiplier >= 10) _messageService.AddBigWinMessage();
            }
            else
            {
                if (CurrentBalance < SlotMachineConfig.MinBet)
                    _messageService.AddBigLossMessage();
            }

            UpdateUIFromState();
        }

        private void OpenBetWindow()
        {
            RefreshBetText();
            _betWindow.SetActive(true);
        }

        private void RefreshBetText()
        {
            if (_betText != null)
                _betText.text = $"{_game.State.CurrentBet:F0}";
        }

        private void UpdateUIFromState()
            => _startButton.interactable = CurrentBalance >= SlotMachineConfig.MinBet && !_game.State.IsSpinning;
    }
}