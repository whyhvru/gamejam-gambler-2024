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
        return (_dataService.Data.MotherIsAlive ? 1 : 0) +
            (_dataService.Data.WifeIsAlive ? 1 : 0) +
            (_dataService.Data.ChildIsAlive ? 1 : 0) +
            (_dataService.Data.Child2IsAlive ? 1 : 0);
    }
}
