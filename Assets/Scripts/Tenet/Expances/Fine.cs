public class Fine : Expense
{
    protected override void Start()
    {
        base.Start();

        amount = DataManager.Instance.SaveData.balance / 2f;

        UpdateVisibility();

        if (DataManager.Instance.SaveData.bankDebt > 0f && DataManager.Instance.SaveData.overdueLoanBank)
        {
            isSelected = true;
        }
    }

    protected override bool ShouldBeVisible() =>
        DataManager.Instance.SaveData.bankDebt > 0f && DataManager.Instance.SaveData.overdueLoanBank;
}
