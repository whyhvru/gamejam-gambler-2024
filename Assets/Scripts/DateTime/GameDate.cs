using UnityEngine;
using TMPro;

public class GameDate : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _dateText;
    private int _day;
    private int _month;
    private int _year = 24;

    private void Start()
    {
        _day = DataManager.Instance.SaveData.Day;
        _month = DataManager.Instance.SaveData.Month;

        UpdateUI();
    }

    private void Update()
    {
        if ( _day != DataManager.Instance.SaveData.Day ||
            _month != DataManager.Instance.SaveData.Month )
        {
            _day = DataManager.Instance.SaveData.Day;
            _month = DataManager.Instance.SaveData.Month;

            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        _dateText.text = $"{_day:D2}.{_month:D2}.{_year}";
    }
}