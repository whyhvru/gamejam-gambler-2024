public class DebtFriends : Expense
{
    protected override void Start()
    {
        base.Start();
        amount = DataManager.Instance.SaveData.debtFriends;
        UpdateVisibility();
    }

    protected override bool ShouldBeVisible() => DataManager.Instance.SaveData.debtFriends > 0f;
}