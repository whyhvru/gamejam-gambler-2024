using UnityEngine;

namespace Module.Presentation.Audio
{
    [CreateAssetMenu(menuName = "Configs/Audio Config", fileName = "AudioConfig")]
    public sealed class AudioConfigSO : ScriptableObject
    {
        [field: Header("Scene Music")]
        [field: SerializeField] public AudioClip MenuMusic { get; private set; }
        [field: SerializeField] public AudioClip GameMusic { get; private set; }
        [field: SerializeField] public AudioClip TenetMusic { get; private set; }
        [field: SerializeField] public AudioClip EndMusic { get; private set; }

        [field: Header("Casino Playlist")]
        [field: SerializeField] public AudioClip[] CasinoPlaylist { get; private set; }
    }
}
