public class CarDebt : Expense
{
    protected override void Start()
    {
        base.Start();
        amount = DataManager.Instance.SaveData.carDebt;
        UpdateVisibility();
    }

    protected override bool ShouldBeVisible() => DataManager.Instance.SaveData.carDebt > 0f;
}