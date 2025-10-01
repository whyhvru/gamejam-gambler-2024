using UnityEngine;

public class Fine : Expense
{
    protected override void Start()
    {
        base.Start();

        _amount = _dataService.Data.Balance / 2f;

        UpdateVisibility();

        if (_dataService.Data.BankDebt > 0f && _dataService.Data.OverdueLoanBank)
        {
            _isSelected = true;
        }
    }

    protected override bool ShouldBeVisible()
    {
        return _dataService.Data.BankDebt > 0f && _dataService.Data.OverdueLoanBank;
    }
}
