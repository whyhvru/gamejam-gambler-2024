using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameTime : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timeText;
    
    private DataManager _dataManager;
    private int _hours = 12;
    private int _minutes = 0;

    // Реальное игровое время по отношению к игровой единице
    private float _updateInterval = 3f;

    private bool _isDayActive = true;

    private void Start() 
    {
        _dataManager = DataManager.Instance;
        StartCoroutine(UpdateGameTime());    
    }

    private IEnumerator UpdateGameTime()
    {
        while (_isDayActive)
        {
            UpdateTimeDisplay();

            yield return new WaitForSeconds(_updateInterval);

            // Добавление игровой единицы
            AddTime(15);
        }
    }

    private void UpdateTimeDisplay()
    {
        _timeText.text = $"{_hours:D2}:{_minutes:D2}";
    }

    private void AddTime(int minutesToAdd)
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
            DataManager.Instance.NewDay();
        }
        else if (_hours == 3)
        {
            _isDayActive = false;
            EndOfDay();
        }
        else
        {
            UpdateTimeDisplay();
        }
    }

    public void SkipTime(int minutesToSkip)
    {
        if (_isDayActive)
        {
            AddTime(minutesToSkip);
        }
    }

    private void EndOfDay()
    {
        if (!IsGameOver())
        {
            _dataManager.Save();
            SceneLoader.Instance.LoadTenet();
        }
        else
        {
            SceneLoader.Instance.LoadEnd();
        }
    }

    private bool IsGameOver()
    {
        bool aLotOfDebt = (_dataManager.SaveData.Debts >= 100000f);
        bool everyoneIsDead = (!_dataManager.SaveData.MotherIsAlive && !_dataManager.SaveData.WifeIsAlive && 
                                !_dataManager.SaveData.ChildIsAlive && !_dataManager.SaveData.Child2IsAlive);
        
        bool isGameOver = (aLotOfDebt && everyoneIsDead);
        
        return isGameOver;
    }
}