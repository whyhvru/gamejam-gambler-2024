using System.Collections;
using System.Collections.Generic;
using Module.Core;
using TMPro;
using UnityEngine;

namespace Module.Presentation.UI
{
    public class MessageManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _messageTextArea;

        private List<string> _currentMessages;
        private Dictionary<string, List<string>> _messageDatabase;
        private IDataService _dataService;

        private void Awake()
        {
            _dataService = ServiceRegistry.Get<IDataService>();
        }

        private void Start()
        {
            _currentMessages = new List<string>();

            InitializeMessageDatabase();

            AddStartOfDayMessage();

            CheckMessages();
        }

        private void AddStartOfDayMessage()
        {
            int totalDays = _dataService.Data.Day + (_dataService.Data.Month == 12 ? 30 : 0);

            if (totalDays == 1)
            {
                AddMessage("Мысли", "Не могу забыть вчерашний день... так просто взяли и уволили.");
            }
            else if (totalDays == 7)
            {
                AddMessage("Мысли", "Прошла неделя. Надеюсь, что-то изменится к лучшему.");
            }
            else if (totalDays == 14)
            {
                AddMessage("Мысли", "Две недели... Думаю, сегодня мне точно повезет.");
            }
            else if (totalDays == 15)
            {
                AddMessage("Мысли", "Уже середина ноября, надо отыграться!");
            }
            else if (totalDays == 30)
            {
                AddMessage("Мысли", "Месяц прошел. Как же время летит...");
            }
            else if (totalDays == 60)
            {
                AddMessage("Мысли", "Прошло два месяца. Время не ждет.");
            }
            else
            {
                if (_messageDatabase.ContainsKey("StartOfDay"))
                {
                    string message = _messageDatabase["StartOfDay"][Random.Range(0, _messageDatabase["StartOfDay"].Count)];
                    AddMessage("Мысли", message);
                }
            }
        }

        public void AddNewFriendDebtMessage(int index)
        {
            if (_messageDatabase.ContainsKey("NewFriendDebt"))
            {
                string message = _messageDatabase["NewFriendDebt"][index];
                AddMessage("Уведомление", message);
            }
        }

        public void AddBankPayDebtMessage()
        {
            if (_messageDatabase.ContainsKey("BankPayDebt"))
            {
                string message = _messageDatabase["BankPayDebt"][Random.Range(0, _messageDatabase["BankPayDebt"].Count)];
                AddMessage("Уведомление", message);
            }
        }

        public void AddBankBanMessage()
        {
            if (_messageDatabase.ContainsKey("BankBan"))
            {
                string message = _messageDatabase["BankBan"][Random.Range(0, _messageDatabase["BankBan"].Count)];
                AddMessage("Уведомление", message);
            }
        }

        public void AddMicroloanMessage()
        {
            if (_messageDatabase.ContainsKey("MicroloanMessage"))
            {
                string message = _messageDatabase["MicroloanMessage"][Random.Range(0, _messageDatabase["MicroloanMessage"].Count)];
                AddMessage("Уведомление", message);
            }
        }

        public void AddFailureMessage()
        {
            if (_messageDatabase.ContainsKey("Failure"))
            {
                string message = _messageDatabase["Failure"][Random.Range(0, _messageDatabase["Failure"].Count)];
                AddMessage("Уведомление", message);
            }
        }

        public void AddBigWinMessage()
        {
            if (_messageDatabase.ContainsKey("BigWin"))
            {
                string message = _messageDatabase["BigWin"][Random.Range(0, _messageDatabase["BigWin"].Count)];
                AddMessage("Мысли", message);
            }
        }

        public void AddBigLossMessage()
        {
            if (_messageDatabase.ContainsKey("BigLoss"))
            {
                string message = _messageDatabase["BigLoss"][Random.Range(0, _messageDatabase["BigLoss"].Count)];
                AddMessage("Мысли", message);
            }
        }

        public void AddDeathMessage(string key)
        {
            if (_messageDatabase.ContainsKey(key))
            {
                ScheduleMessage(key, "Уведомление", 6f);
            }
        }

