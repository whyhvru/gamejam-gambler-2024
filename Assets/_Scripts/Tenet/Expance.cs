using Module.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class Expense : MonoBehaviour
{
    [SerializeField] protected DayReport dayReport;
    [SerializeField] protected string expenseName;
    [SerializeField] protected TextMeshProUGUI expenseText;
    [SerializeField] protected Button expenseButton;
    [SerializeField] protected bool isSelected;
    [SerializeField] protected Sprite boxOn;
    [SerializeField] protected Sprite boxOff;

    protected IDataService dataService;
    protected float amount;

    public string ExpenseName => expenseName;
    public float Amount => amount;
    public bool IsSelected => isSelected;

    [Inject]
    public void Construct(IDataService data) => dataService = data;

    protected virtual void Start() => expenseButton.onClick?.AddListener(OnExpenseSelected);

    public void ToggleSelection()
    {
        isSelected = !isSelected;
        expenseText.color = isSelected ? HexToColor("#FF0000") : HexToColor("#4700DE");

        if (expenseButton.TryGetComponent<Image>(out var buttonImage))
        {
            buttonImage.sprite = isSelected ? boxOn : boxOff;
        }
    }

    public void UpdateButtonAvailability(bool canAfford)
    {
        if (expenseButton != null)
            expenseButton.interactable = canAfford;
    }

    protected virtual void UpdateVisibility()
    {
        if (ShouldBeVisible())
        {
            gameObject.SetActive(true);
            expenseText.text = $"{expenseName}: -${amount}";
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void OnExpenseSelected() => dayReport.OnExpenseSelected(this);
    protected virtual bool ShouldBeVisible() => true;

    private Color HexToColor(string hex)
    {
        if (ColorUtility.TryParseHtmlString(hex, out var color))
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
