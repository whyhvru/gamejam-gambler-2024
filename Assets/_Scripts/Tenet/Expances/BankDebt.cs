using UnityEngine;

public class BankDebt : Expense
{
    protected override void Start()
    {
        base.Start();
        _amount = _dataService.Data.BankDebt;
        UpdateVisibility();
    }

    protected override bool ShouldBeVisible()
    {
        return _dataService.Data.BankDebt > 0f;
    }
}