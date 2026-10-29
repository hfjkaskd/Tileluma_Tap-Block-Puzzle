#if BIZZA_REAL_WITHDRAW
using System.Collections;
using System.Collections.Generic;
using Bizza.GameAnalytics;
using UnityEngine;

public static class BridgingUtil
{
    // 当前关卡
    public static int GameLevel
    {
        get
        {
#if BIZZA_REAL_WITHDRAW
            return SaveDataUtils.GameData.playerSelectedLv;
#else
           PlayerPrefs.GetInt("GameLevel", 1);
#endif
        }
        set
        {
#if BIZZA_REAL_WITHDRAW
            SaveDataUtils.GameData.playerSelectedLv = value;
#else
            PlayerPrefs.SetInt("GameLevel", value);
#endif
        }
    }

    #region 道具 - 每局使用次数限制初始化 - 新手下载初始化为1 - 使用道具的接口
    public static void PropUseLimitInit() // 每局使用次数限制初始化
    {
#if BIZZA_REAL_WITHDRAW
        NumbericalStatistics.InitProp();
#endif
    }

    public static void NewPlayerPropCountInit() // 新手下载初始化为1
    {
#if BIZZA_REAL_WITHDRAW
        if (SaveDataUtils.GameData.newPlayerGiveProp)
        {
            ItemUtils.AddItem(E_ItemType.GameProp_Back, 1);
            ItemUtils.AddItem(E_ItemType.GameProp_ClearArea, 1);
            ItemUtils.AddItem(E_ItemType.GameProp_Refresh, 1);

            SaveDataUtils.GameData.newPlayerGiveProp = false;
            SaveDataUtils.Save();
        }
#endif
    }

    public static void PropUse_1() // 使用道具的接口 - 直接使用不用计数
    {
        BlockTap.Gameplay.BlockTapGameplayController.Instance.OnShufflePressed();
    }

    public static void PropUse_2() // 使用道具的接口
    {
        BlockTap.Gameplay.BlockTapGameplayController.Instance.OnBombPressed();
    }

    public static void PropUse_3() // 使用道具的接口
    {

    }

    #endregion

    #region 新手引导 - 游戏加载完毕可以显示新手引导 - 白包新手引导结束

    public static void CanShowGuide() // 游戏加载完毕可以显示新手引导
    {
#if BIZZA_REAL_WITHDRAW
        SaveDataUtils.GameData.customTutorialCanPlay = true;
#endif
    }

    public static void CannotShowGuide() // 游戏重建棋盘时，暂时不允许显示新手引导
    {
#if BIZZA_REAL_WITHDRAW
        SaveDataUtils.GameData.customTutorialCanPlay = false;
#endif
    }

    public static void NewPlayerGuideEnd() // 白包新手引导结束
    {
#if BIZZA_REAL_WITHDRAW
        GameMode_GamePlay.Instance.OnSetCustomTutorialState(true);
        BizzaGameAnalytics.TrackTutorialComplete("main");
#endif
    }
    #endregion

    #region 游戏重开 - 游戏下一关 - 游戏胜利 - 游戏失败

    public static void ReloadCurrentLevel() // 游戏重开
    {
        PropUseLimitInit();
        BlockTap.Gameplay.BlockTapGameplayController.Instance.OnRestartPressed();
    }

    public static void LoadNextLevel() // 游戏下一关
    {
        PropUseLimitInit();
        BlockTap.Gameplay.BlockTapGameplayController.Instance.OnNextPressed();
    }

    public static void GameWin() // 游戏胜利 - 当打开游戏胜利界面时的逻辑 现在只进行了关卡数值 +1
    {
        GameLevel++;
    }

    public static void GameOver() // 游戏失败 - 打开游戏失败时的逻辑
    {

    }
    #endregion
}
#endif
