using UnityEngine;
using UnityEngine.UI;

public class MapNav : MonoBehaviour
{
    [Header("OpenMap")]
    [SerializeField] private Button _openMap;

    [Header("Panels")]
    [SerializeField] private GameObject _mapPanel;
    [SerializeField] private GameObject _rocketPanel;
    [SerializeField] private GameObject _slotPanel;

    [Header("Locations")]
    [SerializeField] private Button _casinoButton;
    [SerializeField] private GameObject _casinoWindow;
    [SerializeField] private Button _rocketButton;
    [SerializeField] private Button _slotButton;

    [Header("Friends")]
    [SerializeField] private Button _friend_1;
    [SerializeField] private Button _friend_2;
    [SerializeField] private Button _friend_3;
    [SerializeField] private Button _friend_4;
    [SerializeField] private Button _friend_5;

    [Header("Bank")]
    [SerializeField] private Button _bank;
    [SerializeField] private GameObject _bankWindow;
    [SerializeField] private Button _bankConfirmButton;
    [SerializeField] private Button _bankCancelButton;

    [Header("Lender")]
    [SerializeField] private Button _lender;
    [SerializeField] private GameObject _lenderWindow;
    [SerializeField] private Button _lenderConfirmButton;
    [SerializeField] private Button _lenderCancelButton;
    [SerializeField] private Button _microloan;
    [SerializeField] private MessageManager _messageManager;

    private DataManager _dataManager;

    private void Start()
    {
        _dataManager = DataManager.Instance;
        _openMap.onClick.AddListener(OpenMap);
        _casinoButton.onClick.AddListener(OpenCasino);
        _microloan.onClick.AddListener(BorrowMicroloan);

        _bank.onClick.AddListener(OpenBank);

        _lender.onClick.AddListener(OpenLender);

        _friend_1.onClick.AddListener(Borrow1);
        _friend_2.onClick.AddListener(Borrow2);
        _friend_3.onClick.AddListener(Borrow3);
        _friend_4.onClick.AddListener(Borrow4);
        _friend_5.onClick.AddListener(Borrow5);
    }

    private void OpenMap()
    {
        _mapPanel.SetActive(true);
        _rocketPanel.SetActive(false);
        _slotPanel.SetActive(false);
        MusicManager.Instance.OnWindowChanged("MapPanel");
    }

    private void OpenCasino()
    {
        _casinoWindow.SetActive(true);
        _rocketButton.onClick.AddListener(OpenRocket);
        _slotButton.onClick.AddListener(OpenSlot);
    }

    private void OpenRocket()
    {
        _casinoWindow.SetActive(false);
        _rocketPanel.SetActive(true);
        _mapPanel.SetActive(false);
        MusicManager.Instance.OnWindowChanged("RocketPanel");
    }

    private void OpenSlot()
    {
        _casinoWindow.SetActive(false);
        _slotPanel.SetActive(true);
        _mapPanel.SetActive(false);
        MusicManager.Instance.OnWindowChanged("SlotPanel");
    }

    private void OpenBank()
    {
        if (!_dataManager.SaveData.banInBank)
        {
            if (_dataManager.SaveData.bankDebt == 0f)
            {
                _bankWindow.SetActive(true);
                _bankConfirmButton.onClick.AddListener(BorrowBank);
                _bankCancelButton.onClick.AddListener(CloseBank);
            }
            else
            {
                _messageManager.AddBankPayDebtMessage();
            }
        }
        else
        {
            _messageManager.AddBankBanMessage();
        }
    }

    private void CloseBank()
    {
        _bankWindow.SetActive(false);
        _bankConfirmButton.onClick.RemoveAllListeners();
        _bankCancelButton.onClick.RemoveAllListeners();
    }

    private void OpenLender()
    {
        if (_dataManager.SaveData.carDebt == 0f && _dataManager.SaveData.hasACar)
        {
            _lenderWindow.SetActive(true);
            _lenderConfirmButton.onClick.AddListener(BorrowCar);
            _lenderCancelButton.onClick.AddListener(CloseLender);
        }
        else
        {
            _messageManager.AddFailureMessage();
        }
    }

    private void CloseLender()
    {
        _lenderWindow.SetActive(false);
        _lenderConfirmButton.onClick.RemoveAllListeners();
        _lenderCancelButton.onClick.RemoveAllListeners();
    }

    private void BorrowBank()
    {
        if (_dataManager.SaveData.bankDebt == 0f && !_dataManager.SaveData.banInBank)
        {
            _dataManager.SaveData.bankDebt = 1000f;
            _dataManager.AddDebt(1000f);
            _bankWindow.SetActive(false);
        }
    }

    private void BorrowMicroloan()
    {
        float currentMicroloan = _dataManager.SaveData.microloan;
        currentMicroloan = currentMicroloan + 500f;
        _dataManager.SaveData.microloan = currentMicroloan;
        _dataManager.AddDebt(500f);
        _messageManager.AddMicroloanMessage();
    }

    private void BorrowCar()
    {
        if (_dataManager.SaveData.hasACar)
        {
            _dataManager.SaveData.hasACar = false;
            _dataManager.SaveData.carDebt = 10000f;
            _dataManager.AddDebt(10000f);
            _lenderWindow.SetActive(false);
        }
    }

    private void Borrow1()
    {
        if (!_dataManager.SaveData.debtTo1Friend)
        {
            _dataManager.SaveData.debtTo1Friend = true;
            _dataManager.AddDebt(100f);
            _messageManager.AddNewFriendDebtMessage(0);
        }
    }

    private void Borrow2()
    {
        if (!_dataManager.SaveData.debtTo2Friend)
        {
            _dataManager.SaveData.debtTo2Friend = true;
            _dataManager.AddDebt(100f);
            _messageManager.AddNewFriendDebtMessage(1);
        }
    }

    private void Borrow3()
    {
        if (!_dataManager.SaveData.debtTo3Friend)
        {
            _dataManager.SaveData.debtTo3Friend = true;
            _dataManager.AddDebt(100f);
            _messageManager.AddNewFriendDebtMessage(2);
        }
    }

    private void Borrow4()
    {
        if (!_dataManager.SaveData.debtTo4Friend)
        {
            _dataManager.SaveData.debtTo4Friend = true;
            _dataManager.AddDebt(100f);
            _messageManager.AddNewFriendDebtMessage(3);
        }
    }

    private void Borrow5()
    {
        if (!_dataManager.SaveData.debtTo5Friend)
        {
            _dataManager.SaveData.debtTo5Friend = true;
            _dataManager.AddDebt(100f);
            _messageManager.AddNewFriendDebtMessage(4);
        }
    }
}
