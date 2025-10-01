using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using Module.Core;
using Module.Gameplay;

public class DayReport : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _balance;
    [SerializeField] private TextMeshProUGUI _win;
    [SerializeField] private TextMeshProUGUI _loss;
    [SerializeField] private Button _sleepButton;
    [SerializeField] private TextMeshProUGUI _currentBalanceText;

    public List<Expense> expenses;
    private IGameFlowService _gameFlowService;
    private float _currentBalance;
    private IDataService _dataService;
    private BalanceService _balanceService;
    private DebtService _debtService;

    private void Awake()
    {
        _gameFlowService = ServiceRegistry.Get<IGameFlowService>();
        _dataService = ServiceRegistry.Get<IDataService>();
    }

    private void Start()
    {
        UpdateStats();
        _sleepButton.onClick.AddListener(Sleep);
        _currentBalance = _dataService.Data.Balance;
        UpdateBalance();
    }

    private void UpdateStats()
    {
        _balance.text = $"Текущий_баланс: ${_dataService.Data.Balance:F2}";
        _win.text = $"Заработано: ${_dataService.Data.DailyIncome:F2}";
        _loss.text = $"Потрачено: ${_dataService.Data.DailyLesion:F2}";
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
                _balanceService.ChangeBalance(-expense.Amount);
                _debtService.ResetDebt(expense.DebtType);
            }
            else if (expense.gameObject.activeSelf && !expense.IsSelected)
            {
                _debtService.IncreaseDebt(expense.DebtType);
            }
        }
    }

    private void Sleep()
    {
        EndDay();
        ClearDailyStats();
        _gameFlowService.LoadGame();
    }

    private void ClearDailyStats()
    {
        _dataService.Data.DailyIncome = 0;
        _dataService.Data.DailyLesion = 0;
    }
}
