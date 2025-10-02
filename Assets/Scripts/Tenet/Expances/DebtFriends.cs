using UnityEngine;

public class DebtFriends : Expense
{
    protected override void Start() 
    {
        base.Start();
        _amount = DataManager.Instance.SaveData.DebtFriends;
        UpdateVisibility();
    }

    protected override bool ShouldBeVisible()
    {
        return (DataManager.Instance.SaveData.DebtFriends > 0f);
    }
}