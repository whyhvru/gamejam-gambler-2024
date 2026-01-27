using Module.Core;

namespace Module.Gameplay
{
    public static class SlotResultEvaluator
    {
        public static int EvaluateMultiplier(int i1, int i2, int i3)
        {
            bool allEqual = i1 == i2 && i2 == i3;
            bool twoEqual = (i1 == i2) || (i1 == i3) || (i2 == i3);

            if (allEqual) return SlotMachineConfig.ThreeEqualMultiplier;

            if (twoEqual)
                return IsAllSameCategory(i1, i2, i3)
                    ? SlotMachineConfig.TwoEqualSameCategoryMultiplier
                    : SlotMachineConfig.TwoEqualDifferentCategoryMultiplier;

            if (IsAllDifferent(i1, i2, i3) && IsAllSameCategory(i1, i2, i3))
                return SlotMachineConfig.AllDifferentSameCategoryMultiplier;

            return 0;
        }

        private static bool IsAllDifferent(int a, int b, int c) => a != b && b != c && a != c;

        private static bool IsAllSameCategory(int i1, int i2, int i3)
        {
            bool food = IsFood(i1) && IsFood(i2) && IsFood(i3);
            bool symbol = IsSymbol(i1) && IsSymbol(i2) && IsSymbol(i3);
            return food || symbol;
        }

        private static bool IsFood(int index) => index is >= 0 and <= 3;
        private static bool IsSymbol(int index) => index is >= 4 and <= 8;
    }
}