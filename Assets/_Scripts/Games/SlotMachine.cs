using System.Collections;
using Module.Core;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class SlotMachine : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private RectTransform _slot1;
    [SerializeField] private RectTransform _slot2;
    [SerializeField] private RectTransform _slot3;
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
    [SerializeField] private float _acceleration = 0.1f;
    [SerializeField] private float _deceleration = 0.2f;
    [SerializeField] private float _maxSpeed = 1500f;
    [SerializeField] private float _minSpeed = 500f;

    [Header("Dependencies")]
    [SerializeField] private MessageManager _messageManager;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _spinSound;
    [SerializeField] private AudioClip _youWonSound;

    private readonly float[] _slotPositions = { -574f, -446f, -318f, -190f, -62f, 66f, 194f, 322f, 446f };

    private IDataService _dataService;
    private float _currentBet;
    private float _currentBalance;
    private float _betChangeDelta;
    private bool _isGameRunning;
    private bool _isBetConfirmed;

    private const float ChangeRate = 10f;
    private const float HoldDelay = 0.1f;

    [Inject]
    public void Construct(IDataService data) => _dataService = data;

    private void OnEnable()
    {
        RefreshBalance();
        UpdateUI();
    }

    private void Start()
    {
        RefreshBalance();
        UpdateUI();

        _startButton.onClick.AddListener(OnStartButtonClick);
        _confirmBetButton.onClick.AddListener(ConfirmBet);
        _cancelBetButton.onClick.AddListener(() => _betWindow.SetActive(false));

        AddHoldListener(_increaseBetButton, () => _betChangeDelta = ChangeRate, () => _betChangeDelta = 0);
        AddHoldListener(_decreaseBetButton, () => _betChangeDelta = -ChangeRate, () => _betChangeDelta = 0);

        _maxBetButton.onClick.AddListener(() => SetBet(Mathf.Floor(_currentBalance / ChangeRate) * ChangeRate));
        _minBetButton.onClick.AddListener(() => SetBet(_minBet));

        _betWindow.SetActive(false);

        InvokeRepeating(nameof(UpdateBet), HoldDelay, HoldDelay);
    }

    private void UpdateBet()
    {
        if (_betChangeDelta != 0) ChangeBet(_betChangeDelta);
    }

    private void AddHoldListener(Button button, System.Action onPress, System.Action onRelease)
    {
        var trigger = button.gameObject.AddComponent<EventTrigger>();

        trigger.triggers.Add(CreateEvent(EventTriggerType.PointerDown, _ => onPress()));
        trigger.triggers.Add(CreateEvent(EventTriggerType.PointerUp, _ => onRelease()));
    }

    private EventTrigger.Entry CreateEvent(EventTriggerType type, UnityEngine.Events.UnityAction<BaseEventData> callback)
    {
        EventTrigger.Entry entry = new()
        {
            eventID = type,
            callback = new EventTrigger.TriggerEvent()
        };
        entry.callback.AddListener(callback);
        return entry;
    }

    private void OnStartButtonClick()
    {
        if (_isGameRunning) return;

        if (!_isBetConfirmed)
            OpenBetWindow();
        else
            StartGame();
    }

    private void OpenBetWindow()
    {
        _betText.text = $"{_currentBet:F0}";
        _betWindow.SetActive(true);
    }

    private void ConfirmBet()
    {
        if (_currentBet < _minBet || _currentBet > _currentBalance)
        {
            Debug.LogWarning($"Некорректная ставка. Ставка должна быть от {_minBet} до {_currentBalance}.");
            return;
        }

        _isBetConfirmed = true;
        _betWindow.SetActive(false);
    }

    private void ChangeBet(float delta) => SetBet(Mathf.Clamp(_currentBet + delta, _minBet, Mathf.Floor(_currentBalance)));

    private void SetBet(float value)
    {
        _currentBet = Mathf.Floor(value / ChangeRate) * ChangeRate;
        _betText.text = $"{_currentBet:F0}";
    }

    private void StartGame()
    {
        _isGameRunning = true;
        UpdateBalance(-_currentBet);
        _startButton.interactable = false;

        StartCoroutine(SpinCoroutine());
    }

    private IEnumerator SpinCoroutine()
    {
        var spin1 = StartCoroutine(SpinSlot(_slot1, Random.Range(_minSpeed, _maxSpeed), 0f));
        var spin2 = StartCoroutine(SpinSlot(_slot2, Random.Range(_minSpeed, _maxSpeed), 0.5f));
        var spin3 = StartCoroutine(SpinSlot(_slot3, Random.Range(_minSpeed, _maxSpeed), 1f));

        PlaySound(_spinSound, 0.7f);

        yield return spin1;
        yield return spin2;
        yield return spin3;

        CheckResult();

        _isGameRunning = false;
        _isBetConfirmed = false;
        UpdateUI();
    }

    private IEnumerator SpinSlot(RectTransform slot, float targetSpeed, float delay)
    {
        yield return new WaitForSeconds(delay);

        float speed = 0f;
        float elapsed = 0f;

        while (speed < targetSpeed)
        {
            speed += _acceleration * Time.deltaTime;
            MoveSlot(slot, speed);
            yield return null;
        }

        while (elapsed < 2f)
        {
            MoveSlot(slot, speed);
            elapsed += Time.deltaTime;
            yield return null;
        }

        while (speed > 0)
        {
            speed -= _deceleration * Time.deltaTime;
            MoveSlot(slot, speed);
            yield return null;
        }

        float snapPos = FindClosestPosition(slot.anchoredPosition.y);
        slot.anchoredPosition = new Vector2(slot.anchoredPosition.x, snapPos);
    }

    private void MoveSlot(RectTransform slot, float speed)
    {
        slot.anchoredPosition -= new Vector2(0, speed * Time.deltaTime);
        if (slot.anchoredPosition.y <= -574f)
            slot.anchoredPosition = new Vector2(slot.anchoredPosition.x, 574f);
    }

    private float FindClosestPosition(float y)
    {
        float closest = _slotPositions[0];
        float minDist = Mathf.Abs(y - closest);

        foreach (float pos in _slotPositions)
        {
            float dist = Mathf.Abs(y - pos);
            if (dist < minDist)
            {
                closest = pos;
                minDist = dist;
            }
        }

        return closest;
    }

    private void CheckResult()
    {
        float s1 = _slot1.anchoredPosition.y;
        float s2 = _slot2.anchoredPosition.y;
        float s3 = _slot3.anchoredPosition.y;

        int multiplier = GetWinMultiplier(s1, s2, s3);

        if (multiplier > 0)
        {
            UpdateBalance(_currentBet * multiplier);
            PlaySound(_youWonSound, multiplier >= 10 ? 1f : 0.5f);
            if (multiplier >= 10) _messageManager.AddBigWinMessage();
        }
        else if (_currentBalance < _minBet)
        {
            _messageManager.AddBigLossMessage();
        }
    }

    private int GetWinMultiplier(float s1, float s2, float s3)
    {
        bool allEqual = s1 == s2 && s2 == s3;
        bool twoEqual = (s1 == s2) || (s1 == s3) || (s2 == s3);

        if (allEqual) return 10;
        if (twoEqual) return IsAllSameCategory(s1, s2, s3) ? 3 : 2;
        if (IsAllDifferentAndSameCategory(s1, s2, s3)) return 1;

        return 0;
    }

    private bool IsAllSameCategory(float s1, float s2, float s3) =>
        (IsFood(s1) && IsFood(s2) && IsFood(s3)) ||
        (IsSymbol(s1) && IsSymbol(s2) && IsSymbol(s3));

    private bool IsAllDifferentAndSameCategory(float s1, float s2, float s3) =>
        s1 != s2 && s2 != s3 && s1 != s3 && IsAllSameCategory(s1, s2, s3);

    private bool IsFood(float y) => y is -574f or -446f or -318f or -190f;
    private bool IsSymbol(float y) => y is -62f or 66f or 194f or 322f or 446f;
    private void UpdateUI() => _startButton.interactable = _currentBalance >= _minBet;
    private void RefreshBalance() => _currentBalance = _dataService.SaveData.balance;

    private void UpdateBalance(float value)
    {
        _dataService.ChangeBalance(value);
        RefreshBalance();
    }

    private void PlaySound(AudioClip clip, float volumeMultiplier)
        => _audioSource.PlayOneShot(clip, volumeMultiplier);
}
