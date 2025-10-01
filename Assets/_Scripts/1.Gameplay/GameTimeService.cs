using System;
using Module.Core;

namespace Module.Gameplay
{
    public sealed class GameTimeService : IGameTimeService
    {
        public event Action<int, int, int> OnDateChanged;
        public event Action<int, int> OnTimeChanged;
        public event Action OnDayEnded;

        private int _day;
        private int _month;
        private int _year = 24;
        private int _hours = 12;
        private int _minutes = 0;
        private bool _isDayActive;

        private readonly IDataService _dataService;

        public GameTimeService(IDataService dataService)
        {
            _dataService = dataService;
        }

        public void StartDay()
        {
            _day = _dataService.Data.Day;
            _month = _dataService.Data.Month;
            _hours = 12;
            _minutes = 0;
            _isDayActive = true;

            OnDateChanged?.Invoke(_day, _month, _year);
            OnTimeChanged?.Invoke(_hours, _minutes);
        }

        public void SkipTime(int minutes)
        {
            if (!_isDayActive) return;

            AddTime(minutes);
        }

        private void AddTime(int minutesToAdd)
        {
            _minutes += minutesToAdd;

            while (_minutes >= 60)
            {
                _minutes -= 60;
                _hours++;
            }

            if (_hours >= 24)
            {
                _hours = 0;
                _dataService.Data.Day++;
                _dataService.Save();
                OnDateChanged?.Invoke(_day, _month, _year);
            }

            if (_hours == 3)
            {
                _isDayActive = false;
                OnDayEnded?.Invoke();
            }

            OnTimeChanged?.Invoke(_hours, _minutes);
        }
    }
}