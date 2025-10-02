using UnityEngine;
using TMPro;

public class GameDate : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _dateText;

    private DataManager _dataManager;

    private void Awake() => _dataManager = DataManager.Instance;

    private void OnEnable()
    {
        _dataManager.OnDateChanged += HandleDateChanged;
        HandleDateChanged();
    }

    private void OnDisable() => _dataManager.OnDateChanged -= HandleDateChanged;

    private void HandleDateChanged()
    {
        var save = _dataManager.SaveData;
        _dateText.text = $"{save.day:D2}.{save.month:D2}.{24}";
    }
}