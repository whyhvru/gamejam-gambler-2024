using UnityEngine;

public class Medicine : Expense
{
    protected override void Start() 
    {
        base.Start();
        _amount = 125f;
        UpdateVisibility();
    }

    protected override bool ShouldBeVisible()
    {
        return (DataManager.Instance.SaveData.MotherIsAlive);
    }
}