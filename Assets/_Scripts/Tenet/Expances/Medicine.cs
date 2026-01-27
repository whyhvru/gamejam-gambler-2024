public class Medicine : Expense
{
    protected override void Start()
    {
        base.Start();
        amount = 125f;
        UpdateVisibility();
    }

    protected override bool ShouldBeVisible() => dataService.SaveData.motherIsAlive;
}