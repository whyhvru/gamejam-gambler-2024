public class DebtFriends : Expense
{
    protected override void Start()
    {
        base.Start();
        amount = dataService.SaveData.debtFriends;
        UpdateVisibility();
    }

    protected override bool ShouldBeVisible() => dataService.SaveData.debtFriends > 0f;
}