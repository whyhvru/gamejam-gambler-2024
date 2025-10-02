using UnityEngine;

public class IsAlive : MonoBehaviour
{
    [SerializeField] private GameObject _mother;
    [SerializeField] private GameObject _child1;
    [SerializeField] private GameObject _child2;
    [SerializeField] private GameObject _wife;
    [SerializeField] private MessageManager _messageManager;

    private DataManager _dataManager;

    private void Awake() => _dataManager = DataManager.Instance;
    private void Start() => CheckAndUpdateDeaths();

    private void CheckAndUpdateDeaths()
    {
        HandleDeaths();
        UpdateActiveStatus();
    }

    private void HandleDeaths()
    {
        if (_dataManager.SaveData.daysWithoutFood > 2 || _dataManager.SaveData.daysWithoutHeat > 2)
        {
            if (_dataManager.SaveData.childIsAlive)
            {
                _dataManager.SaveData.childIsAlive = false;
                _messageManager.AddDeathMessage("ChildsDeath");
            }
            else if (_dataManager.SaveData.child2IsAlive)
            {
                _dataManager.SaveData.child2IsAlive = false;
                _messageManager.AddDeathMessage("Childs2Death");
            }
            else if (_dataManager.SaveData.wifeIsAlive)
            {
                _dataManager.SaveData.wifeIsAlive = false;
                _messageManager.AddDeathMessage("WifesDeath");
            }
            else if (_dataManager.SaveData.motherIsAlive)
            {
                _dataManager.SaveData.motherIsAlive = false;
                _messageManager.AddDeathMessage("MothersDeath");
            }

            ResetSurvivalCounters();
        }

        if (_dataManager.SaveData.daysWithoutMeds > 1)
        {
            if (_dataManager.SaveData.motherIsAlive)
            {
                _dataManager.SaveData.motherIsAlive = false;
                _messageManager.AddDeathMessage("MothersDeath");
            }
        }

        _dataManager.Save();
    }

    private void ResetSurvivalCounters()
    {
        _dataManager.SaveData.daysWithoutFood = 0;
        _dataManager.SaveData.daysWithoutHeat = 0;
    }

    private void UpdateActiveStatus()
    {
        UpdateGameObjectStatus(_mother, _dataManager.SaveData.motherIsAlive);
        UpdateGameObjectStatus(_wife, _dataManager.SaveData.wifeIsAlive);
        UpdateGameObjectStatus(_child1, _dataManager.SaveData.childIsAlive);
        UpdateGameObjectStatus(_child2, _dataManager.SaveData.child2IsAlive);
    }

    private void UpdateGameObjectStatus(GameObject obj, bool isAlive)
    {
        if (obj != null)
        {
            obj.SetActive(isAlive);
        }
    }
}
