using System;
using UnityEngine.SceneManagement;

namespace Module.Core
{
    public interface ISceneLoaderService
    {
        public event Action<string> SceneLoaded;
        public void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single);
    }
}