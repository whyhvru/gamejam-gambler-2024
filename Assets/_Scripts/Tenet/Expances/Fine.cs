public class Fine : Expense
{
    protected override void Start()
    {
        base.Start();

        amount = dataService.SaveData.balance / 2f;

        UpdateVisibility();

        if (dataService.SaveData.bankDebt > 0f && dataService.SaveData.overdueLoanBank)
        {
            isSelected = true;
        }
    }

    protected override bool ShouldBeVisible() =>
        dataService.SaveData.bankDebt > 0f && dataService.SaveData.overdueLoanBank;
}
