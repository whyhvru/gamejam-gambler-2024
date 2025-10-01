using Module.Core;
using Module.Presentation.UI;
using UnityEngine;

public class IsAlive : MonoBehaviour
{
    [SerializeField] private GameObject _mother;
    [SerializeField] private GameObject _child1;
    [SerializeField] private GameObject _child2;
    [SerializeField] private GameObject _wife;
    [SerializeField] private MessageManager _messageManager;

    private IDataService _dataService;

    private void Awake()
    {
        _dataService = ServiceRegistry.Get<IDataService>();
    }

    private void Start()
    {
        CheckAndUpdateDeaths();
    }

    private void CheckAndUpdateDeaths()
    {
        HandleDeaths();
        UpdateActiveStatus();
    }

    private void HandleDeaths()
    {
        if (_dataService.Data.DaysWithoutFood > 2 || _dataService.Data.DaysWithoutHeat > 2)
        {
            if (_dataService.Data.ChildIsAlive)
            {
                _dataService.Data.ChildIsAlive = false;
                _messageManager.AddDeathMessage("ChildsDeath");
            }
            else if (_dataService.Data.Child2IsAlive)
            {
                _dataService.Data.Child2IsAlive = false;
                _messageManager.AddDeathMessage("Childs2Death");
            }
            else if (_dataService.Data.WifeIsAlive)
            {
                _dataService.Data.WifeIsAlive = false;
                _messageManager.AddDeathMessage("WifesDeath");
            }
            else if (_dataService.Data.MotherIsAlive)
            {
                _dataService.Data.MotherIsAlive = false;
                _messageManager.AddDeathMessage("MothersDeath");
            }

            ResetSurvivalCounters();
        }

        if (_dataService.Data.DaysWithoutMeds > 1)
        {
            if (_dataService.Data.MotherIsAlive)
            {
                _dataService.Data.MotherIsAlive = false;
                _messageManager.AddDeathMessage("MothersDeath");
            }
        }

        _dataService.Save();
    }

    private void ResetSurvivalCounters()
    {
        _dataService.Data.DaysWithoutFood = 0;
        _dataService.Data.DaysWithoutHeat = 0;
    }

    private void UpdateActiveStatus()
    {
        UpdateGameObjectStatus(_mother, _dataService.Data.MotherIsAlive);
        UpdateGameObjectStatus(_wife, _dataService.Data.WifeIsAlive);
        UpdateGameObjectStatus(_child1, _dataService.Data.ChildIsAlive);
        UpdateGameObjectStatus(_child2, _dataService.Data.Child2IsAlive);
    }

    private void UpdateGameObjectStatus(GameObject obj, bool isAlive)
    {
        if (obj != null)
        {
            obj.SetActive(isAlive);
        }
    }
}
