namespace Module.Core
{
    public interface IGameFlowService
    {
        void LoadMainMenu();
        void LoadNewGame();
        void LoadGame();
        void LoadResult();
        void LoadGameOver();
        void QuitGame();
    }
}