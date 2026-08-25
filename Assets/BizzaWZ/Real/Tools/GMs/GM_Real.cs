#if BIZZA_REAL_WITHDRAW
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Bizza.Sdk;
using UnityEngine;

public partial class SROptions
{
    private const string RealBase = "真网赚 - 基础";

    private const string RealPanel = "真网赚 - 页面";

    private const string RealAdHanel = "真网赚 - 广告";

    private const string RealGame = "真网赚 - 游戏功能";

    [Category(RealGame)]
    [DisplayName("结束自定义游戏引导")]
    public void OverGameToturial()
    {
        BridgingUtil.NewPlayerGuideEnd();
    }


    [Category(RealBase)]
    [DisplayName("查看UID")]
    public void CheckUID()
    {
        string uid = PlayerPrefs.GetString(AccountModule.m_userIdKey);
        string googleId = PlayerPrefs.GetString(AccountModule.m_googleIdKey);
        string userInfo = AccountModule.Instance.Os_Current_Uso.GetInfo();
        LogLogger.LogError("UID ：：： " + uid);
        LogLogger.LogError("GOOgleId ：：： " + googleId);
        LogLogger.LogError("UserInfo ：：： " + userInfo);
    }

    [Category(RealBase)]
    [DisplayName("复制UID")]
    public void CopyUID()
    {
        string uid = PlayerPrefs.GetString(AccountModule.m_userIdKey);
        GUIUtility.systemCopyBuffer = uid;
        Debug.Log("已复制: " + uid);
    }

    [Category(RealBase)]
    [DisplayName("编辑器自动每日提现")]
    public void TestDailyMission()
    {
        var panel = UIModule.Instance.GetPage<DailyMissionPanel>();
        if (panel != null)
        {
            panel.EditorTestDailyMission();
        }
    }

    [Category(RealBase)]
    [DisplayName("测试推送通知")]
    public void TestNotification()
    {
        DailyPushManager.Instance.TestConfiguredPushContents();
    }

    [Category(RealBase)]
    [DisplayName("性能级别")]
    public AndroidGraphicsOptimizer.AndroidGraphicsTier tier { get; set; }

    [Category(RealBase)]
    [DisplayName("设置性能")]
    public void SetUrpSetting()
    {
        var v = GameObject.FindObjectOfType<AndroidGraphicsOptimizer>();
        v.SetUrpSettingByTier(tier);
    }

    [Category(RealPanel)]
    [DisplayName("打开恭喜获得")]
    public void OpenGetRewardPanel()
    {
        Real_GetRewardPanelUtil.OpenGetRewardPanel(DoubleGetRewardPanel.E_UseScene.Test);
    }

    [Category(RealPanel)]
    [DisplayName("打开成功恭喜获得")]
    public void OpenGetRewardPanel2()
    {
        Real_GetRewardPanelUtil.OpenGetRewardPanel(DoubleGetRewardPanel.E_UseScene.WinPanel);
    }

     [Category(RealPanel)]
    [DisplayName("看一个广告")]    
    public void LookOne()
    {
         SaveDataUtils.GameData.userTodayLookAdCount++;
    }

    
    [Category(RealPanel)]
    [DisplayName("打开提现比例界面")]
    public void OpenExchangeRatePanel()
    {
        double beforeBlance = AccountModule.Instance.Os_Current_Uso.GetBalance();
        double beforeClash = AccountModule.Instance.Os_Current_Uso.Os_Ewl;
        double nowBlance = AccountModule.Instance.Os_Current_Uso.GetBalance();
        double nowClash = AccountModule.Instance.Os_Current_Uso.Os_Ewl + 25f;

        ExchangeRateInfo exchangeRateInfo = new ExchangeRateInfo()
        {
            beforeBlance = beforeBlance,
            beforeClash = beforeClash,
            nowBlance = nowBlance,
            nowClash = nowClash,
            beforeRate = 0.25f,
            nowRate = 0.45f,
        };

        UIModule.Instance.OpenPage(UIPageIds.ExchangeRatePanel, exchangeRateInfo);
    }

    [Category(RealPanel)]
    [DisplayName("打开每日提现界面")]
    public void OpenDailyWithdrawPanel()
    {
        UIModule.Instance.OpenPage(UIPageIds.DailyWithdrawPanel);
    }

