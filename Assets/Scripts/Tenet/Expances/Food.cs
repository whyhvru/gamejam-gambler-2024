public class Food : Expense
{
    protected override void Start()
    {
        base.Start();
        amount = (GetAliveFamilyCount() * 10f) + 10f;
        UpdateVisibility();
    }

    private int GetAliveFamilyCount() =>
        (DataManager.Instance.SaveData.motherIsAlive ? 1 : 0) +
        (DataManager.Instance.SaveData.wifeIsAlive ? 1 : 0) +
        (DataManager.Instance.SaveData.childIsAlive ? 1 : 0) +
        (DataManager.Instance.SaveData.child2IsAlive ? 1 : 0);
}
