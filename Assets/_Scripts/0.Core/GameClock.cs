using System;
using UnityEngine;
using Zenject;

namespace Module.Core
{
    public sealed class GameClock : IInitializable, ITickable
    {
        private float _accumulator;
        private bool _initialized;

        public int Hours { get; private set; }
        public int Minutes { get; private set; }
        public bool IsActive { get; private set; }

        public event Action<int, int> OnTimeChanged;
        public event Action OnNewDay;
        public event Action OnEndOfDayReached;

        public void Initialize()
        {
            Hours = ClampHour(GameClockConfig.startHour);
            Minutes = ClampMinute(GameClockConfig.startMinute);

            _accumulator = 0f;
            IsActive = true;
            _initialized = true;

            OnTimeChanged?.Invoke(Hours, Minutes);
        }

        public void Tick()
        {
            if (!_initialized) return;
            if (!IsActive) return;

            float interval = Mathf.Max(0.01f, GameClockConfig.updateIntervalSeconds);
            _accumulator += Time.deltaTime;

            while (_accumulator >= interval && IsActive)
            {
                _accumulator -= interval;
                AdvanceMinutesInternal(GameClockConfig.minutesPerTick);
            }
        }

        public void Stop() => IsActive = false;

        public void SkipMinutes(int minutesToSkip)
        {
            if (!IsActive) return;
            if (minutesToSkip <= 0) return;

            AdvanceMinutesInternal(minutesToSkip);
        }

        private void AdvanceMinutesInternal(int minutesToAdd)
        {
            if (minutesToAdd <= 0) return;

            Minutes += minutesToAdd;

            while (Minutes >= 60)
            {
                Minutes -= 60;
                Hours++;
            }

            if (Hours >= 24)
            {
                Hours %= 24;
                OnNewDay?.Invoke();
            }

            if (Hours == ClampHour(GameClockConfig.endOfDayHour))
            {
                IsActive = false;
                OnEndOfDayReached?.Invoke();
                return;
            }

            OnTimeChanged?.Invoke(Hours, Minutes);
        }

        private static int ClampHour(int hour)
        {
            if (hour < 0) return 0;
            if (hour > 23) return 23;
            return hour;
        }

        private static int ClampMinute(int minute)
        {
            if (minute < 0) return 0;
            if (minute > 59) return 59;
            return minute;
        }
    }
}
