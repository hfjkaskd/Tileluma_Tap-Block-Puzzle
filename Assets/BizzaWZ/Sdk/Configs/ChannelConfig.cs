using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Bizza.Sdk
{
        [CreateAssetMenu(fileName = "ChannelConfig", menuName = "BizzaGame/ChannelConfig")][Obfuz.ObfuzIgnore]
        public class ChannelConfig : ChannelConfigBase<ChannelConfig>
        {
                public string bundleId => Application.identifier;
                public static bool IsRelese_Mode
                {
                        get
                        {
                                #if DEBUG_MODE
                                return false;
                                #endif
                                return true;
                        }
                } 

                public E_PlatformType SelectedPlatform
                {
                        get
                        {
#if UNITY_EDITOR
                                return editorPlatformType;
#else
                return platformType; 
#endif
                        }
                }

                #region 基础配置
                [FoldoutGroup("基础配置")]
                [LabelText("平台")]
                public E_PlatformType platformType = E_PlatformType.Oversea;

                [FoldoutGroup("基础配置")]
                [LabelText("AppId/AppCode")]
                public string AppId = "";

                [FoldoutGroup("基础配置")]
                [LabelText("编辑器平台(编辑器模式使用该平台)")]
                public E_PlatformType editorPlatformType = E_PlatformType.Editor;

                [FoldoutGroup("基础配置")]
                [LabelText("默认语言(在读取到语言前使用)")]
                public string defaultLanguage = "en-US";

                [FoldoutGroup("基础配置")]
                [LabelText("GM开关")]
                public bool GM;

                

                [FoldoutGroup("基础配置")]
                [LabelText("debug模式，全量日志输出")]
                public bool debugMode;
                #endregion

                #region 真提http
#if BIZZA_REAL_WITHDRAW
                [Space(5)]
                [FoldoutGroup("HTTP配置")][InlineProperty]
                [ShowIf("_enableHttp")]
                public HttpDataConfig httpConfig = new();

                [Space(5)]
                [FoldoutGroup("用户配置")][InlineProperty]
                [ShowIf("_enableHttp")]
                public Real_CustomConfig real_CustomConfig = new();

                [Space(5)]
                [FoldoutGroup("页面测试")][InlineProperty]
                [ShowIf("_enableHttp")]
                public Real_PageTest real_PageTestConfig = new();

                [Space(5)][InlineProperty]
                [FoldoutGroup("广告设置")]
                [ShowIf("_enableHttp")]
                public Real_ADStatistics real_ADStatistics = new();
#endif

#if BIZZA_REAL_WITHDRAW
                private bool _enableHttp = true;
#else
        private bool _enableHttp = false;
#endif
                #endregion

                #region 归因
                [FoldoutGroup("归因sdk")]
                [LabelText("启用的sdk")]
                public E_AttributeSdkType attributeConfig = E_AttributeSdkType.Adjust;

                [FoldoutGroup("归因sdk")]
               // [ShowIf("_enableAdjust")]
                [LabelText("adjust key")]
                public string adjustKey = "";
#if BIZZA_ENABLE_ADJUST
                private bool _enableAdjust = true;
#else
        private bool _enableAdjust = false;
#endif

                [FoldoutGroup("归因sdk")]
                [ShowIf("_enableTenjin")]
                [LabelText("tenjin key")]
                public string tenjinKey = "";

#if BIZZA_ENABLE_TENJIN
        private bool _enableTenjin = true;
#else
                private bool _enableTenjin = false;
#endif
                #endregion

                #region 打点

                [FoldoutGroup("打点sdk")]
                [LabelText("启用的sdk")]
                public E_AnalysisSdkType analysisSdkType;

                [FoldoutGroup("打点sdk")]
                [LabelText("数数配置")]
                [ShowIf("_enableThinkingData")]
                public ThinkingDataConfig tkConfig = new();
#if BIZZA_ENABLE_SHUSHU
        private bool _enableThinkingData = true;
#else
                private bool _enableThinkingData = false;
#endif

                #endregion

                #region 广告

                [FoldoutGroup("广告配置")]
                [LabelText("广告源")]
                public List<AdConfig> sourceAds = new();
                [FoldoutGroup("广告配置")]
                [LabelText("使用插屏补充激励")]
                public bool useInterReplenishReward;
                [FoldoutGroup("广告配置")]
                [LabelText("使用激励补充插屏")]
                public bool useRewardReplenishInter;
                #endregion


                /// <summary>
                /// 获取广告参数配置
                /// </summary>
                /// <param name="type"></param>
                /// <returns></returns>
                public AdConfig GetAdsConfig(E_AdsSource type)
                {
                        foreach (var cfg in sourceAds)
                        {
                                if (cfg.AdsSource == type)
                                {
                                        return cfg;
                                }
                        }
                        LogLogger.LogError("没有这个广告源，注意配置信息");
                        return null;
                }
        }

        #region 广告配置
        [Serializable][Obfuz.ObfuzIgnore]
        
        public class AdConfig
        {
                // 为每个字段添加FoldoutGroup，确保广告ID配置项也能收缩
                [GUIColor(0.8f, 0.4f, 0.2f)] // 改变颜色，让每个广告源显示更加明显
                [Tooltip("广告源")]
                public E_AdsSource AdsSource;

                [FoldoutGroup("广告ID", Expanded = false)] // 折叠组
                [Tooltip("奖励广告ID")]
                public string rewardAdId = "";

                [FoldoutGroup("广告ID")]
                [Tooltip("插屏广告ID")]
                public string interAdId = "";

                [FoldoutGroup("广告ID")]
                [Tooltip("横幅广告ID")]
                public string bannerAdId = "";

                [FoldoutGroup("广告ID")]
                [Tooltip("开屏广告ID")]
                public string openAdId = "";

                // 构造函数初始化广告源
                public AdConfig(E_AdsSource adsSource)
                {
                        AdsSource = adsSource;
                }

                // 判断广告源是否被选择
                private bool IsMaxSelected => AdsSource == E_AdsSource.Max;
                private bool IsTopOnSelected => AdsSource == E_AdsSource.TopOn;
        }

        [Flags][Obfuz.ObfuzIgnore] // 广告源
        public enum E_AdsSource
        {
                All = 0,
                Max = 1 << 0,   // 1
                TopOn = 1 << 1, // 2
        }
        #endregion

        #region 归因配置
        
        [Flags][Obfuz.ObfuzIgnore] // 广告源
        public enum E_AttributeSdkType
        {
                None = 0,
                Adjust = 1 << 0,   // 1
                Tenjin = 1 << 1, // 2
        }

        
        [Flags][Obfuz.ObfuzIgnore]
        public enum E_AnalysisSdkType
        {
                None = 0,
                ThinkingData = 1 << 0,
                Adjust = 1 << 1,
        }

        #if BIZZA_REAL_WITHDRAW
        [Serializable][Obfuz.ObfuzIgnore]
        public struct Real_CustomConfig
        {
                [LabelText("是否开启单货币模式")]
                public bool singleCurrencyMode;
                [LabelText("国家"), SerializeField] private AccountModule.E_CountryType country;
                public string CountryName => country.ToString();

                [LabelText("是否开启自定义账号")]
                public bool useEditorUserId;

                [LabelText("账号id"), ShowIf("useEditorUserId")]
                public string editorUserId;

                [LabelText("开启ecpm1000 - 给发行时必须开启")]
                public bool testECPM1000;

                [LabelText("ecpm1000值"), ShowIf("testECPM1000")]
                public float TestECPMValue;

                [LabelText("是否开启测试设备")]
                public bool openTestDevice;

                [LabelText("测试设备"), ShowIf("openTestDevice")]
                public string[] testDeviceIds;

                [LabelText("生成用户的ID记录路径")] //  = "Assets/BizzaWZ/Real/Tools/GenereatedInfo"
                public string GenerateFakeAndroidId;

                [LabelText("强制进入新手引导")] 
                public bool enterNewbieGuide;

                [LabelText("是否打开失败广告")]
                public bool failOpenAd;

                [LabelText("版本日志")]
                public string versionLog;

                [LabelText("插屏没有准备好")]
                public bool interAdNotReady;

                [LabelText("激励没有准备好")]
                public bool rewardAdNotReady;
        }
#endif

        
        [Obfuz.ObfuzIgnore]
        public enum E_RealCountry
        {
                BR, US, ID, None,
        }

        [Serializable] [Obfuz.ObfuzIgnore]
        public struct Real_PageTest
        {
                [LabelText("测试体现比例提升页面")]
                public bool testExPanel;

                [LabelText("测试每日提现界面")] 
                public bool testDialyWithdrawalPanel;
        }

        [Serializable][Obfuz.ObfuzIgnore]
        public struct Real_ADStatistics
        {
                [LabelText("关闭恭喜获得界面出现插屏")]
                public int CloseGetRewardCount;

                [LabelText("合成多少次后出现恭喜获得界面")]
                public int ShowGetRewardCount;

                [LabelText("合成多少次后出现假钞")]
                public int ShowDollarCount;
        }
        #endregion
}