using UnityEngine;

public class Microloan : Expense
{
    protected override void Start() 
    {
        base.Start();
        _amount = DataManager.Instance.SaveData.Microloan;
        UpdateVisibility();
    }

    protected override bool ShouldBeVisible()
    {
        return (DataManager.Instance.SaveData.Microloan > 0f);
    }
}