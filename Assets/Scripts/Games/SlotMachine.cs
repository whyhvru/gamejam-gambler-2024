using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.EventSystems;

public class SlotMachine : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private RectTransform slot1;
    [SerializeField] private RectTransform slot2;
    [SerializeField] private RectTransform slot3;
    [SerializeField] private Button _startButton;
    [SerializeField] private GameObject _betWindow;
    [SerializeField] private TextMeshProUGUI _betText;
    [SerializeField] private Button _increaseBetButton;
    [SerializeField] private Button _decreaseBetButton;
    [SerializeField] private Button _confirmBetButton;
    [SerializeField] private Button _cancelBetButton;
    [SerializeField] private Button _maxBetButton;
    [SerializeField] private Button _minBetButton;

    [Header("Slot Settings")]
    [SerializeField] private float _minBet = 50f;
    [SerializeField] private float acceleration = 0.1f;
    [SerializeField] private float deceleration = 0.2f;
    [SerializeField] private float maxSpeed = 1500f;
    [SerializeField] private float minSpeed = 500f;

    [Header("Other")]
    [SerializeField] private MessageManager _messageManager;
    [SerializeField] private SlotMachineSound _slotMachineSound;
    
    [Header("Other")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _spinSound;
    [SerializeField] private AudioClip _youWonSound;

    private readonly float[] slotPositions = { -574f, -446f, -318f, -190f, -62f, 66f, 194f, 322f, 446f };
    private bool _isGameRunning = false;
    private float _currentBet;
    private float _currentBalance;
    private bool _isBetConfirmed = false;
    private bool _isIncreasing = false;
    private bool _isDecreasing = false;
    private float _changeRate = 10f;
    private float _holdDelay = 0.1f;

    private void OnEnable()
    {
        _currentBalance = DataManager.Instance.SaveData.Balance;
        UpdateUI();
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
        _isGameRunning = true;
        UpdateBalance(-_currentBet);
        _startButton.interactable = false;

        StartCoroutine(SpinCoroutine());
        Debug.Log("StartCoroutine(SpinCoroutine()) запущен");
    }

    private IEnumerator SpinCoroutine()
    {
        float slot1Speed = Random.Range(minSpeed, maxSpeed);
        float slot2Speed = Random.Range(minSpeed, maxSpeed);
        float slot3Speed = Random.Range(minSpeed, maxSpeed);

        Coroutine spinSlot1 = StartCoroutine(SpinSlot(slot1, slot1Speed, 0f));
        Coroutine spinSlot2 = StartCoroutine(SpinSlot(slot2, slot2Speed, 0.5f));
        Coroutine spinSlot3 = StartCoroutine(SpinSlot(slot3, slot3Speed, 1f));

        PlaySpinSound();

        yield return spinSlot1;
        yield return spinSlot2;
        yield return spinSlot3;

        CheckResult();

        _isGameRunning = false;
        _isBetConfirmed = false;
        UpdateUI();
    }

    private IEnumerator SpinSlot(RectTransform slot, float initialSpeed, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        float speed = 0f;
        float elapsedTime = 0f;

        while (speed < initialSpeed)
        {
            speed += acceleration * Time.deltaTime;
            MoveSlot(slot, speed);
            yield return null;
        }

        while (elapsedTime < 2f)
        {
            MoveSlot(slot, speed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        while (speed > 0)
        {
            speed -= deceleration * Time.deltaTime;
            MoveSlot(slot, speed);
            yield return null;
        }

        float closestPosition = FindClosestPosition(slot.anchoredPosition.y);
        slot.anchoredPosition = new Vector2(slot.anchoredPosition.x, closestPosition);
    }

    private void MoveSlot(RectTransform slot, float speed)
    {
        slot.anchoredPosition -= new Vector2(0, speed * Time.deltaTime);

        if (slot.anchoredPosition.y <= -574f)
        {
            Debug.Log("Возвращение");
            slot.anchoredPosition = new Vector2(slot.anchoredPosition.x, 574f);
        }
    }

    private float FindClosestPosition(float currentY)
    {
        float closest = slotPositions[0];
        float minDistance = Mathf.Abs(currentY - closest);

        foreach (float position in slotPositions)
        {
            float distance = Mathf.Abs(currentY - position);
            if (distance < minDistance)
            {
                closest = position;
                minDistance = distance;
            }
        }

        return closest;
    }

    private void CheckResult()
    {
        var winMultiplier = 0;

        var slot1Symbol = slot1.anchoredPosition.y;
        var slot2Symbol = slot2.anchoredPosition.y;
        var slot3Symbol = slot3.anchoredPosition.y;

        if (slot1Symbol == slot2Symbol && slot2Symbol == slot3Symbol)
        {
            winMultiplier = 10;
            _messageManager.AddBigWinMessage();
            PlayYouWonSound(1f);
        }
        else if ((slot1Symbol == slot2Symbol && !IsSameCategory(slot1Symbol, slot3Symbol)) ||
                (slot1Symbol == slot3Symbol && !IsSameCategory(slot1Symbol, slot2Symbol)) ||
                (slot2Symbol == slot3Symbol && !IsSameCategory(slot2Symbol, slot1Symbol)))
        {
            winMultiplier = 2;
            PlayYouWonSound(0.5f);
        }
        else if ((slot1Symbol == slot2Symbol && IsSameCategory(slot1Symbol, slot3Symbol)) ||
                (slot1Symbol == slot3Symbol && IsSameCategory(slot1Symbol, slot2Symbol)) ||
                (slot2Symbol == slot3Symbol && IsSameCategory(slot2Symbol, slot1Symbol)))
        {
            winMultiplier = 3;
            PlayYouWonSound(0.7f);
        }
        else if (IsAllDifferentAndSameCategory(slot1Symbol, slot2Symbol, slot3Symbol))
        {
            winMultiplier = 1;
            PlayYouWonSound(0.3f);
        }

        if (winMultiplier > 0)
        {
            UpdateBalance(_currentBet * winMultiplier);
        }
        else if (_currentBalance < _minBet)
        {
            _messageManager.AddBigLossMessage();
        }
    }

    private bool IsSameCategory(float symbol1, float symbol2)
    {
        return (IsFoodCategory(symbol1) && IsFoodCategory(symbol2)) || 
            (IsSymbolCategory(symbol1) && IsSymbolCategory(symbol2));
    }

    private bool IsAllDifferentAndSameCategory(float symbol1, float symbol2, float symbol3)
    {
        return symbol1 != symbol2 && symbol2 != symbol3 && symbol1 != symbol3 && 
            ((IsFoodCategory(symbol1) && IsFoodCategory(symbol2) && IsFoodCategory(symbol3)) ||
                (IsSymbolCategory(symbol1) && IsSymbolCategory(symbol2) && IsSymbolCategory(symbol3)));
    }

    private bool IsFoodCategory(float symbol)
    {
        return symbol == -574f || symbol == -446f || symbol == -318f || symbol == -190f;
    }

    private bool IsSymbolCategory(float symbol)
    {
        return symbol == -62f || symbol == 66f || symbol == 194f || symbol == 322f || symbol == 446f;
    }

    private void UpdateUI()
    {
        _startButton.interactable = _currentBalance >= _minBet;
    }

    private void UpdateBalance(float value)
    {
        DataManager.Instance.ChangeBalance(value);
        _currentBalance = DataManager.Instance.SaveData.Balance;
    }

    public void PlaySpinSound()
    {
        PlaySound(_spinSound, 0.7f);
    }

    public void PlayYouWonSound(float volumeMultiplier)
    {
        PlaySound(_youWonSound, volumeMultiplier);
    }

    private void PlaySound(AudioClip clip, float volumeMultiplier)
    {
        if (clip != null)
        {
            float volume = SettingsManager.Instance.Settings.Volume; 
            _audioSource.PlayOneShot(clip, volume * volumeMultiplier);
        }
    }
}
