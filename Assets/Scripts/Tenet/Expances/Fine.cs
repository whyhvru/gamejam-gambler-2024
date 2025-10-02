using UnityEngine;

public class Fine : Expense
{
    protected override void Start() 
    {
        base.Start();
        
        _amount = DataManager.Instance.SaveData.Balance / 2f;

        UpdateVisibility();

        if (DataManager.Instance.SaveData.BankDebt > 0f && DataManager.Instance.SaveData.OverdueLoanBank)
        {
            _isSelected = true;
        }
    }

    protected override bool ShouldBeVisible()
    {
        return (DataManager.Instance.SaveData.BankDebt > 0f && DataManager.Instance.SaveData.OverdueLoanBank);
    }
}
