using System.Collections.Generic;
using UnityEngine;

namespace BlockTap.Config
{
    [CreateAssetMenu(menuName = "BlockTap/Level Database", fileName = "BlockTapLevelDatabase")]
    public class BlockTapLevelDatabaseSO : ScriptableObject
    {
        [SerializeField] private List<BlockTapLevelConfigSO> levels = new List<BlockTapLevelConfigSO>();

        public IReadOnlyList<BlockTapLevelConfigSO> Levels => levels;
        public int Count => levels.Count;

        public BlockTapLevelConfigSO GetLevel(int levelIndex)
        {
            if (levels.Count == 0)
            {
                return null;
            }

            var clampedIndex = Mathf.Clamp(levelIndex, 0, levels.Count - 1);
            return levels[clampedIndex];
        }

        public void SetLevels(List<BlockTapLevelConfigSO> newLevels)
        {
            levels = newLevels ?? new List<BlockTapLevelConfigSO>();
        }
    }
}
