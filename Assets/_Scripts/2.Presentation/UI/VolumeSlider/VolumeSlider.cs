using Module.Presentation.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace Module.Presentation.UI
{
    [RequireComponent(typeof(Slider))]
    public sealed class VolumeSlider : MonoBehaviour
    {
        [SerializeField] private Slider _volumeSlider;

        private void Awake() => _volumeSlider = GetComponent<Slider>();

        private void Start()
        {
            float savedVolume = AudioPrefs.Volume;

            ApplyVolume(savedVolume);
            _volumeSlider.SetValueWithoutNotify(savedVolume);

            _volumeSlider.onValueChanged.AddListener(ApplyVolume);
        }

        private void ApplyVolume(float value)
        {
            value = Mathf.Clamp01(value);

            AudioListener.volume = value;
            AudioPrefs.Volume = value;
        }

        private void OnDestroy() => _volumeSlider.onValueChanged.RemoveListener(ApplyVolume);
    }
}