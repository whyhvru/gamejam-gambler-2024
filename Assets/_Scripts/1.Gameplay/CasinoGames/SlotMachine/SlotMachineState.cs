namespace Module.Gameplay
{
    public enum ESlotRoundPhase
    {
        Idle,
        Spinning,
        Ended
    }

    public sealed class SlotMachineState
    {
        public ESlotRoundPhase Phase { get; internal set; } = ESlotRoundPhase.Idle;

        public float CurrentBet { get; internal set; }
        public bool BetConfirmed { get; internal set; }
        public bool IsSpinning => Phase == ESlotRoundPhase.Spinning;

        public int LastMultiplier { get; internal set; }
    }
}