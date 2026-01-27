using Module.Core;
using TMPro;
using UnityEngine;
using Zenject;

namespace Module.Presentation.UI
{
    public sealed class GameTimeView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _timeText;
        [SerializeField] private float _maxDebt = 100000f;

        private IDataService _dataService;
        private IAppFlowService _appFlowService;
        private GameClock _clock;

        [Inject]
        public void Construct(IDataService data, IAppFlowService appFlow, GameClock clock)
        {
            _dataService = data;
            _appFlowService = appFlow;
            _clock = clock;
        }

        private void Awake()
        {
            _clock.OnTimeChanged += HandleTimeChanged;
            _clock.OnNewDay += HandleNewDay;
            _clock.OnEndOfDayReached += HandleEndOfDayReached;
        }

        private void OnEnable() => HandleTimeChanged(_clock.Hours, _clock.Minutes);
        public void SkipTime(int minutesToSkip) => _clock.SkipMinutes(minutesToSkip);

        private void HandleTimeChanged(int hours, int minutes)
        {
            if (_timeText != null)
                _timeText.text = $"{hours:D2}:{minutes:D2}";
        }

        private void HandleNewDay() => _dataService.NewDay();

        private void HandleEndOfDayReached()
        {
            _dataService.Save();

            if (IsGameOver())
                _appFlowService.LoadEnd();
            else
                _appFlowService.LoadTenet();
        }

        private bool IsGameOver()
        {
            var save = _dataService.SaveData;

            bool aLotOfDebt = save.debts >= _maxDebt;
            bool everyoneDead = !save.motherIsAlive &&
                                !save.wifeIsAlive &&
                                !save.childIsAlive &&
                                !save.child2IsAlive;

            return aLotOfDebt && everyoneDead;
        }

        private void OnDestroy()
        {
            if (_clock == null) return;

            _clock.OnTimeChanged -= HandleTimeChanged;
            _clock.OnNewDay -= HandleNewDay;
            _clock.OnEndOfDayReached -= HandleEndOfDayReached;
        }
    }
}