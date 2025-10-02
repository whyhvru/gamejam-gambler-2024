using UnityEngine;

public class SlotMachineSound : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _spinSound;
    [SerializeField] private AudioClip _smallWinSound;
    [SerializeField] private AudioClip _bigWinSound;
    [SerializeField] private AudioClip _jackpot1Sound;
    [SerializeField] private AudioClip _jackpot2Sound;
    [SerializeField] private AudioClip _jackpot3Sound;

    public void PlaySpinSound() => PlaySound(_spinSound);
    public void PlaySmallWinSound() => PlaySound(_smallWinSound);
    public void PlayBigWinSound() => PlaySound(_bigWinSound);
    public void PlayJackpot1Sound() => PlaySound(_jackpot1Sound);
    public void PlayJackpot2Sound() => PlaySound(_jackpot2Sound);
    public void PlayJackpot3Sound() => PlaySound(_jackpot3Sound);

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            _ = SettingsManager.Instance.Settings.volume;
            _audioSource.PlayOneShot(clip, 0.5f);
        }
    }
}
