using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RealWzAnalytics
{
    private const string eventName_Tutoriall = "Tutoriall_Event";
    private const string eventName_Withdrawal = "Withdrawal_Event";
    private const string eventName_GameFlow = "GameFlow_Event";
    private const string eventName_Revenue = "Revenue_Event";
    private const string eventName_Performance = "User_Performance";


    #region 事件类型
    public const string eventType_OpenPanle = "打开界面";
    public const string eventType_Revenue_AdjustAttiribute = "Adjust归因";

    // 性能
    public const string eventType_Performance = "性能";

    // 收益
    public const string eventType_UserGetRevenue = "用户获取收益";

    // 游戏流程
    public const string eventType_SynthesisFull = "SynthesisFull";
    public const string eventType_SynthesisCreate = "SynthesisCreate";
    public const string eventType_PropUse = "PropUse";


    public const string eventType_LookDialyMissionAd = "点击每日任务广告";
    #endregion

    private static Dictionary<string, object> props = new();

    public static void AddDict(string key, object info)
    {
        if (string.IsNullOrWhiteSpace(key))
            return;

        props[key] = info;
    }

    public static void ClearDict()
    {
        props.Clear();
    }

    private static void TrackBiz(string eventName)
    {
        if (string.IsNullOrWhiteSpace(eventName) || props.Count == 0)
        {
            props.Clear();
            return;
        }

        try
        {
            BizzaAnalyticsAgent.Track(eventName, props);
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
        }

        props.Clear();
    }

    public static void TrackBiz_Tutoriall(string tag = "") // 新手引导
    {
        TrackBiz(eventName_Tutoriall + $"_{tag}");
    }

    public static void TrackBiz_Withdrawal(string tag = "") // 提现
    {
        TrackBiz(eventName_Withdrawal + $"_{tag}");
    }

    public static void TrackBiz_GameFlow(string tag = "") // 游戏流程
    {
        TrackBiz(eventName_GameFlow + $"_{tag}");
    }

    public static void TrackBiz_Revenue(string tag = "") // 收益
    {
        TrackBiz(eventName_Revenue + $"_{tag}");
    }

    public static void TrackBiz_Performance(string tag = "") // 性能
    {
        TrackBiz(eventName_Performance + $"_{tag}");
    }
}