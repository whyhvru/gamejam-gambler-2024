using UnityEngine;

namespace Module.Presentation.Audio
{
    public static class AudioPrefs
    {
        private const string VolumeKey = "audio.volume";
        private const float DefaultVolume = 1f;

        public static float Volume
        {
            get => Mathf.Clamp01(PlayerPrefs.GetFloat(VolumeKey, DefaultVolume));
            set
            {
                float v = Mathf.Clamp01(value);
                PlayerPrefs.SetFloat(VolumeKey, v);
                PlayerPrefs.Save();
            }
        }
    }
}