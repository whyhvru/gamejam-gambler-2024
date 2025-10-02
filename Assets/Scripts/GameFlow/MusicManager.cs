using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    private const string SceneMenu = "Menu";
    private const string SceneGame = "Game";
    private const string SceneTenet = "Tenet";
    private const string SceneEnd = "End";

    [Header("Music Clips")]
    [SerializeField] private AudioClip _endDayMusic;
    [SerializeField] private AudioClip _mapMusic;
    [SerializeField] private AudioClip _casino1Music;
    [SerializeField] private AudioClip _casino2Music;
    [SerializeField] private AudioClip _endMusic;

    private AudioSource _audioSource;
    private Queue<AudioClip> _casinoPlaylist;
    private string _currentScene;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        _audioSource = GetComponent<AudioSource>();
        _audioSource.volume = SettingsManager.Instance.Settings.volume;

        _casinoPlaylist = new Queue<AudioClip>();
        if (_casino1Music != null) _casinoPlaylist.Enqueue(_casino1Music);
        if (_casino2Music != null) _casinoPlaylist.Enqueue(_casino2Music);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        CancelInvoke(nameof(PlayNextCasinoTrack));
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _currentScene = scene.name;
        _audioSource.volume = SettingsManager.Instance.Settings.volume;
        UpdateMusic();
    }

    public void OnWindowChanged(string windowName)
    {
        if (_currentScene != SceneGame) return;

        switch (windowName)
        {
            case "MapPanel":
                PlayMusic(_mapMusic);
                break;

            case "RocketPanel":
            case "SlotPanel":
                PlayCasinoMusic();
                break;
        }
    }

    private void UpdateMusic()
    {
        switch (_currentScene)
        {
            case SceneMenu:
            case SceneTenet:
                PlayMusic(_endDayMusic);
                break;

            case SceneGame:
                PlayMusic(_mapMusic);
                break;

            case SceneEnd:
                PlayMusic(_endMusic);
                break;
        }
    }

    private void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        if (_audioSource.clip == clip && _audioSource.isPlaying) return;

        SetClipAndPlay(clip, loop: true);
    }

    private void PlayCasinoMusic()
    {
        if (_audioSource.isPlaying && _casinoPlaylist.Contains(_audioSource.clip)) return;
        PlayNextCasinoTrack();
    }

    private void PlayNextCasinoTrack()
    {
        if (_casinoPlaylist.Count == 0) return;

        var nextTrack = _casinoPlaylist.Dequeue();
        _casinoPlaylist.Enqueue(nextTrack);

        SetClipAndPlay(nextTrack, loop: false);

        Invoke(nameof(PlayNextCasinoTrack), nextTrack.length);
    }

    private void SetClipAndPlay(AudioClip clip, bool loop)
    {
        _audioSource.Stop();
        _audioSource.clip = clip;
        _audioSource.loop = loop;
        _audioSource.Play();
    }
}