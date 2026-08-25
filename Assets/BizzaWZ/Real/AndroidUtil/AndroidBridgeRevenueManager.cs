#if BIZZA_REAL_WITHDRAW
using System;
using System.Collections;
using System.Collections.Generic;
using Bizza.Unity.Android;
using UnityEngine;
using Bizza.Sdk;
#if BIZZA_ENABLE_MAX
public class AndroidBridgeRevenueManager
{
    public static string ClassName = "com.bangsawan.oceanshine.MBridgeRevenueUtil";
    public static string MethodName = "OnAdRevenuePaid";

    public static void OnReportRevenue(
        string ATTRIBUTION_PLATFORM,
        string adid,
        string adinfo,
        string waterfallInfo,
        string revenuePrecision,
        double revenue)
    {
        // null 安全兜底
        ATTRIBUTION_PLATFORM = ATTRIBUTION_PLATFORM ?? "";
        adid = adid ?? "";
        adinfo = adinfo ?? "";
        waterfallInfo = waterfallInfo ?? "";
        revenuePrecision = revenuePrecision ?? "";

        LogLogger.LogAttribute(
            $"MTrackBridge上报收益 revenue:{revenue}; adJustAdidKey:{PlayerPrefs.GetString(AccountModule.adJustAdidKey)}; adInfo:{adinfo}; adInfo.WaterfallInfo:{waterfallInfo}; adInfo.RevenuePrecision:{revenuePrecision}"
        );

#if UNITY_ANDROID && !UNITY_EDITOR
    JavaBridgeUtils.CallStaticMethodCacheClass(
        ClassName,
        MethodName,
        ATTRIBUTION_PLATFORM,
        adid,
        adinfo,
        waterfallInfo,
        revenue,
        revenuePrecision
    );
#else
        // 非 Android 环境
#endif
    }

    public static string inMobiClassName = "com.bangsawan.oceanshine.IBridgeRevenueUtil";
    public static string inMobiMethodName = "OnAdRevenuePaidJson";

    public static void OnInMobiReportRevenue(MaxSdkBase.AdInfo adInfo)
    {
        try
        {
            adInfo = adInfo ?? null;

#if UNITY_ANDROID && !UNITY_EDITOR
        // 调用 Java 静态方法
        CallStaticMethodCacheClass(adInfo);
#else
            // 非 Android 环境
            Debug.Log("非 Android 环境，跳过调用");
#endif
        }
        catch (Exception e)
        {
            LogLogger.LogError("激励 AndroidBridgeRevenueManager失败: " + e.Message);
        }
    }

    // 调用 Java 静态方法
    public static void CallStaticMethodCacheClass(MaxSdkBase.AdInfo adInfo)
    {
#if UNITY_EDITOR || !UNITY_ANDROID
        return;
#endif
        if (adInfo == null)
        {
            Debug.LogWarning("CallStaticMethodCacheClass failed: adInfo is null");
            return;
        }

        try
        {
            var payload = new AdInfoPayload
            {
                dir_r = adInfo.Revenue,
                dir_nn = adInfo.NetworkName ?? string.Empty,
                dir_type = string.IsNullOrEmpty(adInfo.AdFormat)
                    ? string.Empty
                    : adInfo.AdFormat.ToUpperInvariant()
            };

            string adInfoJson = JsonUtility.ToJson(payload);

            LogLogger.LogAttribute($"InMobi上报收益: revenue:{adInfo.Revenue} Json:{adInfoJson}");

            JavaBridgeUtils.CallStaticMethodCacheClass(
                inMobiClassName,
                inMobiMethodName,
                adInfoJson
            );
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    [Serializable]
    private class AdInfoPayload
    {
        public double dir_r;
        public string dir_nn;
        public string dir_type;
    }
}
#endif
#endif