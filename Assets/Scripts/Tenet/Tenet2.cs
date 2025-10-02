using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class DayReport : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _balance;
    [SerializeField] private TextMeshProUGUI _win;
    [SerializeField] private TextMeshProUGUI _loss;
    [SerializeField] private Button _sleepButton;
    [SerializeField] private TextMeshProUGUI _currentBalanceText;

    public List<Expense> expenses;

    private float _currentBalance;

    private void Start() 
    {
        UpdateStats();
        _sleepButton.onClick.AddListener(Sleep);
        _currentBalance = DataManager.Instance.SaveData.Balance;
        UpdateBalance();
    }

    private void UpdateStats()
    {
        _balance.text = $"Текущий_баланс: ${DataManager.Instance.SaveData.Balance:F2}";
        _win.text = $"Заработано: ${DataManager.Instance.SaveData.DailyIncome:F2}";
        _loss.text = $"Потрачено: ${DataManager.Instance.SaveData.DailyLesion:F2}";
    }

    private void UpdateBalance()
    {
        float totalExpenses = 0;

        foreach (var expense in expenses)
        {
            if (expense.gameObject.activeSelf && expense.IsSelected)
            {
                totalExpenses += expense.Amount;
            }
        }

        float updatedBalance = _currentBalance - totalExpenses;
        _currentBalanceText.text = $"${updatedBalance:F2}";

        foreach (var expense in expenses)
        {
            if (expense.gameObject.activeSelf && !expense.IsSelected)
            {
                bool canAfford = updatedBalance >= expense.Amount;
                expense.UpdateButtonAvailability(canAfford);
            }
        }
    }

    public void OnExpenseSelected(Expense expense)
    {
        float totalExpenses = GetTotalSelectedExpenses();
        float updatedBalance = _currentBalance - totalExpenses;

        if (updatedBalance >= expense.Amount || expense.IsSelected)
        {
            expense.ToggleSelection();
            UpdateBalance();
        }
    }

    private float GetTotalSelectedExpenses()
    {
        float total = 0;
        foreach (var expense in expenses)
        {
            if (expense.gameObject.activeSelf && expense.IsSelected)
            {
                total += expense.Amount;
            }
        }
        return total;
    }

    public void EndDay()
    {
        foreach (var expense in expenses)
        {
            if (expense.gameObject.activeSelf && expense.IsSelected)
            {
                DataManager.Instance.ChangeBalance(-expense.Amount);
                DataManager.Instance.ResetDebt(expense.ExpenseName);
            }
            else if (expense.gameObject.activeSelf && !expense.IsSelected)
            {
                DataManager.Instance.IncreaseDebt(expense.ExpenseName);
            }
        }
    }

    private void Sleep()
    {
        EndDay();
        ClearDailyStats();
        SceneLoader.Instance.LoadGame();
    }

    private void ClearDailyStats()
    {
        DataManager.Instance.SaveData.DailyIncome = 0;
        DataManager.Instance.SaveData.DailyLesion = 0;
    }
}
