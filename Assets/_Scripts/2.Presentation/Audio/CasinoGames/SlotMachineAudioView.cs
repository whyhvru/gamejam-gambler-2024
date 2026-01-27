using UnityEngine;

namespace Module.Presentation.Audio
{
    public sealed class SlotMachineAudioView : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;

        [Header("Clips")]
        [SerializeField] private AudioClip _spinSound;
        [SerializeField] private AudioClip _winSound;
        [SerializeField] private AudioClip _jackpot1Sound;
        [SerializeField] private AudioClip _jackpot2Sound;
        [SerializeField] private AudioClip _jackpot3Sound;

        [SerializeField, Range(0f, 1f)] private float _oneShotVolume = 0.5f;

        public void PlaySpinSound() => Play(_spinSound);

        public void PlaySmallWinSound() => Play(_winSound, 0.5f);
        public void PlayBigWinSound() => Play(_winSound, 1f);
        public void PlayJackpot1Sound() => Play(_jackpot1Sound);
        public void PlayJackpot2Sound() => Play(_jackpot2Sound);
        public void PlayJackpot3Sound() => Play(_jackpot3Sound);

        public void PlayWinByMultiplier(int multiplier)
        {
            if (multiplier <= 0) return;

            if (multiplier >= 10)
            {
                PlayRandomJackpot();
                return;
            }

            if (multiplier >= 2)
            {
                PlayBigWinSound();
                return;
            }

            PlaySmallWinSound();
        }

        private void PlayRandomJackpot()
        {
            int roll = Random.Range(0, 3);
            switch (roll)
            {
                case 0: PlayJackpot1Sound(); break;
                case 1: PlayJackpot2Sound(); break;
                default: PlayJackpot3Sound(); break;
            }
        }

        private void Play(AudioClip clip, float volume = 0f)
        {
            if (_audioSource == null) return;
            if (clip == null) return;

            if (volume <= 0f)
                _audioSource.PlayOneShot(clip, _oneShotVolume);
            else
                _audioSource.PlayOneShot(clip, volume);
        }
    }
}
