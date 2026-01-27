using Module.Core;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class SleepButton : MonoBehaviour
{
    [SerializeField] private Button _sleepButton;

    private IDataService _dataService;
    private IAppFlowService _appFlowService;

    [Inject]
    public void Construct(IDataService data, IAppFlowService appFlow)
    {
        _dataService = data;
        _appFlowService = appFlow;
    }

    private void Start() => _sleepButton.onClick.AddListener(Sleep);

    private void Sleep()
    {
        ClearDailyStats();
        _appFlowService.LoadGame();
    }

    private void ClearDailyStats()
    {
        _dataService.SaveData.dailyIncome = 0;
        _dataService.SaveData.dailyLesion = 0;
    }
}