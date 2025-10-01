using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    private string _saveFilePath;

    private SaveData _saveData;

    public SaveData SaveData => _saveData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            _saveFilePath = Path.Combine(Application.persistentDataPath, "SaveData.json");
            Load();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(_saveData, true);
        File.WriteAllText(_saveFilePath, json);
    }

    public void Load()
    {
        if (File.Exists(_saveFilePath))
        {
            string json = File.ReadAllText(_saveFilePath);
            _saveData = JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            _saveData = new SaveData();
            Save();
        }
    }

    public void NewDay()
    {
        int CurrentDay = _saveData.Day;
        CurrentDay++;
        _saveData.Day = CurrentDay;
        Save();
    }

    public void AddDebt(float value)
    {
        ChangeBalance(value);
        UpdateDebts();
    }

    private void UpdateDebts()
    {
        _saveData.Debts = _saveData.DebtFriends + _saveData.BankDebt + _saveData.Microloan + _saveData.CarDebt;
    }

    public void ChangeBalance(float value)
    {
        _saveData.Balance += value;
        
        (value > 0 ? ref _saveData.DailyIncome : ref _saveData.DailyLesion) += value;

        Save();
    }

    public void ResetDebt(string expenseName)
    {
        switch (expenseName)
        {
            case "Питание":
                _saveData.DaysWithoutFood = 0;
                break;

            case "Отопление":
                _saveData.DaysWithoutHeat = 0;
                break;

            case "Лекарство_матери":
                _saveData.DaysWithoutMeds = 0;
                break;

            case "Долг_друзьям":
                _saveData.DaysUnpaidDebtFriends = 0;
                _saveData.DebtFriends = 0f;
                break;

            case "Задолженность_в_банке":
                _saveData.DaysUnpaidBank = 0;
                _saveData.BankDebt = 0f;
                break;

            case "Микрозайм":
                _saveData.DaysUnpaidMicroloan = 0;
                _saveData.Microloan = 0f;
                break;

            case "Выкуп_машины":
                _saveData.DaysUnpaidCarDebt = 0;
                _saveData.CarDebt = 0f;
                break;

            default:
                Debug.LogWarning($"Unknown expense name: {expenseName}");
                break;
        }

        UpdateDebts();
    }

    public void IncreaseDebt(string expenseName)
    {
        switch (expenseName)
        {
            case "Питание":
                _saveData.DaysWithoutFood = _saveData.DaysWithoutFood + 1;
                break;

            case "Отопление":
                _saveData.DaysWithoutHeat = _saveData.DaysWithoutHeat + 1;
                break;

            case "Лекарства_матери":
                _saveData.DaysWithoutMeds = _saveData.DaysWithoutMeds + 1;
                break;

            case "Долг_друзьям":
                _saveData.DaysUnpaidDebtFriends = _saveData.DaysUnpaidDebtFriends + 1;
                break;

            case "Задолженность_в_банке":
                _saveData.DaysUnpaidBank = _saveData.DaysUnpaidBank + 1;
                break;

            case "Микрозайм":
                _saveData.DaysUnpaidMicroloan = _saveData.DaysUnpaidMicroloan + 1;
                if (_saveData.DaysUnpaidMicroloan > 2f)
                {
                    _saveData.Microloan = _saveData.Microloan * 1.25f;
                }
                break;

            case "Выкуп_машины":
                _saveData.DaysUnpaidCarDebt = _saveData.DaysUnpaidCarDebt + 1;
                if (_saveData.DaysUnpaidCarDebt > 4)
                {
                    _saveData.CarDebt = 0f;
                    _saveData.HasACar = false;
                    _saveData.CarIsReturnable = false;
                }
                break;

            default:
                Debug.LogWarning($"Unknown expense name: {expenseName}");
                break;
        }

        UpdateDebts();
    }
}
