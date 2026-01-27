namespace Module.Core
{
    public static class SlotMachineConfig
    {
        public const float MinBet = 10f;
        public const float BetStep = 10f;

        public const int ThreeEqualMultiplier = 10;
        public const int TwoEqualSameCategoryMultiplier = 3;
        public const int TwoEqualDifferentCategoryMultiplier = 2;
        public const int AllDifferentSameCategoryMultiplier = 1;

        public const float MinBalanceForLossMessage = 50f;
    }
}
