using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Module.Gameplay;
using Module.Core;

public class Expense : MonoBehaviour
{
    [SerializeField] protected string _expenseName;
    [SerializeField] protected DebtType _debtType;
    [SerializeField] protected TextMeshProUGUI _expenseText;
    [SerializeField] protected Button _expenseButton;
    [SerializeField] protected bool _isSelected;
    [SerializeField] protected Sprite _boxOn;
    [SerializeField] protected Sprite _boxOff;
    protected float _amount;

    private DayReport _dayReport;
    protected IDataService _dataService;

    public string ExpenseName => _expenseName;
    public DebtType DebtType => _debtType;
    public float Amount => _amount;
    public bool IsSelected => _isSelected;

    private void Awake()
    {
        _dataService = ServiceRegistry.Get<IDataService>();
    }

    protected virtual void Start()
    {
        _dayReport = FindObjectOfType<DayReport>();
        _expenseButton?.onClick?.AddListener(OnExpenseSelected);
    }

    public void ToggleSelection()
    {
        _isSelected = !_isSelected;
        _expenseText.color = _isSelected ? HexToColor("#FF0000") : HexToColor("#4700DE");

        Image buttonImage = _expenseButton.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.sprite = _isSelected ? _boxOn : _boxOff;
        }
    }

    public void UpdateButtonAvailability(bool canAfford)
    {
        if (_expenseButton != null)
            _expenseButton.interactable = canAfford;
    }

    protected virtual void UpdateVisibility()
    {
        if (ShouldBeVisible())
        {
            gameObject.SetActive(true);
            _expenseText.text = $"{_expenseName}: -${_amount}";
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void OnExpenseSelected()
    {
        _dayReport.OnExpenseSelected(this);
    }

    protected virtual bool ShouldBeVisible()
    {
        return true;
    }

    private Color HexToColor(string hex)
    {
        if (ColorUtility.TryParseHtmlString(hex, out Color color))
        {
            return color;
        }
        else
        {
            Debug.LogWarning($"Invalid HEX color: {hex}");
            return Color.blue;
        }
    }
}
