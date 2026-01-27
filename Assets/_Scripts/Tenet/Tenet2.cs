using System.Collections.Generic;
using Module.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class DayReport : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _balance;
    [SerializeField] private TextMeshProUGUI _win;
    [SerializeField] private TextMeshProUGUI _loss;
    [SerializeField] private Button _sleepButton;
    [SerializeField] private TextMeshProUGUI _currentBalanceText;

    private IDataService _dataService;
    private IAppFlowService _appFlowService;

    public List<Expense> expenses;

    private float _currentBalance;

    [Inject]
    public void Construct(IDataService data, IAppFlowService appFlow)
    {
        _dataService = data;
        _appFlowService = appFlow;
    }

    private void Start()
    {
        UpdateStats();
        _sleepButton.onClick.AddListener(Sleep);
        _currentBalance = _dataService.SaveData.balance;
        UpdateBalance();
    }

    private void UpdateStats()
    {
        _balance.text = $"Текущий_баланс: ${_dataService.SaveData.balance:F2}";
        _win.text = $"Заработано: ${_dataService.SaveData.dailyIncome:F2}";
        _loss.text = $"Потрачено: ${_dataService.SaveData.dailyLesion:F2}";
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
                _dataService.ChangeBalance(-expense.Amount);
                _dataService.ResetDebt(expense.ExpenseName);
            }
            else if (expense.gameObject.activeSelf && !expense.IsSelected)
            {
                _dataService.IncreaseDebt(expense.ExpenseName);
            }
        }
    }

    private void Sleep()
    {
        EndDay();
        ClearDailyStats();
        _appFlowService.LoadGame();
    }

    private void ClearDailyStats()
    {
        _dataService.SaveData.dailyIncome = 0;
        _dataService.SaveData.dailyLesion = 0;
    }
}
