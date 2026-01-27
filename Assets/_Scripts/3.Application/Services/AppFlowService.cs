using Module.Core;
using UnityEngine.SceneManagement;

namespace Module.Application
{
    public sealed class AppFlowService : IAppFlowService
    {
        public void LoadMenu() => StartScene(0);
        public void LoadGame() => StartScene(1);
        public void LoadTenet() => StartScene(2);
        public void LoadEnd() => StartScene(3);

        private void StartScene(int buildIndex) => SceneManager.LoadScene(buildIndex);
    }
}
