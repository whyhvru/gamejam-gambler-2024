using UnityEngine;
using TMPro;
using System.Collections;
using Module.Core;

public class StatusManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _balanceText;
    [SerializeField] private TextMeshProUGUI _debtText;

    private float _currentBalance;
    private float _preBalance;
    private float _currentDebt;
    private float _preDebt;

    private IDataService _dataService;

    private void Awake()
    {
        _dataService = ServiceRegistry.Get<IDataService>();
    }

    private void Start()
    {
        _currentBalance = _dataService.Data.Balance;
        _currentDebt = _dataService.Data.Debts;

        UpdateUI();
    }

    private void Update()
    {
        if (_currentBalance != _dataService.Data.Balance)
        {
            _preBalance = _currentBalance;
            _currentBalance = _dataService.Data.Balance;

            UpdateUIWithScrolling(_balanceText, _preBalance, _currentBalance);
        }

        if (_currentDebt != _dataService.Data.Debts)
        {
            _preDebt = _currentDebt;
            _currentDebt = _dataService.Data.Debts;

            UpdateUIWithScrolling(_debtText, _preDebt, _currentDebt);
        }
    }

    private void UpdateUI()
    {
        _balanceText.text = FormatBalance(_currentBalance);
        _debtText.text = FormatBalance(_currentDebt);
    }

    private void UpdateUIWithScrolling(TextMeshProUGUI text, float start, float target)
    {
        StartCoroutine(ScrollingUpdate(text, start, target));
    }

    private IEnumerator ScrollingUpdate(TextMeshProUGUI text, float start, float target)
    {
        float duration = 1f;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            float newValue = Mathf.Lerp(start, target, timeElapsed / duration);
            text.text = FormatBalance(newValue);
            yield return null;
        }

        text.text = FormatBalance(target);
    }

    private string FormatBalance(float amount)
    {
        if (amount >= 1_000_000_000)
        {
            return string.Format("${0:0.##}ggg", amount / 1_000_000_000);
        }
        else if (amount >= 1_000_000)
        {
            return string.Format("${0:0.##}kk", amount / 1_000_000);
        }
        else if (amount >= 1_000)
        {
            return string.Format("${0:0.##}k", amount / 1_000);
        }
        else
        {
            return string.Format("${0:0.00}", amount);  // Оставляем два знака после запятой
        }
    }
}
