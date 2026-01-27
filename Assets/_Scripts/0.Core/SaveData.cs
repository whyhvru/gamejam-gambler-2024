namespace Module.Core
{
    [System.Serializable]
    public class SaveData
    {
        public int day = 1;
        public int month = 11;

        public float balance = 100f;
        public float dailyIncome;
        public float dailyLesion;

        public float debts = 0f;
        public float debtFriends = 0f;
        public int numberOfLoansFriends = 0;
        public float bankDebt = 0f;
        public float microloan = 0f;
        public float carDebt = 0f;

        public bool debtTo1Friend = false;
        public bool debtTo2Friend = false;
        public bool debtTo3Friend = false;
        public bool debtTo4Friend = false;
        public bool debtTo5Friend = false;

        public int daysUnpaidDebtFriends = 0;

        public int daysUnpaidBank = 0;
        public bool overdueLoanBank = false;
        public bool banInBank = false;
        public int daysUnpaidMicroloan = 0;
        public int daysUnpaidCarDebt = 0;
        public int daysWithoutFood = 0;
        public int daysWithoutHeat = 0;
        public int daysWithoutMeds = 0;

        public bool motherIsAlive = true;
        public bool wifeIsAlive = true;
        public bool childIsAlive = true;
        public bool child2IsAlive = true;

        public bool hasACar = true;
        public bool hasAApart = true;
        public bool carIsReturnable = true;
    }
}