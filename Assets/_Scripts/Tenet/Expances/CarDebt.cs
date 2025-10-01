using UnityEngine;

public class CarDebt : Expense
{
    protected override void Start()
    {
        base.Start();
        _amount = _dataService.Data.CarDebt;
        UpdateVisibility();
    }

    protected override bool ShouldBeVisible()
    {
        return _dataService.Data.CarDebt > 0f;
    }
}