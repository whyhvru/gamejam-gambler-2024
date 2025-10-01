using System.IO;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }
    private string _saveFilePath;
    private GameSettings _settings;

    public GameSettings Settings => _settings;

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
        }

        _saveFilePath = Path.Combine(Application.persistentDataPath, "Settings.json");
        Load();
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(_settings, true);
        File.WriteAllText(_saveFilePath, json);
    }

    public void Load()
    {
        if (File.Exists(_saveFilePath))
        {
            string json = File.ReadAllText(_saveFilePath);
            _settings = JsonUtility.FromJson<GameSettings>(json);
        }
        else
        {
            _settings = new GameSettings();
        }
    }
}
