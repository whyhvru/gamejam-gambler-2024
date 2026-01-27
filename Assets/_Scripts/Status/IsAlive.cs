using Module.Core;
using UnityEngine;
using Zenject;

public class IsAlive : MonoBehaviour
{
    [SerializeField] private GameObject _mother;
    [SerializeField] private GameObject _child1;
    [SerializeField] private GameObject _child2;
    [SerializeField] private GameObject _wife;
    [SerializeField] private MessageManager _messageManager;

    private IDataService _dataService;

    [Inject]
    public void Construct(IDataService data) => _dataService = data;

    private void Start() => CheckAndUpdateDeaths();

    private void CheckAndUpdateDeaths()
    {
        HandleDeaths();
        UpdateActiveStatus();
    }

    private void HandleDeaths()
    {
        if (_dataService.SaveData.daysWithoutFood > 2 || _dataService.SaveData.daysWithoutHeat > 2)
        {
            if (_dataService.SaveData.childIsAlive)
            {
                _dataService.SaveData.childIsAlive = false;
                _messageManager.AddDeathMessage("ChildsDeath");
            }
            else if (_dataService.SaveData.child2IsAlive)
            {
                _dataService.SaveData.child2IsAlive = false;
                _messageManager.AddDeathMessage("Childs2Death");
            }
            else if (_dataService.SaveData.wifeIsAlive)
            {
                _dataService.SaveData.wifeIsAlive = false;
                _messageManager.AddDeathMessage("WifesDeath");
            }
            else if (_dataService.SaveData.motherIsAlive)
            {
                _dataService.SaveData.motherIsAlive = false;
                _messageManager.AddDeathMessage("MothersDeath");
            }

            ResetSurvivalCounters();
        }

        if (_dataService.SaveData.daysWithoutMeds > 1)
        {
            if (_dataService.SaveData.motherIsAlive)
            {
                _dataService.SaveData.motherIsAlive = false;
                _messageManager.AddDeathMessage("MothersDeath");
            }
        }

        _dataService.Save();
    }

    private void ResetSurvivalCounters()
    {
        _dataService.SaveData.daysWithoutFood = 0;
        _dataService.SaveData.daysWithoutHeat = 0;
    }

    private void UpdateActiveStatus()
    {
        UpdateGameObjectStatus(_mother, _dataService.SaveData.motherIsAlive);
        UpdateGameObjectStatus(_wife, _dataService.SaveData.wifeIsAlive);
        UpdateGameObjectStatus(_child1, _dataService.SaveData.childIsAlive);
        UpdateGameObjectStatus(_child2, _dataService.SaveData.child2IsAlive);
    }

    private void UpdateGameObjectStatus(GameObject obj, bool isAlive)
    {
        if (obj != null)
        {
            obj.SetActive(isAlive);
        }
    }
}
