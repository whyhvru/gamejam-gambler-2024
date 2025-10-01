using System;

namespace Module.Core
{
    public interface IGameTimeService
    {
        event Action<int, int, int> OnDateChanged;
        event Action<int, int> OnTimeChanged;
        event Action OnDayEnded;

        void SkipTime(int minutes);
        void StartDay();
    }
}