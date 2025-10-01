using TMPro;
using UnityEngine;
using Module.Core;

namespace Module.Presentation.UI
{
    public sealed class DateTimeUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _dateText;
        [SerializeField] private TextMeshProUGUI _timeText;

        private IGameTimeService _timeService;

        private void Awake()
        {
            _timeService = ServiceRegistry.Get<IGameTimeService>();
        }

        private void OnEnable()
        {
            _timeService.OnDateChanged += UpdateDate;
            _timeService.OnTimeChanged += UpdateTime;
        }

        private void OnDisable()
        {
            _timeService.OnDateChanged -= UpdateDate;
            _timeService.OnTimeChanged -= UpdateTime;
        }

        private void UpdateDate(int day, int month, int year)
        {
            _dateText.text = $"{day:D2}.{month:D2}.{year}";
        }

        private void UpdateTime(int hours, int minutes)
        {
            _timeText.text = $"{hours:D2}:{minutes:D2}";
        }
    }
}