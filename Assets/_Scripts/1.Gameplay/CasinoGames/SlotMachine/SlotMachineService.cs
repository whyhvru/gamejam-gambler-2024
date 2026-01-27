using System;
using Module.Core;

namespace Module.Gameplay
{
    public sealed class SlotMachineService
    {
        public SlotMachineState State { get; } = new();

        public event Action OnStateChanged;
        public event Action OnSpinStarted;
        public event Action<int> OnSpinFinished;

        public void SetBet(float bet)
        {
            if (State.IsSpinning) return;

            State.CurrentBet = bet;
            OnStateChanged?.Invoke();
        }

        public bool CanConfirmBet(float currentBalance)
            => !State.IsSpinning
                && State.CurrentBet >= SlotMachineConfig.MinBet
                && State.CurrentBet <= currentBalance;

        public void ConfirmBet(float currentBalance)
        {
            if (!CanConfirmBet(currentBalance)) return;

            State.BetConfirmed = true;
            OnStateChanged?.Invoke();
        }

        public bool CanStartSpin(float currentBalance)
            => !State.IsSpinning
                && State.BetConfirmed
                && State.CurrentBet >= SlotMachineConfig.MinBet
                && State.CurrentBet <= currentBalance;

        public void StartSpin()
        {
            State.Phase = ESlotRoundPhase.Spinning;
            OnSpinStarted?.Invoke();
            OnStateChanged?.Invoke();
        }

        public int FinishSpin(int index1, int index2, int index3)
        {
            int multiplier = SlotResultEvaluator.EvaluateMultiplier(index1, index2, index3);

            State.LastMultiplier = multiplier;
            State.BetConfirmed = false;
            State.Phase = ESlotRoundPhase.Ended;

            OnSpinFinished?.Invoke(multiplier);
            OnStateChanged?.Invoke();

            return multiplier;
        }
    }
}
