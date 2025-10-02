using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private Slider _volumeSlider;

    private AudioSource _audioSource;
    private SettingsManager _settingsManager;

    private void Awake()
    {
        _settingsManager = SettingsManager.Instance;
        _audioSource = MusicManager.Instance.GetComponent<AudioSource>();

        if (_volumeSlider == null)
            _volumeSlider = GetComponent<Slider>();
    }

    private void Start()
    {
        float savedVolume = _settingsManager.Settings.volume;

        ApplyVolume(savedVolume);
        _volumeSlider.value = savedVolume;

        _volumeSlider.onValueChanged.AddListener(ApplyVolume);
    }

    private void OnDestroy() => _volumeSlider.onValueChanged.RemoveListener(ApplyVolume);

    private void ApplyVolume(float value)
    {
        if (_audioSource == null) return;

        _audioSource.volume = value;
        _settingsManager.Settings.volume = value;
        _settingsManager.Save();
    }
}