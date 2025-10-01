using System;
using Module.Core;

namespace Module.Gameplay
{
    public sealed class BalanceService
    {
        private readonly IDataService _data;
        public event Action<float> OnBalanceChanged;

        public BalanceService(IDataService data)
        {
            _data = data;
        }

        public void ChangeBalance(float value)
        {
            _data.Data.Balance += value;
            (value > 0 ? ref _data.Data.DailyIncome : ref _data.Data.DailyLesion) += value;
            _data.Save();
        }
    }
}