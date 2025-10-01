using UnityEngine;

public class BankDebt : Expense
{
    protected override void Start() 
    {
        base.Start();
        _amount = DataManager.Instance.SaveData.BankDebt;
        UpdateVisibility();
    }

    protected override bool ShouldBeVisible()
    {
        return (DataManager.Instance.SaveData.BankDebt > 0f);
    }
}