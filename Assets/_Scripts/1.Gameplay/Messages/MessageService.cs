using System;
using System.Collections.Generic;
using Module.Core;

namespace Module.Gameplay
{
    public sealed class MessageService
    {
        private const int MaxMessages = 3;

        private readonly IDataService _data;
        private readonly ITimer _timer;

        private readonly List<string> _current = new(MaxMessages);
        private readonly Dictionary<string, List<string>> _db = new();

        public event Action<string> OnTextChanged;

        public MessageService(IDataService data, ITimer timer)
        {
            _data = data;
            _timer = timer;

            InitializeMessageDatabase();
        }

        public void InitializeForNewDay()
        {
            AddStartOfDayMessage();
            CheckMessages();
            Notify();
        }

        public void AddNewFriendDebtMessage(int index) => AddFromCategoryByIndex("NewFriendDebt", "Уведомление", index);
        public void AddBankPayDebtMessage() => AddRandomFromCategory("BankPayDebt", "Уведомление");
        public void AddBankBanMessage() => AddRandomFromCategory("BankBan", "Уведомление");
        public void AddMicroloanMessage() => AddRandomFromCategory("MicroloanMessage", "Уведомление");
        public void AddFailureMessage() => AddRandomFromCategory("Failure", "Уведомление");
        public void AddBigWinMessage() => AddRandomFromCategory("BigWin", "Мысли");
        public void AddBigLossMessage() => AddRandomFromCategory("BigLoss", "Мысли");
        public void AddDeathMessage(string key) => ScheduleMessage(key, "Уведомление", 6f);

        private void AddStartOfDayMessage()
        {
            int totalDays = _data.SaveData.day + (_data.SaveData.month == 12 ? 30 : 0);

            if (totalDays == 1) { Add("Мысли", "Не могу забыть вчерашний день... так просто взяли и уволили."); return; }
            if (totalDays == 7) { Add("Мысли", "Прошла неделя. Надеюсь, что-то изменится к лучшему."); return; }
            if (totalDays == 14) { Add("Мысли", "Две недели... Думаю, сегодня мне точно повезет."); return; }
            if (totalDays == 15) { Add("Мысли", "Уже середина ноября, надо отыграться!"); return; }
            if (totalDays == 30) { Add("Мысли", "Месяц прошел. Как же время летит..."); return; }
            if (totalDays == 60) { Add("Мысли", "Прошло два месяца. Время не ждет."); return; }

            AddRandomFromCategory("StartOfDay", "Мысли");
        }

        private void CheckMessages()
        {
            var s = _data.SaveData;

            if (s.motherIsAlive && s.daysWithoutMeds == 1)
                ScheduleMessage("MotherNeedsMedicine", "Мать", 12f);

            if (s.wifeIsAlive && s.childIsAlive && s.child2IsAlive && (s.daysWithoutFood == 1 || s.daysWithoutFood == 2))
                ScheduleMessage("WifeNeedsFood", "Жена", UnityEngine.Random.Range(24, 108));

            // FIX: было ==1 && ==2
            if (s.wifeIsAlive && s.childIsAlive && s.child2IsAlive && (s.daysWithoutHeat == 1 || s.daysWithoutHeat == 2))
                ScheduleMessage("WifeNeedsHeat", "Жена", UnityEngine.Random.Range(24, 108));

            if (s.daysUnpaidDebtFriends > 2)
                ScheduleMessage("DebtFriends", "Друг", UnityEngine.Random.Range(24, 108));

            if (s.daysUnpaidBank == 4)
                ScheduleMessage("BankDue", "Банк", 36f);

            if (s.daysUnpaidBank > 4)
                ScheduleMessage("BankLate", "Банк", 36f);

            // В твоём коде BankBan проверялся дважды; оставим один раз
            if (s.daysUnpaidBank > 4)
                ScheduleMessage("BankBan", "Банк", 48f);

            if (s.daysUnpaidMicroloan > 2)
                ScheduleMessage("MicroloanPenalty", "Микрозайм", UnityEngine.Random.Range(108, 144));

            if (s.daysUnpaidCarDebt > 4)
                ScheduleMessage("CarLate", "Кредитор", UnityEngine.Random.Range(108, 144));
        }

        private void ScheduleMessage(string category, string sender, float delay)
        {
            if (!_db.TryGetValue(category, out var list) || list.Count == 0) return;

            string msg = list[UnityEngine.Random.Range(0, list.Count)];
            _timer.Schedule(delay, () =>
            {
                Add(sender, msg);
                Notify();
            });
        }

        private void AddRandomFromCategory(string category, string sender)
        {
            if (!_db.TryGetValue(category, out var list) || list.Count == 0) return;
            Add(sender, list[UnityEngine.Random.Range(0, list.Count)]);
            Notify();
        }

        private void AddFromCategoryByIndex(string category, string sender, int index)
        {
            if (!_db.TryGetValue(category, out var list) || list.Count == 0) return;
            if (index < 0 || index >= list.Count) return;

            Add(sender, list[index]);
            Notify();
        }

