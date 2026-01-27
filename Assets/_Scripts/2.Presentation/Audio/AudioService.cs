using System;
using System.Collections.Generic;
using Module.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Module.Presentation.Audio
{
    public sealed class AudioService : IAudioService, IInitializable, IDisposable
    {
        private const string SceneMenu = "Menu";
        private const string SceneGame = "Game";
        private const string SceneTenet = "Tenet";
        private const string SceneEnd = "End";

        private const string WindowMap = "MapPanel";
        private const string WindowRocket = "RocketPanel";
        private const string WindowSlot = "SlotPanel";

        private readonly AudioPlayer _player;
        private readonly AudioConfigSO _config;

        private readonly Queue<AudioClip> _casinoQueue = new();
        private string _currentScene;

        public AudioService(AudioPlayer player, AudioConfigSO config)
        {
            _player = player;
            _config = config;
        }

        public void Initialize()
        {
            BuildCasinoQueue();

            SceneManager.sceneLoaded += OnSceneLoaded;

            _currentScene = SceneManager.GetActiveScene().name;
            UpdateMusicForScene();
        }

        public void Dispose()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            _player.CancelScheduled();
        }

        public void OnWindowChanged(string windowName)
        {
            if (_currentScene != SceneGame) return;

            switch (windowName)
            {
                case WindowMap:
                    _player.PlayLoop(_config.GameMusic);
                    break;

                case WindowRocket:
                case WindowSlot:
                    PlayCasinoMusic();
                    break;
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            _currentScene = scene.name;
            UpdateMusicForScene();
        }

        private void UpdateMusicForScene()
        {
            switch (_currentScene)
            {
                case SceneMenu:
                    _player.PlayLoop(_config.MenuMusic);
                    break;

                case SceneGame:
                    _player.PlayLoop(_config.GameMusic);
                    break;

                case SceneTenet:
                    _player.PlayLoop(_config.TenetMusic);
                    break;

                case SceneEnd:
                    _player.PlayLoop(_config.EndMusic);
                    break;
            }
        }

        private void BuildCasinoQueue()
        {
            _casinoQueue.Clear();

            var list = _config.CasinoPlaylist;
            if (list == null) return;

            for (int i = 0; i < list.Length; i++)
            {
                if (list[i] != null)
                    _casinoQueue.Enqueue(list[i]);
            }
        }

        private void PlayCasinoMusic()
        {
            if (_casinoQueue.Count == 0) return;

            var current = _player.MusicSource.clip;
            foreach (var clip in _casinoQueue)
            {
                if (clip == current && _player.MusicSource.isPlaying)
                    return;
            }

            PlayNextCasinoTrack();
        }

        private void PlayNextCasinoTrack()
        {
            if (_casinoQueue.Count == 0) return;

            var next = _casinoQueue.Dequeue();
            _casinoQueue.Enqueue(next);

            _player.MusicSource.Stop();
            _player.MusicSource.clip = next;
            _player.MusicSource.loop = false;
            _player.MusicSource.Play();

            _player.PlayNonLoopThen(PlayNextCasinoTrack, next.length);
        }
    }
}
