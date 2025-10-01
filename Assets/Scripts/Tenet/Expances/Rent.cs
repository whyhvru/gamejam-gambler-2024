using UnityEngine;

public class Rent : Expense
{
    protected override void Start() 
    {
        base.Start();
        _amount = 45f;
        _isSelected = true;
        UpdateVisibility();
    }

    protected override bool ShouldBeVisible()
    {
        return (DataManager.Instance.SaveData.HasAApart);
    }
}
