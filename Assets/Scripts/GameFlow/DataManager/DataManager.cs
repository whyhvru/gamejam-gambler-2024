using UnityEngine;
using System;
using System.IO;

public sealed class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    private string _saveFilePath;

    public SaveData SaveData { get; private set; }

    public event Action OnDataChanged;
    public event Action OnDateChanged;

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

    #region Save/Load
    public void Save()
    {
        string json = JsonUtility.ToJson(SaveData, true);
        File.WriteAllText(_saveFilePath, json);
    }

    public void Load()
    {
        if (File.Exists(_saveFilePath))
        {
            string json = File.ReadAllText(_saveFilePath);
            SaveData = JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            SaveData = new SaveData();
            Save();
        }

        NotifyDataChanged();
    }
    #endregion

    #region Date
    public void NewDay()
    {
        SaveData.day++;
        Save();
        NotifyDateChanged();
    }
    #endregion

    #region Balance/Debts
    public void AddDebt(float value)
    {
        ChangeBalance(value);
        UpdateDebts();
    }

    private void UpdateDebts()
    {
        SaveData.debts = SaveData.debtFriends + SaveData.bankDebt + SaveData.microloan + SaveData.carDebt;
        Save();
        NotifyDataChanged();
    }

    public void ChangeBalance(float value)
    {
        SaveData.balance += value;

        if (value > 0)
            SaveData.dailyIncome += value;
        else
            SaveData.dailyLesion += value;

        Save();
        NotifyDataChanged();
    }
    #endregion

    #region Expenses
    public void ResetDebt(string expenseName)
    {
        switch (expenseName)
        {
            case "Питание":
                SaveData.daysWithoutFood = 0;
                break;

            case "Отопление":
                SaveData.daysWithoutHeat = 0;
                break;

            case "Лекарство_матери":
                SaveData.daysWithoutMeds = 0;
                break;

            case "Долг_друзьям":
                SaveData.daysUnpaidDebtFriends = 0;
                SaveData.debtFriends = 0f;
                break;

            case "Задолженность_в_банке":
                SaveData.daysUnpaidBank = 0;
                SaveData.bankDebt = 0f;
                break;

            case "Микрозайм":
                SaveData.daysUnpaidMicroloan = 0;
                SaveData.microloan = 0f;
                break;

            case "Выкуп_машины":
                SaveData.daysUnpaidCarDebt = 0;
                SaveData.carDebt = 0f;
                break;

            default:
                Debug.LogWarning($"Unknown expense name: {expenseName}");
                return;
        }

        UpdateDebts();
    }

    public void IncreaseDebt(string expenseName)
    {
        switch (expenseName)
        {
            case "Питание":
                SaveData.daysWithoutFood++;
                break;

            case "Отопление":
                SaveData.daysWithoutHeat++;
                break;

            case "Лекарства_матери":
                SaveData.daysWithoutMeds++;
                break;

            case "Долг_друзьям":
                SaveData.daysUnpaidDebtFriends++;
                break;

            case "Задолженность_в_банке":
                SaveData.daysUnpaidBank++;
                break;

            case "Микрозайм":
                SaveData.daysUnpaidMicroloan++;
                if (SaveData.daysUnpaidMicroloan > 2)
                    SaveData.microloan *= 1.25f;
                break;

            case "Выкуп_машины":
                SaveData.daysUnpaidCarDebt++;
                if (SaveData.daysUnpaidCarDebt > 4)
                {
                    SaveData.carDebt = 0f;
                    SaveData.hasACar = false;
                    SaveData.carIsReturnable = false;
                }
                break;

            default:
                Debug.LogWarning($"Unknown expense name: {expenseName}");
                return;
        }

        UpdateDebts();
    }
    #endregion

    #region Private Helpers
    private void NotifyDataChanged() => OnDataChanged?.Invoke();
    private void NotifyDateChanged() => OnDateChanged?.Invoke();
    #endregion
}
