using UnityEngine;

namespace BlockTap.Config
{
    [CreateAssetMenu(menuName = "BlockTap/Visual Config", fileName = "BlockTapVisualConfig")]
    public class BlockTapVisualConfigSO : ScriptableObject
    {
        [Header("Palette")]
        public Sprite[] blockSprites = new Sprite[7];
        public Sprite[] goalSprites = new Sprite[7];

        [Header("Layout")]
        public Vector2 referenceResolution = new Vector2(1080f, 1920f);
        public float boardCellSize = 110f;
        public float boardSpacing = 12f;

        [Header("Animation")]
        public float spawnDuration = 0.30f;
        public float spawnColumnDelay = 0.02f;
        public float spawnRowDelay = 0.02f;
        public float moveDuration = 0.18f;
        public float collectFlyDuration = 0.35f;
        public float removePunchDuration = 0.12f;
        public float boardSlideDuration = 0.45f;
        public float bombPulseDuration = 0.85f;
    }
}
