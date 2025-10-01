using System.IO;
using Module.Core;
using UnityEngine;

namespace Module.Application
{
    public class DataService : IDataService
    {
        private readonly string _saveFilePath;
        public SaveData Data { get; private set; }

        public DataService()
        {
            _saveFilePath = Path.Combine(UnityEngine.Application.persistentDataPath, "SaveData.json");
        }

        public void Save()
        {
            string json = JsonUtility.ToJson(Data, true);
            File.WriteAllText(_saveFilePath, json);
        }

        public void LoadNewGame()
        {
            if (File.Exists(_saveFilePath)) File.Delete(_saveFilePath);
            Load();
        }

        public void Load()
        {
            if (File.Exists(_saveFilePath))
            {
                string json = File.ReadAllText(_saveFilePath);
                Data = JsonUtility.FromJson<SaveData>(json);
            }
            else
            {
                Data = new SaveData();
                Save();
            }
        }
    }
}