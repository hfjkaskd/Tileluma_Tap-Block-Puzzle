using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bizza.Sdk
{
    public abstract class InstanceClass<T> where T : InstanceClass<T>
    {
        public static T Instance { get; private set; }

        public InstanceClass()
        {
            Instance = this as T;
        }
    }

    public struct CheckAdReadyArgs
    {
        public float ecpmLimit;
        public bool excludeReplenish;
    }

    public struct ShowAdArgs
    {
        public E_AdsSource adSource;
        public Action<Bizza.Sdk.ShowAdResult> onFinish;
        public Action onFailed;
        public float ecpmLimit;
        public string adPos;

        // 真网赚
        public float fakeDollarNum;
    }

    public struct ShowAdResult
    {
        public bool success;

#if BIZZA_REAL_WITHDRAW
        public AccountModule.OceanShineAdRevenueResponse response;

        public ShowAdResult(bool success, AccountModule.OceanShineAdRevenueResponse response)
        {
            this.success = success;
            this.response = response;
        }
        public ShowAdResult Default()
        {
            return new ShowAdResult(false, null);
        }
#endif
    }

 [Obfuz.ObfuzIgnore]
    public enum AdType
    {
        Reward,
        Interstitial,
        Banner,
    }

    /// <summary>
    ///
    /// </summary>
    public abstract class Platform : InstanceClass<Platform>
    {
        public static bool IsInit { get; private set; }

        public AggregationAdSdk AdSdk { get; protected set; }
        public AttributionAdapterBase AttributeSdk { get; protected set; }
        public AnalysisSdkBase AnalysisSdk { get; protected set; }

        public abstract E_PlatformType Type { get; }
        protected PlayerPrefsSystem saveSystem;
        public PlayerPrefsSystem SaveSystem => saveSystem;
        public virtual string version => Application.version;

        public static void InitPlatform()
        {
            if (IsInit)
            {
                return;
            }

            IsInit = true;

            var cfg = ChannelConfig.Instance;
            var platformType = cfg.SelectedPlatform;
            LogLogger.LogInfo(LogTag.Frame, $"Init Platform type: {platformType}");
            switch (E_PlatformType.Oversea)
            {
                case E_PlatformType.Editor:
                    new DebugPlatform(cfg);
                    break;
                case E_PlatformType.Oversea:
                    new OverseaPlatform(cfg);
                    break;
            }
            LogLogger.LogInfo(LogTag.Frame, $"Init Platform done: {platformType}");
        }

        protected Platform(ChannelConfig channelConfig)
        {
            saveSystem = new UnityPlayerPrefs();
        }
    }
}
