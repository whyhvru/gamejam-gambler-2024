public class CarDebt : Expense
{
    protected override void Start()
    {
        base.Start();
        amount = dataService.SaveData.carDebt;
        UpdateVisibility();
    }

    protected override bool ShouldBeVisible() => dataService.SaveData.carDebt > 0f;
}