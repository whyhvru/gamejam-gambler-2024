using UnityEngine;
using TMPro;
using System.Collections;

public sealed class GameTime : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _timeText;

    [Header("Time Settings")]
    [SerializeField] private int _startHour = 12;
    [SerializeField] private int _startMinute = 0;
    [SerializeField] private int _endOfDayHour = 3;
    [SerializeField] private float _updateInterval = 0.1f;
    [SerializeField] private int _minutesPerTick = 15;

    [Header("Game Over Settings")]
    [SerializeField] private float _maxDebt = 100000f;

    private DataManager _dataManager;
    private int _hours;
    private int _minutes;
    private bool _isDayActive;

    private void Start()
    {
        _dataManager = DataManager.Instance;

        _hours = _startHour;
        _minutes = _startMinute;
        _isDayActive = true;

        StartCoroutine(TimeRoutine());
        UpdateTimeDisplay();
    }

    private IEnumerator TimeRoutine()
    {
        while (_isDayActive)
        {
            yield return new WaitForSeconds(_updateInterval);
            AdvanceTime(_minutesPerTick);
        }
    }

    private void AdvanceTime(int minutesToAdd)
    {
        _minutes += minutesToAdd;

        while (_minutes >= 60)
        {
            _minutes -= 60;
            _hours++;
        }

        if (_hours >= 24)
        {
            _hours = 0;
            _dataManager.NewDay();
        }

        if (_hours == _endOfDayHour)
        {
            _isDayActive = false;
            HandleEndOfDay();
            return;
        }

        UpdateTimeDisplay();
    }

    public void SkipTime(int minutesToSkip)
    {
        if (_isDayActive)
            AdvanceTime(minutesToSkip);
    }

    private void UpdateTimeDisplay()
    {
        if (_timeText != null)
            _timeText.text = $"{_hours:D2}:{_minutes:D2}";
    }

    private void HandleEndOfDay()
    {
        _dataManager.Save();

        if (IsGameOver())
            SceneLoader.Instance.LoadEnd();
        else
            SceneLoader.Instance.LoadTenet();
    }

    private bool IsGameOver()
    {
        var save = _dataManager.SaveData;

        bool aLotOfDebt = save.debts >= _maxDebt;
        bool everyoneDead = !save.motherIsAlive &&
                            !save.wifeIsAlive &&
                            !save.childIsAlive &&
                            !save.child2IsAlive;

        return aLotOfDebt && everyoneDead;
    }
}
