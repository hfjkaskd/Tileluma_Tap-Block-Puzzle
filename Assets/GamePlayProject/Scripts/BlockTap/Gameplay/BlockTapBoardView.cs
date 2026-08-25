using UnityEngine;

namespace BlockTap.Gameplay
{
    public class BlockTapBoardView : MonoBehaviour
    {
        [Header("Scene Roots")]
        [SerializeField] private RectTransform boardRoot;
        [SerializeField] private RectTransform blockContainer;
        [SerializeField] private RectTransform effectsRoot;
        [SerializeField] private RectTransform flyingContainer;

        [Header("Dynamic Prefabs")]
        [SerializeField] private BlockTapBlockView blockPrefab;

        public RectTransform BoardRoot => boardRoot;
        public RectTransform BlockContainer => blockContainer != null ? blockContainer : boardRoot;
        public RectTransform EffectsRoot => effectsRoot != null ? effectsRoot : boardRoot;
        public RectTransform FlyingContainer => flyingContainer != null ? flyingContainer : EffectsRoot;
        public BlockTapBlockView BlockPrefab => blockPrefab;

        private void Awake()
        {
            boardRoot = boardRoot != null ? boardRoot : GetComponent<RectTransform>();
        }

        public void SetBoardSize(Vector2 size)
        {
            if (boardRoot != null)
            {
                boardRoot.sizeDelta = size;
            }
        }
    }
}
