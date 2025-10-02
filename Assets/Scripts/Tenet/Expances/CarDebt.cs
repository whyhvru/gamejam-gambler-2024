using UnityEngine;

public class CarDebt : Expense
{
    protected override void Start() 
    {
        base.Start();
        _amount = DataManager.Instance.SaveData.CarDebt;
        UpdateVisibility();
    }

    protected override bool ShouldBeVisible()
    {
        return (DataManager.Instance.SaveData.CarDebt > 0f);
    }
}