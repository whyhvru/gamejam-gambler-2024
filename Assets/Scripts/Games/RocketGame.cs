using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class RocketGame : MonoBehaviour
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

    [Header("Game Settings")]
    [SerializeField] private float _minBet = 50f;
    [SerializeField] private float _maxMultiplier = 25f;
    [SerializeField] private float _alpha = 0.1f;

    [Header("References")]
    [SerializeField] private MessageManager _messageManager;
    [SerializeField] private Animator _rocketAnimator;
    [SerializeField] private RocketBackground _rocketBackground;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _explosionSound;
    [SerializeField] private AudioClip _youWonSound;

    // Internal state
    private float _currentBet;
    private float _lastMultiplier;
    private bool _isGameRunning;
    private bool _hasClaimed;
    private bool _isBetConfirmed;

    private float _explodeTime;
    private float _elapsedTime;

    // Hold button control
    private bool _isIncreasing;
    private bool _isDecreasing;

    // Constants
    private const float BetChangeRate = 50f;
    private const float HoldDelay = 0.1f;
    private const float BigWinMultiplier = 10f;
    private const float MinBalanceForLossMessage = 10f;

    public float CurrentMultiplier { get; private set; } = 1f;
    private float CurrentBalance => DataManager.Instance.SaveData.balance;

    private void OnEnable()
    {
        UpdateUI();
        if (_isGameRunning)
        {
            _rocketAnimator.Play("Fly");
        }
    }

    private void Start()
    {
        BindUI();
        _betWindow.SetActive(false);
        InvokeRepeating(nameof(UpdateBet), HoldDelay, HoldDelay);
    }

    private void Update()
    {
        if (!_isGameRunning) return;

        _elapsedTime += Time.deltaTime;
        UpdateMultiplier();

        if (_elapsedTime >= _explodeTime)
        {
            EndGame();
        }
    }

    #region UI Binding
    private void BindUI()
    {
        _startButton.onClick.AddListener(OnStartButtonClick);
        _confirmBetButton.onClick.AddListener(ConfirmBet);
        _cancelBetButton.onClick.AddListener(() => _betWindow.SetActive(false));

        AddHoldListener(_increaseBetButton, () => _isIncreasing = true, () => _isIncreasing = false);
        AddHoldListener(_decreaseBetButton, () => _isDecreasing = true, () => _isDecreasing = false);

        _maxBetButton.onClick.AddListener(SetMaxBet);
        _minBetButton.onClick.AddListener(SetMinBet);
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

    #region Betting
    private void UpdateBet()
    {
        if (_isIncreasing) ChangeBet(BetChangeRate);
        if (_isDecreasing) ChangeBet(-BetChangeRate);
    }

    private void ChangeBet(float delta)
    {
        float newBet = Mathf.Clamp(_currentBet + delta, _minBet, Mathf.Floor(CurrentBalance));
        _currentBet = Mathf.Floor(newBet / BetChangeRate) * BetChangeRate;
        _betText.text = $"{_currentBet:F0}";
    }

    private void SetMaxBet()
    {
        _currentBet = Mathf.Floor(CurrentBalance / BetChangeRate) * BetChangeRate;
        _betText.text = $"{_currentBet:F0}";
    }

    private void SetMinBet()
    {
        _currentBet = _minBet;
        _betText.text = $"{_currentBet:F0}";
    }

    private void ConfirmBet()
    {
        if (_currentBet >= _minBet && _currentBet <= CurrentBalance)
        {
            _isBetConfirmed = true;
            _betWindow.SetActive(false);
        }
        else
        {
            Debug.LogWarning($"Некорректная ставка. Ставка должна быть в пределах от {_minBet} до {CurrentBalance}.");
        }
    }
    #endregion

    #region Game Flow
    private void OnStartButtonClick()
    {
        if (!_isGameRunning)
        {
            if (!_isBetConfirmed)
                OpenBetWindow();
            else
                StartGame();
        }
        else if (!_hasClaimed)
        {
            ClaimWinnings();
        }
    }

    private void OpenBetWindow()
    {
        _betText.text = $"{_currentBet:F2}";
        _betWindow.SetActive(true);
    }

    private void StartGame()
    {
        UpdateBalance(-_currentBet);

        _rocketAnimator.Play("Fly");
        _rocketBackground.StartScrolling();

        _isGameRunning = true;
        _hasClaimed = false;
        _elapsedTime = 0f;

        _explodeTime = CalculateExplosionTime();

        _multiplierText.color = Color.white;
    }

    private float CalculateExplosionTime()
    {
        float maxTime = Mathf.Log(_maxMultiplier + 1, Mathf.Exp(1)) / _alpha;
        float u = Random.value;
        const float k = 2f;
        return 1f + ((maxTime - 1f) * Mathf.Pow(u, k));
    }

    private void ClaimWinnings()
    {
        if (!_isGameRunning || _hasClaimed) return;

        _hasClaimed = true;
        PlayYouWonSound();

        float winnings = _currentBet * CurrentMultiplier;
        UpdateBalance(winnings);

        if (winnings > _currentBet * BigWinMultiplier)
        {
            _messageManager.AddBigWinMessage();
        }

        _startButton.interactable = false;
    }

    private void EndGame()
    {
        _rocketAnimator.Play("GameOver");
        _rocketBackground.StopScrolling();

        PlayExplosionSound();

        if (!_hasClaimed && CurrentBalance < MinBalanceForLossMessage)
        {
            _messageManager.AddBigLossMessage();
        }

        _isGameRunning = false;
        _isBetConfirmed = false;

        _lastMultiplier = CurrentMultiplier;
        _multiplierText.text = $"{_lastMultiplier:F2}x";
        _multiplierText.color = Color.red;

        _startButton.interactable = true;
        UpdateUI();
    }
    #endregion

    #region Helpers
    private void UpdateMultiplier()
    {
        CurrentMultiplier = 1.0f + Mathf.Exp(_alpha * _elapsedTime) - 1.0f;
        _multiplierText.text = $"{CurrentMultiplier:F2}x";
    }

    private void UpdateUI()
    {
        _startButton.interactable = CurrentBalance >= _minBet;

        if (!_isGameRunning)
        {
            _multiplierText.text = $"{_lastMultiplier:F2}x";
            _multiplierText.color = Color.red;
        }
    }

    private void UpdateBalance(float value) => DataManager.Instance.ChangeBalance(value);
    public void PlayExplosionSound() => PlaySound(_explosionSound);
    public void PlayYouWonSound() => PlaySound(_youWonSound, 1f);

    private void PlaySound(AudioClip clip, float volumeMultiplier = 0.7f)
    {
        if (clip == null) return;
        float volume = SettingsManager.Instance.Settings.volume;
        _audioSource.PlayOneShot(clip, volume * volumeMultiplier);
    }
    #endregion
}
