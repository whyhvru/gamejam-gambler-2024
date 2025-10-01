using System;
using Module.Core;
using UnityEngine.SceneManagement;

namespace Module.Application
{
    public sealed class SceneLoaderService : ISceneLoaderService
    {
        public event Action<string> SceneLoaded;

        public void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene(sceneName, mode);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneLoaded?.Invoke(scene.name);
        }
    }
}