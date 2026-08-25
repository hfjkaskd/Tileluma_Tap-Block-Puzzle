#if BIZZA_REAL_WITHDRAW
using System;
using System.Collections;
using System.Collections.Generic;
using Bizza.Sdk;
using Cysharp.Threading.Tasks;
using UnityEngine;

public partial class NumbericalStatistics
{
    public static int currentLevel => SaveDataUtils.GameData.playerSelectedLv; // 当前关卡
    public static int CloseGetRewardCount // 关闭多少次恭喜获得界面出现广告
    {
        get
        {
            return ChannelConfig.Instance.real_ADStatistics.CloseGetRewardCount;
        }
    }
    public static int CloseGetRewardNum = 0; // 当前关闭恭喜获得界面次数进度

    public static int ShowGetRewardCount  // 多少次出现恭喜获得界面
    {
        get
        {
            return ChannelConfig.Instance.real_ADStatistics.ShowGetRewardCount;
        }
    }
    public static int ShowGetRewardNum = 0; // 当前出现恭喜获得界面的进度

    public static int ShowDollarCount  // 多少次出现假钞获得界面
    {
        get
        {
            return ChannelConfig.Instance.real_ADStatistics.ShowDollarCount;
        }
    }

    /// <summary>
    /// 合成指定次数打开   恭喜获得界面
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="money"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static bool CheckShowGetReward()
    {
        ShowGetRewardNum++;
        bool show = ShowGetRewardNum >= ShowGetRewardCount;
        LogLogger.LogVerbose(LogTag.ADNumericalStatistics, $"打开恭喜获得界面 - 进度:{ShowGetRewardNum},最大次数:{ShowGetRewardCount},是否显示:{show}");
        if (!show) return false;
        ShowGetRewardNum = 0;
        Real_GetRewardPanelUtil.OpenGetRewardPanel(DoubleGetRewardPanel.E_UseScene.MatchReward);
        return true;
    }

    /// <summary>
    /// 关闭恭喜获得界面一定次数会强  弹插屏
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="money"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static bool CheckCloseGetReward(E_AdPos pos, float money, Action<Bizza.Sdk.ShowAdResult> action)
    {
        CloseGetRewardNum++;
        bool show = CloseGetRewardNum >= CloseGetRewardCount;
        LogLogger.LogVerbose(LogTag.ADNumericalStatistics, $"关闭界面弹插屏 - 进度:{CloseGetRewardNum},最大次数:{CloseGetRewardCount},是否显示:{show}");
        if (!show) return false;
        CloseGetRewardNum = 0;
        BizzaSdk.Ad.ShowInterAd(pos.ToString(), money, action);
        return true;
    }

    /// <summary>
    /// 合成多少次出现Dollar界面
    /// </summary>
    /// <returns></returns>
    public static bool CheckGetDollar()
    {
        bool show = ShowGetRewardNum != 0 && ShowGetRewardNum % ShowDollarCount == 0;
        LogLogger.LogVerbose(LogTag.ADNumericalStatistics, $"出现Dollar界面 - 进度:{ShowGetRewardNum},最大次数:{ShowDollarCount},是否显示:{show}");
        if (!show) return false;

        float moneyValue = WithdrawalUtil.GetDollarCountBtFree();
        ItemEntry dollar = new()
        {
            Type = E_ItemType.Dollar,
            Count = WithdrawalUtil.GetCustomizedFloatByCountryType(moneyValue)
        };

        UIUtils.ShowTips(dollar, default);

        ItemUtils.AddItem(dollar, new AddItemParam
        {
            playAnim = true,
            isAd = false,
            startPos = BroadcastBarController.Instance.entryBImage.transform.position,
            bUiPos = false,
            source = DoubleGetRewardPanel.GetItemSource(DoubleGetRewardPanel.E_UseScene.Ad),
        });
        return true;
    }
}
#endif