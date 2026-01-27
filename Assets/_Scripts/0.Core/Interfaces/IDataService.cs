using System;

namespace Module.Core
{
    public interface IDataService
    {
        SaveData SaveData { get; }

        event Action OnDataChanged;
        event Action OnDateChanged;

        void Save();
        void Load();

        void NewDay();

        void AddDebt(float value);
        void ChangeBalance(float value);

        void ResetDebt(string expenseName);
        void IncreaseDebt(string expenseName);
    }
}