        private void CheckMessages()
        {
            if (_dataService.Data.MotherIsAlive && _dataService.Data.DaysWithoutMeds == 1)
            {
                ScheduleMessage("MotherNeedsMedicine", "Мать", 12f);
            }

            if (_dataService.Data.WifeIsAlive &&
                _dataService.Data.ChildIsAlive &&
                _dataService.Data.Child2IsAlive &&
                (_dataService.Data.DaysWithoutFood == 1 || _dataService.Data.DaysWithoutFood == 2))
            {
                ScheduleMessage("WifeNeedsFood", "Жена", Random.Range(24, 108));
            }

            if (_dataService.Data.WifeIsAlive &&
                _dataService.Data.ChildIsAlive &&
                _dataService.Data.Child2IsAlive &&
                _dataService.Data.DaysWithoutHeat == 1 && _dataService.Data.DaysWithoutHeat == 2)
            {
                ScheduleMessage("WifeNeedsHeat", "Жена", Random.Range(24, 108));
            }

            if (_dataService.Data.DaysUnpaidDebtFriends > 2)
            {
                ScheduleMessage("DebtFriends", "Друг", Random.Range(24, 108));
            }

            if (_dataService.Data.DaysUnpaidBank == 4)
            {
                ScheduleMessage("BankDue", "Банк", 36f);
            }

            if (_dataService.Data.DaysUnpaidBank > 4)
            {
                ScheduleMessage("BankLate", "Банк", 36f);
            }

            if (_dataService.Data.DaysUnpaidBank > 4)
            {
                ScheduleMessage("BankBan", "Банк", 48f);
            }

            if (_dataService.Data.DaysUnpaidMicroloan > 2)
            {
                ScheduleMessage("MicroloanPenalty", "Микрозайм", Random.Range(108, 144));
            }

            if (_dataService.Data.DaysUnpaidCarDebt > 4)
            {
                ScheduleMessage("CarLate", "Кредитор", Random.Range(108, 144));
            }
        }

        private void ScheduleMessage(string category, string sender, float delay)
        {
            if (!_messageDatabase.ContainsKey(category)) return;

            string message = _messageDatabase[category][Random.Range(0, _messageDatabase[category].Count)];
            StartCoroutine(AddMessageWithDelay(sender, message, delay));
        }

        private IEnumerator AddMessageWithDelay(string sender, string message, float delay)
        {
            yield return new WaitForSeconds(delay);
            AddMessage(sender, message);
        }

        private void AddMessage(string sender, string message)
        {
            _currentMessages.Add($"{sender}: {message}");

            if (_currentMessages.Count > 3)
            {
                _currentMessages.RemoveAt(0);
            }

            UpdateMessageText();
        }

        private void UpdateMessageText()
        {
            _messageTextArea.text = string.Join("\n\n", _currentMessages);
        }

