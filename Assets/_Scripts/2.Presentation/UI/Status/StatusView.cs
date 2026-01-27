using System.Collections;
using Module.Core;
using TMPro;
using UnityEngine;
using Zenject;

namespace Module.Presentation.UI
{
    public sealed class StatusView : MonoBehaviour
    {
        private const float ScrollDuration = 1f;

        [SerializeField] private TextMeshProUGUI _balanceText;
        [SerializeField] private TextMeshProUGUI _debtText;

        private IDataService _data;

        private float _currentBalance;
        private float _currentDebt;

        private Coroutine _balanceRoutine;
        private Coroutine _debtRoutine;

        [Inject]
        public void Construct(IDataService data) => _data = data;

        private void OnEnable()
        {
            _currentBalance = _data.SaveData.balance;
            _currentDebt = _data.SaveData.debts;

            ApplyImmediate();
            _data.OnDataChanged += HandleDataChanged;
        }

        private void OnDisable()
        {
            if (_data != null)
                _data.OnDataChanged -= HandleDataChanged;
        }

        private void HandleDataChanged()
        {
            float newBalance = _data.SaveData.balance;
            float newDebt = _data.SaveData.debts;

            if (!Mathf.Approximately(newBalance, _currentBalance))
            {
                if (_balanceRoutine != null) StopCoroutine(_balanceRoutine);
                _balanceRoutine = StartCoroutine(AnimateValue(_balanceText, _currentBalance, newBalance));
                _currentBalance = newBalance;
            }

            if (!Mathf.Approximately(newDebt, _currentDebt))
            {
                if (_debtRoutine != null) StopCoroutine(_debtRoutine);
                _debtRoutine = StartCoroutine(AnimateValue(_debtText, _currentDebt, newDebt));
                _currentDebt = newDebt;
            }
        }

        private void ApplyImmediate()
        {
            if (_balanceText != null) _balanceText.text = FormatMoney(_currentBalance);
            if (_debtText != null) _debtText.text = FormatMoney(_currentDebt);
        }

        private IEnumerator AnimateValue(TextMeshProUGUI text, float start, float target)
        {
            if (text == null) yield break;

            float t = 0f;
            float d = Mathf.Max(0.01f, ScrollDuration);

            while (t < d)
            {
                t += Time.deltaTime;
                float v = Mathf.Lerp(start, target, t / d);
                text.text = FormatMoney(v);
                yield return null;
            }

            text.text = FormatMoney(target);
        }

        private static string FormatMoney(float amount)
        {
            float abs = Mathf.Abs(amount);

            if (abs >= 1_000_000_000f) return string.Format("${0:0.##}ggg", amount / 1_000_000_000f);
            if (abs >= 1_000_000f) return string.Format("${0:0.##}kk", amount / 1_000_000f);
            if (abs >= 1_000f) return string.Format("${0:0.##}k", amount / 1_000f);

            return string.Format("${0:0.00}", amount);
        }
    }
}