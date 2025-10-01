using Module.Core;
using UnityEngine;
using UnityEngine.Audio;

namespace Module.Application
{
    public sealed class SettingsService : ISettingsService
    {
        private const string VolumeKey = nameof(VolumeKey);
        private readonly AudioMixer _audioMixer;

        private float _volume;

        public float Volume
        {
            get => _volume;
            set
            {
                if (Mathf.Approximately(_volume, value))
                    return;

                _volume = Mathf.Clamp01(value);
                ApplyVolume(_volume);
                PlayerPrefs.SetFloat(VolumeKey, _volume);
                PlayerPrefs.Save();
            }
        }

        public SettingsService(AudioMixer audioMixer)
        {
            _audioMixer = audioMixer;
            Load();
        }

        private void Load()
        {
            _volume = PlayerPrefs.HasKey(VolumeKey)
                ? PlayerPrefs.GetFloat(VolumeKey)
                : 1.0f;

            ApplyVolume(_volume);
        }

        private void ApplyVolume(float value)
        {
            float db = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f;
            _audioMixer.SetFloat(VolumeKey, db);
        }
    }
}