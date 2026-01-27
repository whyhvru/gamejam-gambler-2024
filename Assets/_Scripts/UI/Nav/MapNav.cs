using Module.Core;
using Module.Gameplay;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

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

    private IDataService _dataService;
    private IAudioService _audioService;
    private MessageService _messageService;

    [Inject]
    public void Construct(IDataService data, IAudioService audio, MessageService message)
    {
        _dataService = data;
        _audioService = audio;
        _messageService = message;
    }

    private void Start()
    {
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
        _audioService.OnWindowChanged("MapPanel");
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
        _audioService.OnWindowChanged("RocketPanel");
    }

    private void OpenSlot()
    {
        _casinoWindow.SetActive(false);
        _slotPanel.SetActive(true);
        _mapPanel.SetActive(false);
        _audioService.OnWindowChanged("SlotPanel");
    }

    private void OpenBank()
    {
        if (!_dataService.SaveData.banInBank)
        {
            if (_dataService.SaveData.bankDebt == 0f)
            {
                _bankWindow.SetActive(true);
                _bankConfirmButton.onClick.AddListener(BorrowBank);
                _bankCancelButton.onClick.AddListener(CloseBank);
            }
            else
            {
                _messageService.AddBankPayDebtMessage();
            }
        }
        else
        {
            _messageService.AddBankBanMessage();
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
        if (_dataService.SaveData.carDebt == 0f && _dataService.SaveData.hasACar)
        {
            _lenderWindow.SetActive(true);
            _lenderConfirmButton.onClick.AddListener(BorrowCar);
            _lenderCancelButton.onClick.AddListener(CloseLender);
        }
        else
        {
            _messageService.AddFailureMessage();
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
        if (_dataService.SaveData.bankDebt == 0f && !_dataService.SaveData.banInBank)
        {
            _dataService.SaveData.bankDebt = 1000f;
            _dataService.AddDebt(1000f);
            _bankWindow.SetActive(false);
        }
    }

    private void BorrowMicroloan()
    {
        float currentMicroloan = _dataService.SaveData.microloan;
        currentMicroloan += 500f;
        _dataService.SaveData.microloan = currentMicroloan;
        _dataService.AddDebt(500f);
        _messageService.AddMicroloanMessage();
    }

    private void BorrowCar()
    {
        if (_dataService.SaveData.hasACar)
        {
            _dataService.SaveData.hasACar = false;
            _dataService.SaveData.carDebt = 10000f;
            _dataService.AddDebt(10000f);
            _lenderWindow.SetActive(false);
        }
    }

    private void Borrow1()
    {
        if (!_dataService.SaveData.debtTo1Friend)
        {
            _dataService.SaveData.debtTo1Friend = true;
            _dataService.AddDebt(100f);
            _messageService.AddNewFriendDebtMessage(0);
        }
    }

    private void Borrow2()
    {
        if (!_dataService.SaveData.debtTo2Friend)
        {
            _dataService.SaveData.debtTo2Friend = true;
            _dataService.AddDebt(100f);
            _messageService.AddNewFriendDebtMessage(1);
        }
    }

    private void Borrow3()
    {
        if (!_dataService.SaveData.debtTo3Friend)
        {
            _dataService.SaveData.debtTo3Friend = true;
            _dataService.AddDebt(100f);
            _messageService.AddNewFriendDebtMessage(2);
        }
    }

    private void Borrow4()
    {
        if (!_dataService.SaveData.debtTo4Friend)
        {
            _dataService.SaveData.debtTo4Friend = true;
            _dataService.AddDebt(100f);
            _messageService.AddNewFriendDebtMessage(3);
        }
    }

    private void Borrow5()
    {
        if (!_dataService.SaveData.debtTo5Friend)
        {
            _dataService.SaveData.debtTo5Friend = true;
            _dataService.AddDebt(100f);
            _messageService.AddNewFriendDebtMessage(4);
        }
    }
}
