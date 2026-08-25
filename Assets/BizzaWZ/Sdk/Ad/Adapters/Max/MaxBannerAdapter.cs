#if BIZZA_ENABLE_MAX
using System;
using UnityEngine;

namespace Bizza.Sdk
{
     
    public class MaxBannerAdapter : BannerAdapterBase
    {
        private string _bannerAdUnitId;
        private bool _isBannerLoaded;
        private bool _isBannerShowing;

        public override E_AdType adType => E_AdType.Banner;
        public override string AdUnitId => _bannerAdUnitId ?? "";
        public override bool IsValid => !string.IsNullOrEmpty(_bannerAdUnitId);
        public override bool IsReady => _isBannerLoaded;
        public override bool IsShowing => _isBannerShowing;

        public override void Init(string adUnitId)
        {
            if (string.IsNullOrEmpty(adUnitId))
                return;
            _bannerAdUnitId = adUnitId;

            MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
            MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdLoadFailedEvent;
            MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
            MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;
            MaxSdkCallbacks.Banner.OnAdExpandedEvent += OnBannerAdExpandedEvent;
            MaxSdkCallbacks.Banner.OnAdCollapsedEvent += OnBannerAdCollapsedEvent;

            MaxSdk.CreateBanner(_bannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);
            MaxSdk.SetBannerBackgroundColor(_bannerAdUnitId, Color.black);
        }

        public override void Load()
        {
            if (string.IsNullOrEmpty(_bannerAdUnitId)) return;
            MaxSdk.LoadBanner(_bannerAdUnitId);
        }

        public override void Show()
        {
            if (!string.IsNullOrEmpty(_bannerAdUnitId))
                MaxSdk.ShowBanner(_bannerAdUnitId);
            _isBannerShowing = true;
        }

        public override void Hide()
        {
            if (!string.IsNullOrEmpty(_bannerAdUnitId))
                MaxSdk.HideBanner(_bannerAdUnitId);
            _isBannerShowing = false;
        }

        #region 回调

        private void OnBannerAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            _isBannerLoaded = true;
        }

        private void OnBannerAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            _isBannerLoaded = false;
        }

        private void OnBannerAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

        private void OnBannerAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

        private void OnBannerAdExpandedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

        private void OnBannerAdCollapsedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

        #endregion
    }
}
#endif
