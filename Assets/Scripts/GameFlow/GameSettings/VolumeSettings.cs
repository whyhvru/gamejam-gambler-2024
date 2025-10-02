using UnityEngine;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private Slider _volumeSlider;
    private AudioSource _audioSource;
    private SettingsManager _settingsManager;

    private void Start() 
    {
        _settingsManager = SettingsManager.Instance;
        _audioSource = MusicManager.Instance.GetComponent<AudioSource>();

        if (_volumeSlider != null)
        {
            _volumeSlider.value = _audioSource.volume;
            _volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }
    }

    private void OnVolumeChanged(float value)
    {
        if (_audioSource != null)
        {
            _audioSource.volume = value;
            _settingsManager.Settings.Volume = value;
            _settingsManager.Save();
        }
    }
}