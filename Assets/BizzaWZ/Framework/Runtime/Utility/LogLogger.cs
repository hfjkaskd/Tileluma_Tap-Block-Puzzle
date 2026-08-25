using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;



 
public static class LogTag
{
    #region 日志

    public const string LOGLevl_Audit = "[测试日志]";
    public const string LOGLevl_Develop = "[流程日志]";

    // 测试日志类型 ===============

    public const string LOG_Attribute = "[归因]";
    public const string LOG_Issuing = "[发行参数]"; // 
    public const string LOG_PlatformInit = "[平台初始化]";
    public const string LOG_ADPlayReport = "[广告播放上报]";
    public const string LOG_UserInstall = "[用户安装]";
    public const string LOG_Country = "[国家选择]";
    public const string LOG_GameStart = "[游戏启动]";
    public const string LOG_HTTPInfo = "[HTTP信息]";

    // ===================

    // 流程日志类型 ===============

    public const string LOG_NumericalShow = "[数值显示]";
    public const string LOG_NewTutorial = "[新手教程]";
    public const string LOG_DailyMission = "[每日任务]";
    public const string LOG_UserInfoAlter = "[用户信息变更]";

    // ===================

    public const string LOG_Asset = "AssetPro_";
    public const string LOG_HTTP = "HTTP_";
    public const string LOG_Account = "Account_";
    public const string LOG_Device = "Device_";
    public const string Frame = "[关键流程]";
    public const string ADAttribute = "广告归因_";
    public const string ADReportFlow = "广告播放流程_";
    public const string ADNumericalStatistics = "广告次数统计_";

    public const string DailyAD = "每日任务_";

    public const string LOG_Game = "GAME_";
    public const string Log_Prop = "Prop_";
    public const string LOG_Load =  "LOAD_";

    public const string LOG_Pool = "Pool";
    #endregion
}


public static class LogLogger
{
    public const string LogMode = "DEBUG_MODE";
    public static bool AdLogIsError = false;

    public static void LogAssert(bool condition, string text)
    {
        if (!condition)
        {
            Debug.LogError(text);
        }
    }

    public static void LogAuditInfo(string tag, string text)
    {
        string content = $"{LogTag.LOGLevl_Audit} {tag} {text}";
        Debug.Log(content);
    }

    [Conditional(LogMode)]
    public static void LogAdInfo(string text)
    {
        if (AdLogIsError)
        {
            Debug.LogError($"[广告SDk流程_] {text}");
        }
        else
        {
            Debug.Log($"[广告SDk流程_] {text}");
        }
    }

    [Conditional(LogMode)]
    public static void LogVerbose(string text)
    {
        Debug.Log(text);
    }

    [Conditional(LogMode)]
    public static void LogVerbose(string tag, string text)
    {
        Debug.Log($"[{tag}] {text}");
    }
    
    [Conditional(LogMode)]
    public static void LogInfo(string tag, string text)
    {
        Debug.Log($"[{tag}] {text}");
    }
    
    [Conditional(LogMode)]
    public static void LogInfo(string text)
    {
        Debug.Log(text);
    }

    [Conditional(LogMode)]
    public static void LogBus(string text)
    {
        Debug.Log("[BusAway] " + text);
    }

    public static void LogError(string tag, string text)
    {
        LogError($"[{tag}] {text}");
    }

    public static void LogError(object text)
    {
        Debug.LogError(text);
    }

    #region 测试日志

    public static void LogAttribute(string text) // 广告归因 - Adjust初始化 adjust MTG Inmobi
    {
        Debug.Log($"{LogTag.LOGLevl_Audit}{LogTag.LOG_Attribute} {text}");
    }

    public static void LogIssuing(string text) // 发行参数 - 
    {
        Debug.Log($"{LogTag.LOGLevl_Audit}{LogTag.LOG_Issuing} {text}");
    }

    public static void LogPlatformInit(string text) // 平台初始化 - 
    {
        Debug.Log($"{LogTag.LOGLevl_Audit}{LogTag.LOG_PlatformInit} {text}");
    }

    public static void LOGUserInstall(string text) // 用户安装
    {
        Debug.Log($"{LogTag.LOGLevl_Audit}{LogTag.LOG_UserInstall} {text}");
    }

    public static void LOGCountry(string text) // 国家选择
    {
        Debug.Log($"{LogTag.LOGLevl_Audit}{LogTag.LOG_Country} {text}");
    }
    
    public static void LOGGameStart(string text) // 游戏启动
    {
        Debug.Log($"{LogTag.LOGLevl_Audit}{LogTag.LOG_GameStart} {text}");
    }
    
    public static void LOGHTTPInfo(string text) // 游戏启动
    {
        Debug.Log($"{LogTag.LOGLevl_Audit}{LogTag.LOG_HTTPInfo} {text}");
    }
    
    public static void LogADPR(string text) // 广告播放上报
    {
        Debug.Log($"{LogTag.LOGLevl_Audit}{LogTag.LOG_ADPlayReport} {text}");
    }

    [Conditional(LogMode)]
    public static void LogHttpInfo(string tag, string url, string text) // HTTP请求
    {
        Debug.Log($"HTTP数据内容: {tag} - {url} - {text}");
    }

    #endregion

    #region 流程日志

    public static void LOGNumericalShow(string tag, string text) // 数值显示
    {
        Debug.Log($"{LogTag.LOGLevl_Develop}{tag} {text}");
    }

    public static void LOGNewTutorial(string text) // 新手引导
    {
        Debug.Log($"{LogTag.LOGLevl_Develop}{LogTag.LOG_NewTutorial} {text}");
    }

    public static void LOGDailyMission(string text) // 每日任务
    {
        Debug.Log($"{LogTag.LOGLevl_Develop}{LogTag.LOG_DailyMission} {text}");
    }

    public static void LOGUserInfoAlter(string text) // 用户信息变更
    {
        Debug.Log($"{LogTag.LOGLevl_Develop}{LogTag.LOG_UserInfoAlter} {text}");
    }

    #endregion
}
