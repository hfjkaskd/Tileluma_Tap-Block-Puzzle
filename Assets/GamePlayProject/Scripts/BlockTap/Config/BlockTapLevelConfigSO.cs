using UnityEngine;

namespace BlockTap.Config
{
    [CreateAssetMenu(menuName = "BlockTap/Level Config", fileName = "BlockTapLevelConfig")]
    public class BlockTapLevelConfigSO : ScriptableObject
    {
        public int pass;
        public int type;
        public int num;
        public int[] collect;
    }
}
