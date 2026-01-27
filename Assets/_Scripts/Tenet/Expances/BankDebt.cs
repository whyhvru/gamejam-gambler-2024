public class BankDebt : Expense
{
    protected override void Start()
    {
        base.Start();
        amount = dataService.SaveData.bankDebt;
        UpdateVisibility();
    }

    protected override bool ShouldBeVisible() => dataService.SaveData.bankDebt > 0f;
}