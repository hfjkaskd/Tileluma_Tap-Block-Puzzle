using System.Collections.Generic;
namespace BlockTap.Gameplay
{
    public class LevelGoalTracker
    {
        private readonly Dictionary<int, int> remainingByColor = new Dictionary<int, int>();
        private readonly Dictionary<int, int> targetByColor = new Dictionary<int, int>();
        private readonly List<int> orderedTargetColors = new List<int>();

        public IReadOnlyDictionary<int, int> RemainingByColor => remainingByColor;
        public IReadOnlyDictionary<int, int> TargetByColor => targetByColor;
        public IReadOnlyList<int> OrderedTargetColors => orderedTargetColors;

        public void Configure(BlockTap.Config.BlockTapLevelConfigSO level, IList<int> targetColors)
        {
            remainingByColor.Clear();
            targetByColor.Clear();
            orderedTargetColors.Clear();

            if (level == null || level.collect == null || targetColors == null)
            {
                return;
            }

            var targetCount = System.Math.Min(targetColors.Count, level.collect.Length);
            for (var i = 0; i < targetCount; i++)
            {
                var colorIndex = targetColors[i];
                var remainingCount = level.collect[i];
                orderedTargetColors.Add(colorIndex);
                remainingByColor[colorIndex] = remainingCount;
                targetByColor[colorIndex] = remainingCount;
            }
        }

        public bool ConsumeColor(int colorIndex)
        {
            if (!remainingByColor.ContainsKey(colorIndex))
            {
                return false;
            }

            remainingByColor[colorIndex] = remainingByColor[colorIndex] > 0 ? remainingByColor[colorIndex] - 1 : 0;
            return true;
        }

        public int GetRemaining(int colorIndex)
        {
            return remainingByColor.TryGetValue(colorIndex, out var remaining) ? remaining : 0;
        }

        public int GetCollected(int colorIndex)
        {
            if (!targetByColor.TryGetValue(colorIndex, out var target))
            {
                return 0;
            }

            return target - GetRemaining(colorIndex);
        }

        public bool IsTargetColor(int colorIndex)
        {
            return targetByColor.ContainsKey(colorIndex);
        }

        public bool IsComplete()
        {
            foreach (var pair in remainingByColor)
            {
                if (pair.Value > 0)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
