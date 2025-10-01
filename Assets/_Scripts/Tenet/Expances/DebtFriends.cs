using UnityEngine;

public class DebtFriends : Expense
{
    protected override void Start()
    {
        base.Start();
        _amount = _dataService.Data.DebtFriends;
        UpdateVisibility();
    }

    protected override bool ShouldBeVisible()
    {
        return _dataService.Data.DebtFriends > 0f;
    }
}