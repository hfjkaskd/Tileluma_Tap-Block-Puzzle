using System.ComponentModel;
using Bizza.Sdk;
using BizzaSdk;
using UnityEngine;

public partial class SROptions
{
    private const string SdkCategory = "Sdk/广告";

    [Category(SdkCategory)]
    [DisplayName("播放激励视频")]
    public void Sdk_ShowRewardAd()
    {
        Ad.ShowRewardAd(new ShowAdArgs());
    }

    [Category(SdkCategory)]
    [DisplayName("播放插屏广告")]
    public void Sdk_ShowInterAd()
    {
        Ad.ShowInterAd(new ShowAdArgs());
    }

    [Category(SdkCategory)]
    [DisplayName("激励是否就绪")]
    public bool Sdk_IsRewardReady => Ad.IsRewardReady;

    [Category(SdkCategory)]
    [DisplayName("插屏是否就绪")]
    public bool Sdk_IsInterReady => Ad.IsInterReady;

    [Category(SdkCategory)]
    [DisplayName("显示Banner")]
    public void Sdk_ShowBanner()
    {
        Ad.ShowBanner();
    }

    [Category(SdkCategory)]
    [DisplayName("隐藏Banner")]
    public void Sdk_HideBanner()
    {
        Ad.HideBanner();
    }

    [Category(SdkCategory)]
    [DisplayName("开屏广告")]
    public void Sdk_ShowSplashAd()
    {
        Ad.ShowSplashAd();
    }

    [Category(SdkCategory)]
    [DisplayName("Native广告-显示")]
    public void Sdk_ShowNative()
    {
        Ad.ShowNative();
    }

    [Category(SdkCategory)]
    [DisplayName("Native广告-隐藏")]
    public void Sdk_HideNative()
    {
        Ad.HideNative();
    }

    [Category(SdkCategory)]
    [DisplayName("输出广告SDK信息")]
    public void Sdk_LogAdSdkInfo()
    {
        string info = Ad.GetAdSdkDebugInfo();
        Debug.Log(info);
    }

    private const string AnalysisCategory = "Sdk/打点";

    [Category(AnalysisCategory)]
    [DisplayName("打点是否已初始化")]
    public bool Sdk_IsAnalysisInited => Analysis.Inited;

    [Category(AnalysisCategory)]
    [DisplayName("发送测试事件(c_test_event)")]
    public void Sdk_SendTestEvent()
    {
        Analysis.AddParam("source", "gm_test");
        Analysis.SendCustomEvent("c_test_event");
        Debug.Log("打点已发送: c_test_event");
    }

    [Category(AnalysisCategory)]
    [DisplayName("发送自定义事件(带参数)")]
    public void Sdk_SendCustomEventWithParams()
    {
        Analysis.AddParam("level_id", 1);
        Analysis.AddParam("round_id", 1);
        Analysis.SendCustomEvent("c_begin_game");
        Debug.Log("打点已发送: c_begin_game");
    }

    [Category(AnalysisCategory)]
    [DisplayName("设置用户属性并上报")]
    public void Sdk_SetUserPropertyAndUpload()
    {
        Analysis.SetUserProperty("gm_test_key", "gm_test_value");
        Analysis.SetUserProperty("test_time", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        Analysis.UploadUserProperty();
        Debug.Log("用户属性已设置并上报");
    }
}
