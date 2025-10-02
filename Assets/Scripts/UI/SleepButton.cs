using UnityEngine;
using UnityEngine.UI;

public class SleepButton : MonoBehaviour
{
    [SerializeField] private Button _sleepButton;

    private void Start() => _sleepButton.onClick.AddListener(Sleep);

    private void Sleep()
    {
        ClearDailyStats();
        SceneLoader.Instance.LoadGame();
    }

    private void ClearDailyStats()
    {
        DataManager.Instance.SaveData.dailyIncome = 0;
        DataManager.Instance.SaveData.dailyLesion = 0;
    }
}