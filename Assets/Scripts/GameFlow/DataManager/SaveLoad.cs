using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public int Day = 1;
    public int Month = 11;

    public float Balance = 100f;
    public float DailyIncome;
    public float DailyLesion;

    public float Debts = 0f;
    public float DebtFriends = 0f;
    public int NumberOfLoansFriends = 0; 
    public float BankDebt = 0f;
    public float Microloan = 0f;
    public float CarDebt = 0f;

    public bool DebtTo1Friend = false;
    public bool DebtTo2Friend = false;
    public bool DebtTo3Friend = false;
    public bool DebtTo4Friend = false;
    public bool DebtTo5Friend = false;

    public int DaysUnpaidDebtFriends = 0;

    public int DaysUnpaidBank = 0;
    public bool OverdueLoanBank = false;
    public bool BanInBank = false;
    public int DaysUnpaidMicroloan = 0;
    public int DaysUnpaidCarDebt = 0;
    public int DaysWithoutFood = 0;
    public int DaysWithoutHeat = 0;
    public int DaysWithoutMeds = 0;

    public bool MotherIsAlive = true;
    public bool WifeIsAlive = true;
    public bool ChildIsAlive = true;
    public bool Child2IsAlive = true;

    public bool HasACar = true;
    public bool HasAApart = true;
    public bool CarIsReturnable = true;
}
