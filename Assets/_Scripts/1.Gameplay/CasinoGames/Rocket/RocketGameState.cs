namespace Module.Gameplay
{
    public enum ERocketRoundPhase
    {
        Idle,
        Betting,
        Running,
        Ended
    }

    public sealed class RocketGameState
    {
        public ERocketRoundPhase Phase { get; internal set; } = ERocketRoundPhase.Idle;

        public float CurrentBet { get; internal set; }
        public float CurrentMultiplier { get; internal set; } = 1f;

        public float LastMultiplier { get; internal set; } = 1f;

        public bool BetConfirmed { get; internal set; }
        public bool HasClaimed { get; internal set; }

        public float ElapsedTime { get; internal set; }
        public float ExplodeTime { get; internal set; }
    }
}
