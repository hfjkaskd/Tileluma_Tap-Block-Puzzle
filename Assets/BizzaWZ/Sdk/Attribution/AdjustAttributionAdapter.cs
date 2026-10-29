#if BIZZA_ENABLE_ADJUST

using System;
using Bizza.GameAnalytics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdjustSdk;

namespace Bizza.Sdk
{
    public class AdjustAttributionAdapter : AttributionAdapterBase
    {
        public static string adJustAdidKey = "adJustAdidKey";
        public static string adJustGooglIdKey = "adJustGoogleIdKey";
        public static AdjustConfig adjustConfig;

        public override void Init(ChannelConfig channelConfig)
        {

        }

        /// <summary>
        /// 初始化归因
        /// </summary>
        public override void AttributeStart(string eventName)
        {
            if (adjustConfig != null)
            {
                return;
            }

#if DEBUG_MODE
            adjustConfig = new AdjustConfig(
                    ChannelConfig.Instance.adjustKey,
                    AdjustEnvironment.Sandbox,
                    allowSuppressLogLevel: true
                );
            LogLogger.LogPlatformInit($"Adjust 是沙盒模式 ： {AdjustEnvironment.Sandbox}");
#else
            adjustConfig = new AdjustConfig(
                    ChannelConfig.Instance.adjustKey,
                    AdjustEnvironment.Production,
                    allowSuppressLogLevel: true
                );
                LogLogger.LogPlatformInit($"Adjust 是正式模式 ： {AdjustEnvironment.Production}");
#endif

            // 设置开始日志等级
            // AdjustLogLevel.Verbose	启用完整日志
            // AdjustLogLevel.Suppress	禁止所有日志
            adjustConfig.LogLevel = AdjustLogLevel.Verbose;

            LogLogger.LogVerbose(LogTag.ADAttribute, "设备的安卓ID" + DeviceInfoUtil.deviceInfoDataForCloud.Os_Anid);
            // 外部设备ID
            adjustConfig.ExternalDeviceId = DeviceInfoUtil.deviceInfoDataForCloud.Os_Anid;

            // SDK在应用在后台运行时发送信息
            adjustConfig.IsSendingInBackgroundEnabled = false;

            // 在归因中接收广告支出数据
            adjustConfig.IsCostDataInAttributionEnabled = true;

            adjustConfig.AttributionChangedDelegate = (attribution) =>
            {
                AttributionChangedDelegate(attribution, eventName);
            };

            Adjust.InitSdk(adjustConfig);
            LogLogger.LogAttribute($"Adjust初始化");
            var adid = PlayerPrefs.GetString(adJustAdidKey, "");
            if (string.IsNullOrEmpty(adid))
            {
                Adjust.GetAdid(idfa =>
                {
                    PlayerPrefs.SetString(adJustAdidKey, idfa);
                    LogLogger.LogVerbose(LogTag.LOG_Attribute, $"GetAdid:{idfa}");
                });
            }

            Adjust.GetGoogleAdId(googleId =>
            {
                PlayerPrefs.SetString(adJustGooglIdKey, googleId);
                LogLogger.LogVerbose(LogTag.LOG_Attribute, $"GetGoogleAdId:{googleId}");
            });
        }

        public override void AttributeAdShow(object param)
        {

        }

        public static string networkKey = "network";
        public static string campaignKey = "campaign";
        public static string adgroupkKey = "adgroup";

        public static string AD_REVENUE_APPLOVIN_MAX = "applovin_max_sdk";

        public static Dictionary<string, object> oceanShineUserAttrs = new Dictionary<string, object>();

        public static void AttributionChangedDelegate(AdjustAttribution attribution, string eventName)
        {
            var network = attribution?.Network ?? "";
            var campaign = attribution?.Campaign ?? "";
            var adgroup = attribution?.Adgroup ?? "";
            BizzaGameAnalytics.SetAttribution(network, campaign);

            LogLogger.LogAttribute($"Adjust数据发生变化 ::: networkKey {network} campaignKey {campaign} adgroupkKey {adgroup}");
            PlayerPrefs.SetString(networkKey, network);
            PlayerPrefs.SetString(campaignKey, campaign);
            PlayerPrefs.SetString(adgroupkKey, adgroup);

            var _oceanShineUserAttrs = new Dictionary<string, object>
            {
                { "PbSadgp", attribution.Adgroup },
                // 设备当前归因广告组的名称

                { "PbSaid", PlayerPrefs.GetString(adJustAdidKey, "") },
                // 设备的唯一 Adjust ID

                { "PbSapid", DeviceInfoUtil.GetDeviceInfoDataForCloud().Os_Apid },
                // 应用 ID

                { "PbScat", attribution.CostAmount ?? 0 },
                // 安装成本 - costAmount

                { "PbSccy", attribution.CostCurrency },
                // 成本相关的货币代码 - costCurrency

                { "PbSckl", "" },
                // 安装被标记的点击标签 - clickLabel

                { "PbScam", attribution.Campaign },
                // 设备当前归因 - campaign

                { "PbSctv", attribution.Creative },
                // 设备当前归因素材的名称 - creative

                { "PbSctm", DateTimeOffset.UtcNow.ToUnixTimeSeconds() },
                // 客户端收到回调的时间 - clientTime

                { "PbScty", attribution.CostType },
                // 推广活动定价模型 - costType

                { "PbSdbu", "" },
                // 投放参数 - deepLinkUrl

                { "PbSevt", eventName },
                // 触发归因的事件 - event

                { "PbSfbr", attribution.FbInstallReferrer },
                // 回调原始值 - fbInstallReferrer

                { "PbSgbt", "" },
                // Google 点击时间 - ggClickTime

                { "PbSnbt", attribution.Network },
                // 归因渠道名称 - network

                { "PbStra", attribution.TrackerName },
                // 归因跟踪码 - trackerName

                { "PbStrt", attribution.TrackerToken },
                // 归因跟踪名称 - trackerToken

                { "PbSusid", PlayerPrefs.GetString(AccountModule.m_userIdKey) },
                // 用户 ID

                { "vn", Application.version }
                // 应用版本号
            };

            oceanShineUserAttrs = _oceanShineUserAttrs;
            AccountModule.Instance.Request_UserAttrsRequest(_oceanShineUserAttrs);
        }

        private static bool _hasAttributionInfo;


        public static void ReportAdShowForAdjust(double revenue, string currency, string AdRevenueNetwork,
            string AdRevenueUnit, string AdRevenuePlacement)
        {
            AdjustAdRevenue adjustAdRevenue = new AdjustAdRevenue(AD_REVENUE_APPLOVIN_MAX);
            adjustAdRevenue.SetRevenue(revenue, currency);
            adjustAdRevenue.AdRevenueNetwork = AdRevenueNetwork;
            adjustAdRevenue.AdRevenueUnit = AdRevenueUnit;
            adjustAdRevenue.AdRevenuePlacement = AdRevenuePlacement;

            LogLogger.LogVerbose(LogTag.LOG_Attribute,
                $"广告归因 ：revenue - {revenue} currency - {currency} AdRevenueNetwork - {AdRevenueNetwork} AdRevenueUnit - {AdRevenueUnit} AdRevenuePlacement - {AdRevenuePlacement} ");
            Adjust.TrackAdRevenue(adjustAdRevenue);
        }

    }
}
#endif
