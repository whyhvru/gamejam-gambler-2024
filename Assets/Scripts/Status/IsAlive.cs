using UnityEngine;

public class IsAlive : MonoBehaviour
{
    [SerializeField] private GameObject _mother;
    [SerializeField] private GameObject _child1;
    [SerializeField] private GameObject _child2;
    [SerializeField] private GameObject _wife;
    [SerializeField] private MessageManager _messageManager;

    private DataManager _dataManager;

    private void Awake()
    {
        _dataManager = DataManager.Instance;
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
        if (_dataManager.SaveData.DaysWithoutFood > 2 || _dataManager.SaveData.DaysWithoutHeat > 2)
        {
            if (_dataManager.SaveData.ChildIsAlive)
            {
                _dataManager.SaveData.ChildIsAlive = false;
                _messageManager.AddDeathMessage("ChildsDeath");
            }
            else if (_dataManager.SaveData.Child2IsAlive)
            {
                _dataManager.SaveData.Child2IsAlive = false;
                _messageManager.AddDeathMessage("Childs2Death");
            }
            else if (_dataManager.SaveData.WifeIsAlive)
            {
                _dataManager.SaveData.WifeIsAlive = false;
                _messageManager.AddDeathMessage("WifesDeath");
            }
            else if (_dataManager.SaveData.MotherIsAlive)
            {
                _dataManager.SaveData.MotherIsAlive = false;
                _messageManager.AddDeathMessage("MothersDeath");
            }

            ResetSurvivalCounters();
        }

        if (_dataManager.SaveData.DaysWithoutMeds > 1)
        {
            if (_dataManager.SaveData.MotherIsAlive)
            {
                _dataManager.SaveData.MotherIsAlive = false;
                _messageManager.AddDeathMessage("MothersDeath");
            }
        }

        _dataManager.Save();
    }

    private void ResetSurvivalCounters()
    {
        _dataManager.SaveData.DaysWithoutFood = 0;
        _dataManager.SaveData.DaysWithoutHeat = 0;
    }

    private void UpdateActiveStatus()
    {
        UpdateGameObjectStatus(_mother, _dataManager.SaveData.MotherIsAlive);
        UpdateGameObjectStatus(_wife, _dataManager.SaveData.WifeIsAlive);
        UpdateGameObjectStatus(_child1, _dataManager.SaveData.ChildIsAlive);
        UpdateGameObjectStatus(_child2, _dataManager.SaveData.Child2IsAlive);
    }

    private void UpdateGameObjectStatus(GameObject obj, bool isAlive)
    {
        if (obj != null)
        {
            obj.SetActive(isAlive);
        }
    }
}
