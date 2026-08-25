using System;
using Bizza.Sdk;
using UnityEngine;

namespace BizzaSdk
{
    /// <summary>
    /// 广告 SDK 统一入口，封装 Platform.Instance.AdSdk 的调用。
    /// </summary>
    public static class Ad
    {
        public static bool Inited => Platform.Instance.AdSdk.Inited;

        private static AggregationAdSdk AdSdk => Platform.Instance?.AdSdk;

        public static void SetUserId(string userId) => AdSdk?.SetUserId(userId);

        public static string curRewardAdPos = "";
        public static string curInterAdPos = "";

        #region 开屏
        public static bool IsSplashInvalid => AdSdk != null && AdSdk.IsSplashInvalid;
        public static bool IsSplashReady => AdSdk != null && AdSdk.IsSplashReady;
        public static bool IsSplashShowing => AdSdk != null && AdSdk.IsSplashShowing;
        public static void ShowSplashAd() => AdSdk?.ShowSplashAd();
        #endregion

        #region 激励视频
        public static bool IsRewardInvalid => AdSdk != null && AdSdk.IsRewardInvalid;
        public static bool IsRewardReady => AdSdk != null && AdSdk.IsRewardReady;
        public static bool IsRewardShowing => AdSdk != null && AdSdk.IsRewardShowing;
        public static bool CheckRewardAdReady(CheckAdReadyArgs args = default) => AdSdk != null && AdSdk.CheckRewardAdReady(args);
        public static void ShowRewardAd(ShowAdArgs args) => AdSdk?.ShowRewardAd(args);
        public static void ShowRewardAd(string adPos, float dollarNum, Action<Bizza.Sdk.ShowAdResult> onFinish, float ecpmLimit = 0)
        {
            var showAdArgs = new Bizza.Sdk.ShowAdArgs()
            {
                adSource = E_AdsSource.Max,
                onFinish = onFinish,
                ecpmLimit = ecpmLimit,
                adPos = adPos.ToString(),
                fakeDollarNum = dollarNum
            };
            ShowRewardAd(showAdArgs);
        }
        public static void ShowRewardAd(Action<Bizza.Sdk.ShowAdResult> onFinish) => ShowRewardAd(new ShowAdArgs(){onFinish = onFinish});
        #endregion

        #region 插屏/插页
        public static bool IsInterInvalid => AdSdk != null && AdSdk.IsInterInvalid;
        public static bool IsInterReady => AdSdk != null && AdSdk.IsInterReady;
        public static bool IsInterShowing => AdSdk != null && AdSdk.IsInterShowing;
        public static bool CheckInterAdReady(CheckAdReadyArgs args = default) => AdSdk != null && AdSdk.CheckInterAdReady(args);
        public static void ShowInterAd(ShowAdArgs args = default) => AdSdk?.ShowInterAd(args);
        public static void ShowInterAd(string adPos, float dollarNum, Action<Bizza.Sdk.ShowAdResult> onFinish)
        {
            var showAdArgs = new ShowAdArgs()
            {
                adSource = E_AdsSource.Max,
                onFinish = onFinish,
                adPos = adPos.ToString(),
                fakeDollarNum = dollarNum
            };
            ShowInterAd(showAdArgs);
        }
        #endregion

        #region Banner
        public static bool IsBannerInvalid => AdSdk != null && AdSdk.IsBannerInvalid;
        public static bool IsBannerReady => AdSdk != null && AdSdk.IsBannerReady;
        public static bool IsBannerShowing => AdSdk != null && AdSdk.IsBannerShowing;
        public static void ShowBanner() => AdSdk?.ShowBanner();
        public static void HideBanner() => AdSdk?.HideBanner();
        #endregion

        #region Native
        public static bool IsNativeValid => AdSdk != null && AdSdk.IsNativeValid;
        public static bool IsNativeReady => AdSdk != null && AdSdk.IsNativeReady;
        public static bool IsNativeShowing => AdSdk != null && AdSdk.IsNativeShowing;
        public static void ShowNative() => AdSdk?.ShowNative();
        public static void HideNative() => AdSdk?.HideNative();
        #endregion

        /// <summary>
        /// 获取广告 SDK 调试信息（各适配器 UnitId、就绪状态、ECPM 等）。
        /// </summary>
        public static string GetAdSdkDebugInfo()
        {
            if (AdSdk == null) return "AdSdk 未初始化";
            return AdSdk.GetAdSdkDebugInfo();
        }
    }
}
