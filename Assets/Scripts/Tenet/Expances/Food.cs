using UnityEngine;

public class Food : Expense
{
    protected override void Start() 
    {
        base.Start();
        _amount = (GetAliveFamilyCount() * 10f) + 10f;
        UpdateVisibility();
    }

    private int GetAliveFamilyCount()
    {
        return (DataManager.Instance.SaveData.MotherIsAlive ? 1 : 0) +
            (DataManager.Instance.SaveData.WifeIsAlive ? 1 : 0) +
            (DataManager.Instance.SaveData.ChildIsAlive ? 1 : 0) +
            (DataManager.Instance.SaveData.Child2IsAlive ? 1 : 0);
    }
}