    [Category(RealAdHanel)]
    [DisplayName("测试插屏广告")]
    public void OpenInteriAd()
    {
        var money = new ItemEntry() { Type = E_ItemType.Dollar, Count = 100f };
        BizzaSdk.Ad.ShowInterAd(E_AdPos.LocalTest.ToString(), money.Count, (success) =>
        {
            LogLogger.LogAdInfo("插屏广告成功");
        }
        );
    }

    [Category(RealAdHanel)]
    [DisplayName("测试激励广告")]
    public void OpenRewardAd()
    {
        var money = new ItemEntry() { Type = E_ItemType.Dollar, Count = 100f };
        BizzaSdk.Ad.ShowRewardAd(E_AdPos.LocalTest.ToString(), money.Count, (success) =>
        {
            LogLogger.LogAdInfo("激励广告成功");
        }
        );
    }

     [Category(RealAdHanel)]
    [DisplayName("切换插屏准备状态")]    
    public void InterestAdReay()
    {
        ChannelConfig.Instance.real_CustomConfig.interAdNotReady = !ChannelConfig.Instance.real_CustomConfig.interAdNotReady;
        LogLogger.LogADPR($"插屏广告准备状态:{ChannelConfig.Instance.real_CustomConfig.interAdNotReady}");

    }

     [Category(RealAdHanel)]
    [DisplayName("切换激励准备状态")]    
    public void RewardAdReay()
    {
        ChannelConfig.Instance.real_CustomConfig.rewardAdNotReady = !ChannelConfig.Instance.real_CustomConfig.rewardAdNotReady;
        LogLogger.LogADPR($"激励广告准备状态:{ChannelConfig.Instance.real_CustomConfig.rewardAdNotReady}");
    }


    [Category(RealAdHanel)]
    [DisplayName("加Dollar")]
    public void OpenDollar()
    {
        var money = new ItemEntry() { Type = E_ItemType.Dollar, Count = 100f };
        money.Type = E_ItemType.Dollar;
        money = ItemUtils.ToCorrect(money);
        ItemUtils.AddItem(money, new AddItemParam
        {
            playAnim = true,
            isAd = false,
            startPos = BroadcastBarController.Instance.entryAObj.transform.position,
            bUiPos = false,
            source = DoubleGetRewardPanel.GetItemSource(DoubleGetRewardPanel.E_UseScene.Test),
        });
        UIUtils.ShowTips(money, default);
    }

    [Category(RealGame)]
    [DisplayName("解锁关卡数")]
    public int unlockLevel
    {
        get => _unlockLevel;
        set => _unlockLevel = Math.Max(1, value);
    }

    private int _unlockLevel = 1;

    [Category(RealGame)]
    [DisplayName("跳关")]
    public void GotoLevel()
    {
        SaveDataUtils.GameData.playerSelectedLv = _unlockLevel;
        //GameManager.Instance.Init(SaveDataUtils.GameData.playerUnlockedLv - 1);
    }

    [Category(RealGame)]
    [DisplayName("老虎机启动")]
    public void SlotEnter()
    {
        SaveDataUtils.GameData.playerPassLv += 5;
        
    }

    [Category(RealGame)]
    [DisplayName("加道具")]
    public void AddProp()
    {
        SaveDataUtils.ItemData.itemMap[E_ItemType.GameProp_Back] = new ItemEntry()
        {
            Type = E_ItemType.GameProp_Back,
            Count = 100f,
        };
        SaveDataUtils.ItemData.itemMap[E_ItemType.GameProp_ClearArea] = new ItemEntry()
        {
            Type = E_ItemType.GameProp_ClearArea,
            Count = 100f,
        };
        SaveDataUtils.ItemData.itemMap[E_ItemType.GameProp_Refresh] = new ItemEntry()
        {
            Type = E_ItemType.GameProp_Refresh,
            Count = 100f,
        };
    }

    [Category(RealGame)]
    [DisplayName("胜利")]
    public void Win()
    {
        //GamePanel.I.ShowWinPanel();
    }

    [Category(RealGame)]
    [DisplayName("下一个盒子")]
    public void Next()
    {
        //SpriteGridSpawner.Instance.GetCreateSynthesisInfoBFS_GM();
        //Debug.LogError(" --- ");
    }

    [Category(RealGame)]
    [DisplayName("100次测试")]
    public void N100ext()
    {
        for (int i = 0; i < 100; i++)
        {
           // SpriteGridSpawner.Instance.GetCreateSynthesisInfoBFS_GM();
          //  Debug.LogError(" --- ");
        }
    }
}
#endif