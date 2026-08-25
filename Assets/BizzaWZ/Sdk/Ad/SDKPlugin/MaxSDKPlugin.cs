using System;
using System.Collections;
using System.Collections.Generic;
using Bizza.Sdk;
using Cysharp.Threading.Tasks;
using LitMotion;
using UnityEngine;

namespace Bizza.Sdk
{
    [Obfuz.ObfuzIgnore]
    public interface SDKPlugin
    {
        bool Inited { get; }
        E_AdsSource Name { get; }

        bool CanPlayRewardAd { get; }
        bool CanPlayIntAd { get; }

        VideoAdAdapterBase InterAdAdapter { get; }
        VideoAdAdapterBase RewardAdAdapter { get; }
        /// <summary> 开屏广告适配器，未配置开屏时可为 null </summary>
        VideoAdAdapterBase SplashAdAdapter { get; }

        void Init(E_AdsSource name);
        void SetUserId(string userId);

        void LoadAds(E_AdType adType);

        void ShowBanner();
        void HideBanner();

        bool IsEmpty();
    }
}

[Obfuz.ObfuzIgnore]
public enum E_AdType
{
    All,
    InsertAd,
    RewardAd,
    SplashAd,
    Banner,
    Native,
}

#if BIZZA_ENABLE_MAX

[Obfuz.ObfuzIgnore]
public class MaxSDKPlugin : SDKPlugin
{
    private MaxInsertAdAdapter _maxInsertAd = new();
    private MaxRewardAdAdapter _maxRewardAd = new();
    private MaxSplashAdAdapter _maxSplashAd;

    public VideoAdAdapterBase InterAdAdapter => _maxInsertAd;
    public VideoAdAdapterBase RewardAdAdapter => _maxRewardAd;
    public VideoAdAdapterBase SplashAdAdapter => _maxSplashAd;

    public bool CanPlayIntAd => _maxInsertAd.IsReady;
    public bool CanPlayRewardAd => _maxRewardAd.IsReady;
    private bool _inited = false;
    public bool Inited
    {
        get => _inited;
    }

    private E_AdsSource name;
    public E_AdsSource Name => name;

    [Obfuz.ObfuzIgnore]
    public void Init(E_AdsSource name)
    {
        this.name = name;
        this._inited = false;
        string[] devices = new string[] { };

        if (ChannelConfig.Instance.real_CustomConfig.openTestDevice)
        {
#if DEBUG_MODE
            devices = ChannelConfig.Instance.real_CustomConfig.testDeviceIds;
#endif
        }
        MaxSdk.SetTestDeviceAdvertisingIdentifiers(devices);
        MaxSdk.SetHasUserConsent(true);
        MaxSdk.SetVerboseLogging(false);
        LogLogger.LogInfo(LogTag.ADReportFlow, "初始化MaxSdk-开始");
        MaxSdkCallbacks.OnSdkInitializedEvent += (x) =>
        {
            LogLogger.LogInfo(LogTag.ADReportFlow, "初始化MaxSdk-完成");
            if (_maxInsertAd == null) _maxInsertAd = new MaxInsertAdAdapter();
            if (_maxRewardAd == null) _maxRewardAd = new MaxRewardAdAdapter();

            var rewardAdUnitId = ChannelConfig.Instance.GetAdsConfig(E_AdsSource.Max).rewardAdId;
            _maxRewardAd.Init(rewardAdUnitId);
            var interAdUnitId = ChannelConfig.Instance.GetAdsConfig(E_AdsSource.Max).interAdId;
            _maxInsertAd.Init(interAdUnitId);
            var openAdId = ChannelConfig.Instance.GetAdsConfig(E_AdsSource.Max).openAdId;
            if (!string.IsNullOrEmpty(openAdId))
            {
                _maxSplashAd = new MaxSplashAdAdapter();
                _maxSplashAd.Init(openAdId);
            }

            _inited = true;
            LoadAd(E_AdType.All).Forget();

            LMotion.Create(0, 0, 1).WithScheduler(MotionScheduler.InitializationIgnoreTimeScale).WithOnComplete(() =>
            {
                // canPlayAd = true;
            }).RunWithoutBinding();

            LMotion.Create(0, 0, 1).WithScheduler(MotionScheduler.InitializationIgnoreTimeScale).WithOnComplete(() =>
            {
                //BannerAd.InitBanner();
            }).RunWithoutBinding();

            LMotion.Create(0, 0, 2).WithScheduler(MotionScheduler.InitializationIgnoreTimeScale).WithOnComplete(() =>
            {
                //  BannerAd.ShowBanner();
            }).RunWithoutBinding();
        };

        MaxSdk.InitializeSdk();
    }

    public void SetUserId(string userId)
    {
        MaxSdk.SetUserId(userId);
    }


    public void LoadAds(E_AdType adType)
    {
        LoadAd(adType).Forget();
    }


    #region 加载广告
    private async UniTask LoadAd(E_AdType adType)
    {
        LogLogger.LogVerbose(LogTag.ADReportFlow, $"Plugin加载广告:{adType}");
        if (_inited)
        {
            switch (adType)
            {
                case E_AdType.All:
                    LoadRewardAd();
                    LoadInserAd();
                    LoadSplashAd();
                    break;
                case E_AdType.InsertAd:
                    LoadInserAd();
                    break;
                case E_AdType.RewardAd:
                    LoadRewardAd();
                    break;
                case E_AdType.SplashAd:
                    LoadSplashAd();
                    break;
            }
        }
        else
        {
            await UniTask.Delay(1000); // 延迟1秒
            LoadAd(adType);
        }
    }

    private void LoadRewardAd()
    {
        LogLogger.LogInfo(LogTag.ADReportFlow, "加载MAX Reward... ");
        _maxRewardAd.Load();
    }
    private void LoadInserAd()
    {
        LogLogger.LogInfo(LogTag.ADReportFlow, "加载MAX Insert... ");
        _maxInsertAd.Load();

    }

    private void LoadSplashAd()
    {
        if (_maxSplashAd == null) return;
        LogLogger.LogInfo(LogTag.ADReportFlow, "加载MAX 开屏... ");
        _maxSplashAd.Load();
    }
    #endregion


    #region 播放广告
    // /// <summary>
    // /// 播放激励广告
    // /// </summary>
    // /// <param name="adId"></param>
    // /// <param name="success"></param>
    // /// <param name="fail"></param>
    // public void ShowRewardAd(ShowAdArgs args)
    // {
    //     _maxRewardAd.ShowAds(args);
    // }
    //
    // /// <summary>
    // /// 播放插屏广告
    // /// </summary>
    // /// <param name="adId"></param>
    // /// <param name="success"></param>
    // /// <param name="fail"></param>
    // public void ShowInterstitialAd(E_AdPos pos, string adId, Action success, Action fail = null)
    // {
    //     _maxInsertAd.ShowAds(pos, adId, success, fail);
    // }

    public void ShowBanner()
    {

    }

    public void HideBanner()
    {

    }
    #endregion

    /// <summary>
    /// 判断本轮中 该SDK是否有广告播放
    /// </summary>
    /// <returns></returns>
    public bool IsEmpty()
    {
        bool isEmpty = string.IsNullOrEmpty(_maxInsertAd.AdUnitId) && string.IsNullOrEmpty(_maxRewardAd.AdUnitId);
        return isEmpty;
    }
}

#endif