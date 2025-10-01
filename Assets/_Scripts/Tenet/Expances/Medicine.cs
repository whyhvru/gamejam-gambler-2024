using Module.Core;

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
        return _dataService.Data.MotherIsAlive;
    }
}