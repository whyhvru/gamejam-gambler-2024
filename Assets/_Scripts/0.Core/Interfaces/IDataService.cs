namespace Module.Core
{
    public interface IDataService
    {
        SaveData Data { get; }
        void Save();
        void LoadNewGame();
        void Load();
    }
}
