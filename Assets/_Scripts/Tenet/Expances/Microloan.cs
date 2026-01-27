public class Microloan : Expense
{
    protected override void Start()
    {
        base.Start();
        amount = dataService.SaveData.microloan;
        UpdateVisibility();
    }

    protected override bool ShouldBeVisible() => dataService.SaveData.microloan > 0f;
}