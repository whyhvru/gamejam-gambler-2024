public class BankDebt : Expense
{
    protected override void Start()
    {
        base.Start();
        amount = DataManager.Instance.SaveData.bankDebt;
        UpdateVisibility();
    }

    protected override bool ShouldBeVisible() => DataManager.Instance.SaveData.bankDebt > 0f;
}