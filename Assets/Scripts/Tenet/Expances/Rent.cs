public class Rent : Expense
{
    protected override void Start()
    {
        base.Start();
        amount = 45f;
        isSelected = true;
        UpdateVisibility();
    }

    protected override bool ShouldBeVisible() => DataManager.Instance.SaveData.hasAApart;
}
