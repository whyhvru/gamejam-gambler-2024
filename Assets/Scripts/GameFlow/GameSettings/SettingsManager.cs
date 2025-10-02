using System;
using System.IO;
using UnityEngine;

public sealed class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    private const string FileName = "Settings.json";
    private string _saveFilePath;

    public GameSettings Settings { get; private set; }

    // События
    public event Action OnSettingsLoaded;
    public event Action OnSettingsSaved;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _saveFilePath = Path.Combine(Application.persistentDataPath, FileName);
            Load();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnApplicationQuit() => Save();

    public void Save()
    {
        Settings ??= new GameSettings();

        string json = JsonUtility.ToJson(Settings, true);
        File.WriteAllText(_saveFilePath, json);

        OnSettingsSaved?.Invoke();
    }

    public void Load()
    {
        if (File.Exists(_saveFilePath))
        {
            string json = File.ReadAllText(_saveFilePath);
            Settings = JsonUtility.FromJson<GameSettings>(json);

            Settings ??= new GameSettings();
        }
        else
        {
            Settings = new GameSettings();
            Save(); // создаём дефолтные настройки сразу
        }

        OnSettingsLoaded?.Invoke();
    }

    public void ResetToDefaults()
    {
        Settings = new GameSettings();
        Save();
    }
}