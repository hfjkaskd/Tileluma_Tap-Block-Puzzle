using UnityEngine;

namespace BlockTap.Gameplay
{
    public class PropController
    {
        public enum PropType
        {
            Shuffle,
            Bomb,
        }

        private const string ShuffleCountKey = "BlockTap.Prop0.Count";
        private const string BombCountKey = "BlockTap.Prop1.Count";

        public int ShuffleCount { get => (int)ItemUtils.GetItemCount(E_ItemType.GameProp_Back); }
        public int BombCount { get => (int)ItemUtils.GetItemCount(E_ItemType.GameProp_ClearArea); }
        public bool BombSelectionActive { get; private set; }

        public void Initialize(int defaultShuffleCount, int defaultBombCount)
        {
            // ShuffleCount = (int)ItemUtils.GetItemCount(E_ItemType.GameProp_Back);
            // BombCount = (int)ItemUtils.GetItemCount(E_ItemType.GameProp_ClearArea);
            BombSelectionActive = false;
            SaveCounts();
        }

        public void ResetSelectionState()
        {
            BombSelectionActive = false;
        }

        public bool TryUseShuffle()
        {
            if (ShuffleCount <= 0)
            {
                return false;
            }

            //ShuffleCount--;
            BombSelectionActive = false;
            SaveCounts();
            return true;
        }

        public bool TryArmBomb()
        {
            if (BombCount <= 0 || BombSelectionActive)
            {
                return false;
            }

            //BombCount--;
            BombSelectionActive = true;
            SaveCounts();
            return true;
        }

        public void ConsumeBombSelection()
        {
            BombSelectionActive = false;
        }

        public void AddProp(PropType propType, int amount)
        {
            // if (amount <= 0)
            // {
            //     return;
            // }

            // switch (propType)
            // {
            //     case PropType.Shuffle:
            //         ShuffleCount += amount;
            //         break;
            //     case PropType.Bomb:
            //         BombCount += amount;
            //         break;
            // }

            //SaveCounts();
        }

        private void SaveCounts()
        {
            // PlayerPrefs.SetInt(ShuffleCountKey, ShuffleCount);
            // PlayerPrefs.SetInt(BombCountKey, BombCount);
            // ItemUtils.AddItem(E_ItemType.GameProp_Back, ShuffleCount, false);
            // ItemUtils.AddItem(E_ItemType.GameProp_ClearArea, ShuffleCount, false);
            // PlayerPrefs.Save();
        }
    }
}
