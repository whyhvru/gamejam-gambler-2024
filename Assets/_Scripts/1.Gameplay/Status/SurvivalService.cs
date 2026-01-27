using System;
using Module.Core;

namespace Module.Gameplay
{
    public enum EFamilyMember
    {
        Mother,
        Wife,
        Child1,
        Child2
    }

    public sealed class SurvivalService
    {
        private readonly IDataService _data;
        private readonly MessageService _messages;

        public event Action<EFamilyMember> OnMemberDied;

        public SurvivalService(IDataService data, MessageService messages)
        {
            _data = data;
            _messages = messages;
        }

        public void EvaluateDeaths()
        {
            bool changed = false;

            changed |= HandleFoodOrHeatDeaths();
            changed |= HandleMedicineDeaths();

            if (changed)
                _data.Save();
        }

        private bool HandleFoodOrHeatDeaths()
        {
            var s = _data.SaveData;

            if (s.daysWithoutFood <= 2 && s.daysWithoutHeat <= 2)
                return false;

            if (s.childIsAlive)
            {
                s.childIsAlive = false;
                _messages.AddDeathMessage("ChildsDeath");
                OnMemberDied?.Invoke(EFamilyMember.Child1);
            }
            else if (s.child2IsAlive)
            {
                s.child2IsAlive = false;
                _messages.AddDeathMessage("Childs2Death");
                OnMemberDied?.Invoke(EFamilyMember.Child2);
            }
            else if (s.wifeIsAlive)
            {
                s.wifeIsAlive = false;
                _messages.AddDeathMessage("WifesDeath");
                OnMemberDied?.Invoke(EFamilyMember.Wife);
            }
            else if (s.motherIsAlive)
            {
                s.motherIsAlive = false;
                _messages.AddDeathMessage("MothersDeath");
                OnMemberDied?.Invoke(EFamilyMember.Mother);
            }

            s.daysWithoutFood = 0;
            s.daysWithoutHeat = 0;

            return true;
        }

        private bool HandleMedicineDeaths()
        {
            var s = _data.SaveData;

            if (s.daysWithoutMeds <= 1)
                return false;

            if (!s.motherIsAlive)
                return false;

            s.motherIsAlive = false;
            _messages.AddDeathMessage("MothersDeath");
            OnMemberDied?.Invoke(EFamilyMember.Mother);

            return true;
        }
    }
}
