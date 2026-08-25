using System.Collections.Generic;
using BlockTap.Config;
using UnityEngine;

namespace BlockTap.Gameplay
{
    public class BoardGenerator
    {
        private readonly BlockTapGameplayConfigSO gameplayConfig;

        public BoardGenerator(BlockTapGameplayConfigSO gameplayConfig)
        {
            this.gameplayConfig = gameplayConfig;
        }

        public List<int> BuildDropColorPool(IList<int> targetColors)
        {
            var pool = new List<int>(gameplayConfig.totalColors);
            for (var colorIndex = 0; colorIndex < gameplayConfig.totalColors; colorIndex++)
            {
                pool.Add(colorIndex);
            }

            return pool;
        }

        public List<int> PickTargetColors(int targetTypeCount)
        {
            var result = new List<int>();
            while (result.Count < targetTypeCount)
            {
                var candidate = Random.Range(0, gameplayConfig.totalColors);
                if (!result.Contains(candidate))
                {
                    result.Add(candidate);
                }
            }

            return result;
        }

        public int PickColor(IReadOnlyList<int> dropColorPool)
        {
            return dropColorPool[Random.Range(0, dropColorPool.Count)];
        }
    }
}
