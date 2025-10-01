using Module.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Module.Presentation.UI
{
    public sealed class VolumeUI : MonoBehaviour
    {
        [SerializeField] private Slider _slider;

        private ISettingsService _settings;

        private void Awake()
        {
            _settings = ServiceRegistry.Get<ISettingsService>();
        }

        private void Start()
        {
            _slider.value = _settings.Volume;
            _slider.onValueChanged.AddListener(OnVolumeChanged);
        }

        private void OnDestroy()
        {
            _slider.onValueChanged.RemoveListener(OnVolumeChanged);
        }

        private void OnVolumeChanged(float value)
        {
            _settings.Volume = value;
        }
    }
}
