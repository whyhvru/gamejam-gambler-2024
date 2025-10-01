using System.Collections.Generic;
using Module.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

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
        _audioSource.volume = ServiceRegistry.Get<ISettingsService>().Volume;

        SceneManager.sceneLoaded += OnSceneLoaded;

        _casinoPlaylist = new Queue<AudioClip>(new[] { _casino1Music, _casino2Music });
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _currentScene = scene.name;
        _audioSource.volume = ServiceRegistry.Get<ISettingsService>().Volume;
        UpdateMusic();
    }

    public void OnWindowChanged(string windowName)
    {
        if (_currentScene != "Game") return;

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
            case "Menu":
            case "Tenet":
                PlayMusic(_endDayMusic);
                break;
            case "Game":
                PlayMusic(_mapMusic);
                break;
            case "End":
                PlayMusic(_endMusic);
                break;
        }
    }

    private void PlayMusic(AudioClip clip)
    {
        if (_audioSource.clip == clip && _audioSource.isPlaying) return;

        _audioSource.Stop();
        _audioSource.clip = clip;
        _audioSource.loop = true;
        _audioSource.Play();
    }

    private void PlayCasinoMusic()
    {
        if (_audioSource.isPlaying && _casinoPlaylist.Contains(_audioSource.clip)) return;

        PlayNextCasinoTrack();
    }

    private void PlayNextCasinoTrack()
    {
        if (_casinoPlaylist.Count == 0) return;

        _audioSource.Stop();
        var nextTrack = _casinoPlaylist.Dequeue();
        _audioSource.clip = nextTrack;
        _audioSource.loop = false;
        _audioSource.Play();

        _casinoPlaylist.Enqueue(nextTrack);

        Invoke(nameof(PlayNextCasinoTrack), _audioSource.clip.length);
    }
}
