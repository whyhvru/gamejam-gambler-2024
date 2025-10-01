using System.Collections.Generic;
using Module.Core;
using Module.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Module.Presentation.UI
{
    public class MapUI : MonoBehaviour
    {
        private const string BankConfirmText = "Вы в банке. Вы можете взять кредит на сумму $1000. (4 дня)";
        private const string LenderConfirmText = "Кредитор предлагает $10000 под залог вашей машины. (4 дня)";
        private const float BankLoanAmount = 1000f;
        private const float MicroloanAmount = 500f;
        private const float CarLoanAmount = 10000f;
        private const float FriendLoanAmount = 100f;

        [Header("Casino")]
        [SerializeField] private GameObject _casinoWindow;
        [SerializeField] private Button _casinoButton;
        [SerializeField] private Button _rocketButton;
        [SerializeField] private Button _slotButton;
        [SerializeField] private GameObject _rocketPanel;
        [SerializeField] private GameObject _slotPanel;

        [Header("Confirm Window")]
        [SerializeField] private GameObject _confirmWindow;
        [SerializeField] private TextMeshProUGUI _confirmText;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private Button _confirmButton;

        [Header("Bank")]
        [SerializeField] private Button _bankButton;

        [Header("Lender")]
        [SerializeField] private Button _lenderButton;

        [Header("Microloan")]
        [SerializeField] private Button _microloanButton;

        [Header("Friends")]
        [SerializeField] private List<Button> _friendButtons = new();

        [Header("Services")]
        [SerializeField] private MessageManager _messageManager;

        private IDataService _dataService;
        private DebtService _debtService;

        private void Awake()
        {
            _dataService = ServiceRegistry.Get<IDataService>();
            _debtService = ServiceRegistry.Get<DebtService>();
        }

        private void Start()
        {
            _casinoButton.onClick.AddListener(OpenCasino);
            _rocketButton.onClick.AddListener(() => OpenCasinoGame(_rocketPanel, "RocketPanel"));
            _slotButton.onClick.AddListener(() => OpenCasinoGame(_slotPanel, "SlotPanel"));

            _bankButton.onClick.AddListener(OpenBank);
            _lenderButton.onClick.AddListener(OpenLender);
            _microloanButton.onClick.AddListener(BorrowMicroloan);

            for (int i = 0; i < _friendButtons.Count; i++)
            {
                int friendIndex = i;
                _friendButtons[i].onClick.AddListener(() => BorrowFromFriend(friendIndex));
            }
        }

        #region Casino
        private void OpenCasino() => _casinoWindow.SetActive(true);

        private void OpenCasinoGame(GameObject panel, string musicTag)
        {
            _casinoWindow.SetActive(false);
            panel.SetActive(true);
            MusicManager.Instance.OnWindowChanged(musicTag);
        }
        #endregion

        #region Bank
        private void OpenBank()
        {
            var data = _dataService.Data;

            if (data.BanInBank)
            {
                _messageManager.AddBankBanMessage();
                return;
            }

            if (data.BankDebt > 0f)
            {
                _messageManager.AddBankPayDebtMessage();
                return;
            }

            ShowWindow(
                BankConfirmText,
                () => Borrow(BankLoanAmount, () => data.BankDebt = BankLoanAmount),
                HideWindow);
        }

        #endregion

        #region Lender
        private void OpenLender()
        {
            var data = _dataService.Data;

            if (data.CarDebt > 0f || !data.HasACar)
            {
                _messageManager.AddFailureMessage();
                return;
            }

            ShowWindow(LenderConfirmText,
                () => Borrow(CarLoanAmount, () =>
                {
                    data.CarDebt = CarLoanAmount;
                    data.HasACar = false;
                }),
                HideWindow);
        }

        #endregion

        #region Microloan
        private void BorrowMicroloan()
        {
            _dataService.Data.Microloan += MicroloanAmount;
            _debtService.AddDebt(MicroloanAmount);
            _messageManager.AddMicroloanMessage();
        }
        #endregion

        #region Friends
        private void BorrowFromFriend(int index)
        {
            var data = _dataService.Data;
            bool[] debts =
            {
                data.DebtTo1Friend,
                data.DebtTo2Friend,
                data.DebtTo3Friend,
                data.DebtTo4Friend,
                data.DebtTo5Friend
            };

            if (debts[index]) return;

            switch (index)
            {
                case 0: data.DebtTo1Friend = true; break;
                case 1: data.DebtTo2Friend = true; break;
                case 2: data.DebtTo3Friend = true; break;
                case 3: data.DebtTo4Friend = true; break;
                case 4: data.DebtTo5Friend = true; break;
            }

            _debtService.AddDebt(FriendLoanAmount);
            _messageManager.AddNewFriendDebtMessage(index);
        }
        #endregion

        #region Helpers
        private void ShowWindow(string text, UnityEngine.Events.UnityAction confirm, UnityEngine.Events.UnityAction cancel)
        {
            _confirmText.text = text;
            _confirmWindow.SetActive(true);
            _confirmButton.onClick.AddListener(confirm);
            _cancelButton.onClick.AddListener(cancel);
        }

        private void HideWindow()
        {
            _confirmWindow.SetActive(false);
            _confirmButton.onClick.RemoveAllListeners();
            _cancelButton.onClick.RemoveAllListeners();
        }

        private void Borrow(float amount, System.Action applyDebt)
        {
            applyDebt.Invoke();
            _debtService.AddDebt(amount);
        }
        #endregion
    }
}