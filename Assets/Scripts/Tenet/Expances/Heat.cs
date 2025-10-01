using UnityEngine;

public class Heat : Expense
{
    protected override void Start() 
    {
        base.Start();
        _amount = 10f;
        UpdateVisibility();
    }
}
