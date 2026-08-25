using UnityEngine;

namespace BlockTap.Config
{
    [CreateAssetMenu(menuName = "BlockTap/Gameplay Config", fileName = "BlockTapGameplayConfig")]
    public class BlockTapGameplayConfigSO : ScriptableObject
    {
        [Header("Board")]
        public int columns = 7;
        public int rows = 7;
        public int totalColors = 7;
        public int minMatchCount = 2;

        [Header("Progression")]
        public int effectiveEliminationsPerRhythmBeat = 7;
        public int startingShuffleProps = 3;
        public int startingBombProps = 3;
        public float nextLevelDelay = 0.8f;
    }
}