        private void Add(string sender, string message)
        {
            _current.Add($"{sender}: {message}");
            while (_current.Count > MaxMessages)
                _current.RemoveAt(0);
        }

        private void Notify() => OnTextChanged?.Invoke(string.Join("\n\n", _current));

        private void InitializeMessageDatabase()
        {
            _db.Clear();

            _db["StartOfDay"] = new List<string>
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

            _db["BigWin"] = new List<string>
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

            _db["BigLoss"] = new List<string>
            {
                "Черт, нет, как так?! Где бы... взять в долг?",
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

            _db["MotherNeedsMedicine"] = new List<string>
            {
                "Сынок, ты вчера забыл про моё лекарство? Прошу, купи сегодня.",
                "Мне становится хуже. Пожалуйста, найди время и сходи в аптеку!",
                "Пожалуйста, купи лекарства, сынок. Я больше не могу терпеть боль!",
                "Сынок... Я понимаю, что у тебя много дел, но прошу, купи лекарства сегодня.",
                "Мне очень больно, сынок. Прошу тебя, не забудь купить лекарства."
            };

            _db["WifeNeedsFood"] = new List<string>
            {
                "О боже, ты вообще думаешь о детях? Сегодня принеси еды. Обязательно!",
                "Дети уже плачут от голода. Что с тобой стряслось??",
                "Мы не выдержим больше, срочно купи поесть... хотя бы детям.",
                "Невыносимо! Ты о детях всегда в последнюю очередь думаешь? Принеси еды!"
            };

            _db["WifeNeedsHeat"] = new List<string>
            {
                "Не знаю, что с тобой в последнее время, но оплати ты уже за отопление!",
                "Дети заболели, дома невыносимо холодно! Оплати за отопление!",
                "Мы не выдержим больше, холодрыга пробирает до костей. Ты тут?",
                "Невыносимо холодно! Срочно оплати за отопление!"
            };

            _db["NewFriendDebt"] = new List<string>
            {
                "Вы взяли в долг у лучшего друга. Постарайтесь вернуть за пару дней.",
                "Ваш друг нашел деньги, чтобы одолжить их Вам.",
                "Ваш давний друг проявил к Вам жалость, одолжив $100.",
                "Подруге было неловко Вам отказывать и вы получили $100.",
                "Ваш знакомый был удивлен, но все же согласился помочь."
            };

            _db["DebtFriends"] = new List<string>
            {
                "Привет, не мог бы ты вернуть те деньги? Напиши, как прочитаешь",
                "Привет, дружище. Когда вернешь долг? Мне сейчас тоже нужны деньги.",
                "Привет, слушай... Ты говорил о пару дней, а уже прошло сколько. Напиши мне.",
                "Привет, помнишь, ты деньги занимал? Когда их вернешь примерно?",
                "Привет, нашел работу? В общем, мне сейчас деньги нужны, верни поскорее."
            };

            _db["BankPayDebt"] = new List<string>
            {
                "Погасите текущую задолженность, чтобы оформить новый кредит."
            };

            _db["BankBan"] = new List<string>
            {
                "Уважаемый клиент,\nВы внесены в черный список кредиторов.\nИЦ Банк."
            };

            _db["BankDue"] = new List<string>
            {
                "Завтра наступает срок платежа по вашему кредиту.\nВнесите платеж вовремя, чтобы избежать начисления штрафов.\nС уважением, ИЦ Банк."
            };

            _db["BankLate"] = new List<string>
            {
                "Банк: Уважаемый клиент,\nВаш платеж по кредиту просрочен. Погасите задолженность, чтобы избежать дополнительных штрафов.\nИЦ Банк."
            };

            _db["MicroloanMessage"] = new List<string>
            {
                "Вы взяли микрозайм на $500. У вас есть 2 дня, чтобы вернуть их."
            };

            _db["MicroloanPenalty"] = new List<string>
            {
                "Уважаемый клиент,\nСрок погашения вашего микрозайма истек. Начисляется пеня в размере 25% за каждый день просрочки.\nИО Микрозайм."
            };

            _db["Failure"] = new List<string>
            {
                "К сожалению, Вам нечего закладывать.",
                "К сожалению, Вам нечего предложить кредитору."
            };

            _db["CarLate"] = new List<string>
            {
                "Уважаемый клиент,\nСрок погашения долга по вашему займу истек.\nМы имеем право изъять заложенное имущество - ваш автомобиль."
            };

            _db["MothersDeath"] = new List<string>
            {
                "Плохие новости, Ваша мать ушла из жизни. Это большая утрата для Вас. Примите наши соболезнования."
            };

            _db["WifesDeath"] = new List<string>
            {
                "Ваша жена скончалась. Пожалуйста, найдите силы пережить эту утрату."
            };

            _db["ChildsDeath"] = new List<string>
            {
                "Несвоевременные решения или трудные обстоятельства привели к трагедии. Ваш ребенок больше не с вами."
            };

            _db["Childs2Death"] = new List<string>
            {
                "Вы потеряли второго ребенка. Мы понимаем, как тяжело вам справляться с этой болью. Берегите себя."
            };
        }
    }
}
