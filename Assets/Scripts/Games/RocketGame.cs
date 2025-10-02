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

    [Header("Other")]
    [SerializeField] private MessageManager _messageManager;
    [SerializeField] private Animator _rocketAnimator;
    [SerializeField] private RocketBackground _rocketBackground;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _explosionSound;
    [SerializeField] private AudioClip _youWonSound;
    private float _currentBet;
    private float _currentBalance;
    private float _currentMultiplier = 1f;
    private float _lastMultiplier = 0f;
    private bool _isGameRunning = false;
    private bool _hasClaimed = false;
    private float _explodeTime;
    private float _elapsedTime;
    private bool _isBetConfirmed = false;

    private bool _isIncreasing = false;
    private bool _isDecreasing = false;

    private float _changeRate = 50f;
    private float _holdDelay = 0.1f;

    public float CurrentMultiplier => _currentMultiplier;

    private void OnEnable()
    {
        _currentBalance = DataManager.Instance.SaveData.Balance;
        UpdateUI();
        
        if (_isGameRunning)
        {
            _rocketAnimator.Play("Fly");
        }
    }

    private void Start()
    {
        _currentBalance = DataManager.Instance.SaveData.Balance;
        UpdateUI();

        _startButton.onClick.AddListener(OnStartButtonClick);
        _confirmBetButton.onClick.AddListener(ConfirmBet);
        _cancelBetButton.onClick.AddListener(CancelBet);

        AddHoldListener(_increaseBetButton, () => _isIncreasing = true, () => _isIncreasing = false);
        AddHoldListener(_decreaseBetButton, () => _isDecreasing = true, () => _isDecreasing = false);

        _maxBetButton.onClick.AddListener(SetMaxBet);
        _minBetButton.onClick.AddListener(SetMinBet);

        _betWindow.SetActive(false);

        InvokeRepeating(nameof(UpdateBet), _holdDelay, _holdDelay);
    }

    private void Update()
    {
        if (_isGameRunning)
        {
            _elapsedTime += Time.deltaTime;

            _currentMultiplier = 1.0f + Mathf.Exp(_alpha * _elapsedTime) - 1.0f;
            _multiplierText.text = $"{_currentMultiplier:F2}x";

            if (_elapsedTime >= _explodeTime)
            {
                EndGame();
            }
        }
    }

    private void UpdateBet()
    {
        if (_isIncreasing)
        {
            ChangeBet(_changeRate);
        }
        if (_isDecreasing)
        {
            ChangeBet(-_changeRate);
        }
    }

    private void AddHoldListener(Button button, System.Action onPress, System.Action onRelease)
    {
        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry pointerDown = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerDown
        };
        pointerDown.callback.AddListener(_ => onPress());
        trigger.triggers.Add(pointerDown);

        EventTrigger.Entry pointerUp = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerUp
        };
        pointerUp.callback.AddListener(_ => onRelease());
        trigger.triggers.Add(pointerUp);
    }

    private void OnStartButtonClick()
    {
        if (!_isGameRunning)
        {
            if (!_isBetConfirmed)
            {
                OpenBetWindow();
            }
            else
            {
                StartGame();
            }
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

    private void ConfirmBet()
    {
        if (_currentBet >= _minBet && _currentBet <= _currentBalance)
        {
            _isBetConfirmed = true;
            _betWindow.SetActive(false);
        }
        else
        {
            Debug.LogWarning($"Некорректная ставка. Ставка должна быть в пределах от {_minBet} до {_currentBalance}.");
        }
    }

    private void CancelBet()
    {
        _betWindow.SetActive(false);
    }

    private void ChangeBet(float delta)
    {
        float newBet = _currentBet + delta;

        newBet = Mathf.Clamp(newBet, _minBet, Mathf.Floor(_currentBalance));

        _currentBet = Mathf.Floor(newBet / _changeRate) * _changeRate;

        _betText.text = $"{_currentBet:F0}";
    }

    private void SetMaxBet()
    {
        _currentBet = Mathf.Floor(_currentBalance / _changeRate) * _changeRate;
        _betText.text = $"{_currentBet:F0}";
    }

    private void SetMinBet()
    {
        _currentBet = _minBet;
        _betText.text = $"{_currentBet:F0}";
    }

    private void StartGame()
    {
        UpdateBalance(-_currentBet);

        _rocketAnimator.Play("Fly");
        _rocketBackground.StartScrolling();

        _isGameRunning = true;
        _hasClaimed = false;
        _elapsedTime = 0f;

        float maxTime = Mathf.Log(_maxMultiplier + 1, Mathf.Exp(1)) / _alpha;
        float u = Random.value;
        float k = 2f;
        _explodeTime = 1f + (maxTime - 1f) * Mathf.Pow(u, k);

        _multiplierText.color = Color.white;
    }

    private void ClaimWinnings()
    {
        if (!_isGameRunning || _hasClaimed) return;

        _hasClaimed = true;

        PlayYouWonSound();

        float winnings = _currentBet * _currentMultiplier;
        UpdateBalance(winnings);

        if (winnings > _currentBet * 10)
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

        if (!_hasClaimed && _currentBalance < 10f)
        {
            _messageManager.AddBigLossMessage();
        }

        _isGameRunning = false;

        _lastMultiplier = _currentMultiplier;
        _isBetConfirmed = false;

        _multiplierText.text = $"{_lastMultiplier:F2}x";
        _multiplierText.color = Color.red;

        _startButton.interactable = true;

        UpdateUI();
    }

    private void UpdateUI()
    {
        _startButton.interactable = _currentBalance >= _minBet;

        if (!_isGameRunning)
        {
            _multiplierText.text = $"{_lastMultiplier:F2}x";
            _multiplierText.color = Color.red;
        }
    }

    private void UpdateBalance(float value)
    {
        DataManager.Instance.ChangeBalance(value);
        _currentBalance = DataManager.Instance.SaveData.Balance;
    }

    public void PlayExplosionSound()
    {
        PlaySound(_explosionSound);
    }

    public void PlayYouWonSound()
    {
        PlaySound(_youWonSound, 1f);
    }

    private void PlaySound(AudioClip clip, float volumeMultiplier = 0.7f)
    {
        if (clip != null)
        {
            float volume = SettingsManager.Instance.Settings.Volume; 
            _audioSource.PlayOneShot(clip, volume * volumeMultiplier);
        }
    }
}
