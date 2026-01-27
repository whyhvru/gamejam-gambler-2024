public class Food : Expense
{
    protected override void Start()
    {
        base.Start();
        amount = (GetAliveFamilyCount() * 10f) + 10f;
        UpdateVisibility();
    }

    private int GetAliveFamilyCount() =>
        (dataService.SaveData.motherIsAlive ? 1 : 0) +
        (dataService.SaveData.wifeIsAlive ? 1 : 0) +
        (dataService.SaveData.childIsAlive ? 1 : 0) +
        (dataService.SaveData.child2IsAlive ? 1 : 0);
}
