using Module.Core;

namespace Module.Gameplay
{
    public sealed class DebtService
    {
        private readonly IDataService _data;
        private readonly BalanceService _balanceService;

        public DebtService(IDataService dataService, BalanceService balanceService)
        {
            _data = dataService;
            _balanceService = balanceService;
        }

        public void AddDebt(float amount)
        {
            _balanceService.ChangeBalance(amount);
            UpdateDebts();
        }

        public void ResetDebt(DebtType type)
        {
            switch (type)
            {
                case DebtType.Food:
                    _data.Data.DaysWithoutFood = 0;
                    break;
                case DebtType.Heat:
                    _data.Data.DaysWithoutHeat = 0;
                    break;
                case DebtType.Meds:
                    _data.Data.DaysWithoutMeds = 0;
                    break;
                case DebtType.Friends:
                    _data.Data.DaysUnpaidDebtFriends = 0;
                    _data.Data.DebtFriends = 0f;
                    break;
                case DebtType.Bank:
                    _data.Data.BankDebt = 0;
                    _data.Data.DaysUnpaidBank = 0;
                    break;
                case DebtType.Microloan:
                    _data.Data.DaysUnpaidMicroloan = 0;
                    _data.Data.Microloan = 0f;
                    break;
                case DebtType.Car:
                    _data.Data.DaysUnpaidCarDebt = 0;
                    _data.Data.CarDebt = 0f;
                    break;
            }
            _data.Save();
        }

        public void IncreaseDebt(DebtType type)
        {
            switch (type)
            {
                case DebtType.Food:
                    _data.Data.DaysWithoutFood = _data.Data.DaysWithoutFood + 1;
                    break;
                case DebtType.Heat:
                    _data.Data.DaysWithoutHeat = _data.Data.DaysWithoutHeat + 1;
                    break;
                case DebtType.Meds:
                    _data.Data.DaysWithoutMeds = _data.Data.DaysWithoutMeds + 1;
                    break;
                case DebtType.Friends:
                    _data.Data.DaysUnpaidDebtFriends = _data.Data.DaysUnpaidDebtFriends + 1;
                    break;
                case DebtType.Bank:
                    _data.Data.DaysUnpaidBank = _data.Data.DaysUnpaidBank + 1;
                    break;
                case DebtType.Microloan:
                    _data.Data.DaysUnpaidMicroloan = _data.Data.DaysUnpaidMicroloan + 1;
                    if (_data.Data.DaysUnpaidMicroloan > 2f)
                    {
                        _data.Data.Microloan = _data.Data.Microloan * 1.25f;
                    }
                    break;
                case DebtType.Car:
                    _data.Data.DaysUnpaidCarDebt = _data.Data.DaysUnpaidCarDebt + 1;
                    if (_data.Data.DaysUnpaidCarDebt > 4)
                    {
                        _data.Data.CarDebt = 0f;
                        _data.Data.HasACar = false;
                        _data.Data.CarIsReturnable = false;
                    }
                    break;
            }

            UpdateDebts();
        }

        private void UpdateDebts()
        {
            _data.Data.Debts = _data.Data.DebtFriends + _data.Data.BankDebt + _data.Data.Microloan + _data.Data.CarDebt;
            _data.Save();
        }
    }

    public enum DebtType
    {
        Food,
        Heat,
        Meds,
        Friends,
        Bank,
        Microloan,
        Car,
    }
}
