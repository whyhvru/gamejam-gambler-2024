using UnityEngine;

public class Microloan : Expense
{
    protected override void Start()
    {
        base.Start();
        _amount = _dataService.Data.Microloan;
        UpdateVisibility();
    }

    protected override bool ShouldBeVisible()
    {
        return _dataService.Data.Microloan > 0f;
    }
}