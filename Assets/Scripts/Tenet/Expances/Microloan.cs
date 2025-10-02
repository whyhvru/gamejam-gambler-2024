public class Microloan : Expense
{
    protected override void Start()
    {
        base.Start();
        amount = DataManager.Instance.SaveData.microloan;
        UpdateVisibility();
    }

    protected override bool ShouldBeVisible() => DataManager.Instance.SaveData.microloan > 0f;
}