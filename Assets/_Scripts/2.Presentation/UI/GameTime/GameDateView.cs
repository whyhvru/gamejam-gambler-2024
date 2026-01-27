using Module.Core;
using TMPro;
using UnityEngine;
using Zenject;

namespace Module.Presentation.UI
{
    public class GameDateView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _dateText;

        private IDataService _dataService;

        [Inject]
        public void Construct(IDataService data) => _dataService = data;

        private void Awake()
        {
            _dataService.OnDateChanged += HandleDateChanged;
            HandleDateChanged();
        }

        private void HandleDateChanged()
        {
            var save = _dataService.SaveData;
            int year = 24;

            _dateText.text = $"{save.day:D2}.{save.month:D2}.{year}";
        }

        private void OnDestroy() => _dataService.OnDateChanged -= HandleDateChanged;
    }
}