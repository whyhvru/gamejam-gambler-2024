public class Heat : Expense
{
    protected override void Start()
    {
        base.Start();
        amount = 10f;
        UpdateVisibility();
    }
}
