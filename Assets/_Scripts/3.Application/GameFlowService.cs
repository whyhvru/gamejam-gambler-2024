using Module.Core;

namespace Module.Application
{
    public class GameFlowService : IGameFlowService
    {
        private readonly IDataService _dataService;
        private readonly ISceneLoaderService _sceneLoaderService;

        public GameFlowService(IDataService dataService, ISceneLoaderService sceneLoaderService)
        {
            _dataService = dataService;
            _sceneLoaderService = sceneLoaderService;
        }

        public void LoadMainMenu()
        {
            _sceneLoaderService.LoadScene("MainMenu");
        }

        public void LoadNewGame()
        {
            _dataService.LoadNewGame();
            _sceneLoaderService.LoadScene("Game");
        }

        public void LoadGame()
        {
            _dataService.Load();
            _sceneLoaderService.LoadScene("Game");
        }

        public void LoadResult()
        {
            _sceneLoaderService.LoadScene("Result");
        }

        public void LoadGameOver()
        {
            _sceneLoaderService.LoadScene("GameOver");
        }

        public void QuitGame()
        {
            UnityEngine.Application.Quit();
        }
    }
}