        private void InitializeMessageDatabase()
        {
            _messageDatabase = new Dictionary<string, List<string>>();

            _messageDatabase["StartOfDay"] = new List<string>
        {
            "Не особо выспался, но это не помешает мне сорвать куш.",
            "Новый день, пора начать играть аккуратнее.",
            "Утро доброе, сегодня я точно отыграюсь.",
            "Вчера играл слишком аккуратно, сегодня поставлю больше.",
            "Чувствую, сегодня мне точно повезет.",
            "Сегодня я точно отыграюсь. Уж точно.",
            "Надо скорее вставать и идти побеждать.",
            "Черт, сегодня мой счастливый день, попытаюсь.",
            "Руки так и чешутся... Может сегодня сорву куш?"
        };

            _messageDatabase["BigWin"] = new List<string>
        {
            "Черт возьми, да! Это мое время!",
            "Да ну нахер, я выиграл! Это просто невероятно!",
            "Ооооооооооо, ну вот! Как чувствовал!",
            "Вот это да! Ну всё, не остановить!",
            "Вот это поворот! Можно теперь ставить больше.",
            "Да, сучара! Как же жестко! Наконец-то!",
            "Хаа! Серьезно? Это я, я! Все я!",
            "Твою мать, наконец-то! Вот оно!",
            "Дааааааааа! Как же я силен!",
            "Я был прав! Черт, наконец-то результат!"
        };

            _messageDatabase["BigLoss"] = new List<string>
        {
            "Черт, нет, как так?! Где быть... взять в долг?",
            "Как я мог так ошибиться, твою мать!",
            "Не могу поверить! Что теперь делать?",
            "Черт, как я мог так проиграть?",
            "Так и знал, сучара! Так и знал...",
            "Вот чувствовал же, что так будет, черт!",
            "Потерял всё... Может... кредит взять?",
            "Черт, да как так-то, а?! Я был так близок...",
            "Как я мог так прогореть? Может... взять у кого в долг?",
            "Ааа, ну нет! Ожидаемо было. Что же делать?"
        };

            _messageDatabase["MotherNeedsMedicine"] = new List<string>
        {
            "Сынок, ты вчера забыл про моё лекарство? Прошу, купи сегодня.",
            "Мне становится хуже. Пожалуйста, найди время и сходи в аптеку!",
            "Пожалуйста, купи лекарства, сынок. Я больше не могу терпеть боль!",
            "Сынок... Я понимаю, что у тебя много дел, но прошу, купи лекарства сегодня.",
            "Мне очень больно, сынок. Прошу тебя, не забудь купить лекарства."
        };

            _messageDatabase["WifeNeedsFood"] = new List<string>
        {
            "О боже, ты вообще думаешь о детях? Сегодня принеси еды. Обязательно!",
            "Дети уже плачут от голода. Что с тобой стряслось??",
            "Мы не выдержим больше, срочно купи поесть... хотя бы детям.",
            "Невыносимо! Ты о детях всегда в последнюю очередь думаешь? Принеси еды!"
        };

            _messageDatabase["WifeNeedsHeat"] = new List<string>
        {
            "Не знаю, что с тобой в последнее время, но оплати ты уже за отопление!",
            "Дети заболели, дома невыносимо холодно! Оплати за отопление!",
            "Мы не выдержим больше, холодрыга пробирает до костей. Ты тут?",
            "Невыносимо холодно! Срочно оплати за отопление!"
        };

            _messageDatabase["NewFriendDebt"] = new List<string>
        {
            "Вы взяли в долг у лучшего друга. Постарайтесь вернуть за пару дней.",
            "Ваш друг нашел деньги, чтобы одолжить их Вам.",
            "Ваш давний друг проявил к Вам жалость, одолжив $100.",
            "Подруге было неловко Вам отказывать и вы получили $100.",
            "Ваш знакомый был удивлен, но все же согласился помочь."
        };

            _messageDatabase["DebtFriends"] = new List<string>
        {
            "Привет, не мог бы ты вернуть те деньги? Напиши, как прочитаешь",
            "Привет, дружище. Когда вернешь долг? Мне сейчас тоже нужны деньги.",
            "Привет, слушай... Ты говорил о пару дней, а уже прошло сколько. Напиши мне.",
            "Привет, помнишь, ты деньги занимал? Когда их вернешь примерно?",
            "Привет, нашел работу? В общем, мне сейчас деньги нужны, верни поскорее."
        };

            _messageDatabase["BankPayDebt"] = new List<string>
        {
            $"Погасите текущую задолженность, чтобы оформить новый кредит."
        };

            _messageDatabase["BankBan"] = new List<string>
        {
            $"К сожалению, Вы внесены в черный список кредиторов. Оформить кредит невозможно."
        };

            _messageDatabase["BankDue"] = new List<string>
        {
            $"Завтра наступает срок платежа по вашему кредиту.\nВнесите платеж вовремя, чтобы избежать начисления штрафов.\nС уважением, ИЦ Банк."
        };

            _messageDatabase["BankLate"] = new List<string>
        {
            $"Банк: Уважаемый клиент,\nВаш платеж по кредиту просрочен. Погасите задолженность, чтобы избежать дополнительных штрафов.\nИЦ Банк."
        };

            _messageDatabase["BankBan"] = new List<string>
        {
            $"Уважаемый клиент,\nВы внесены в черный список кредиторов.\nИЦ Банк."
        };

            _messageDatabase["MicroloanMessage"] = new List<string>
        {
            $"Вы взяли микрозайм на $500. У вас есть 2 дня, чтобы вернуть их."
        };

            _messageDatabase["MicroloanPenalty"] = new List<string>
        {
            $"Уважаемый клиент,\nСрок погашения вашего микрозайма истек. Начисляется пеня в размере 25% за каждый день просрочки.\nИО Микрозайм."
        };

            _messageDatabase["Failure"] = new List<string>
        {
            $"К сожалению, Вам нечего закладывать.",
            $"К сожалению, Вам нечего предложить кредитору."
        };

            _messageDatabase["CarLate"] = new List<string>
        {
            $"Уважаемый клиент,\nСрок погашения долга по вашему займу истек.\nМы имеем право изъять заложенное имущество - ваш автомобиль."
        };

            _messageDatabase["MothersDeath"] = new List<string>
        {
            $"Плохие новости, Ваша мать ушла из жизни. Это большая утрата для Вас. Примите наши соболезнования."
        };

            _messageDatabase["WifesDeath"] = new List<string>
        {
            $"Ваша жена скончалась. Пожалуйста, найдите силы пережить эту утрату."
        };

            _messageDatabase["ChildsDeath"] = new List<string>
        {
            $"Несвоевременные решения или трудные обстоятельства привели к трагедии. Ваш ребенок больше не с вами."
        };

            _messageDatabase["Childs2Death"] = new List<string>
        {
            $"Вы потеряли второго ребенка. Мы понимаем, как тяжело вам справляться с этой болью. Берегите себя."
        };
        }
    }
}