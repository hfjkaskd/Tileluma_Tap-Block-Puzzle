using UnityEngine;

namespace BlockTap.Config
{
    [CreateAssetMenu(menuName = "BlockTap/Prefab Config", fileName = "BlockTapPrefabConfig")]
    public class BlockTapPrefabConfigSO : ScriptableObject
    {
        public GameObject blockCellPrefab;
        public GameObject goalItemPrefab;
        public GameObject flyingBlockPrefab;
        public GameObject bombEffectPrefab;
    }
}
