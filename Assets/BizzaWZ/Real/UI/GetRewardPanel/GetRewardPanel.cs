#if BIZZA_REAL_WITHDRAW
using System;
using System.Collections;
using System.Collections.Generic;
using Bizza.Sdk;
using Obfuz;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class UIPageIds
{
    public static readonly PageId GetRewardPanel = "GetRewardPanel";
}


public class GetRewardPanel : UIPageBase<ItemEntry, ItemEntry, DoubleGetRewardPanel.E_UseScene, Action<bool>>
{
    public BizzaButton claimBtn;
    public BizzaButton closeBtn;
    public ItemEntry itemA;
    public ItemEntry itemB;
    // private DoubleGetRewardPanel.E_UseScene doubleGetRewardPanel;
    public GameObject LevelObj;
    public TMP_Text itemATxt;
    public TMP_Text itemBTxt; // 另一个货币的
    public TMP_Text levelTxt;
    public Transform itemAPos;
    public Transform itemBPos;
    public GameObject levelTips;
    public TMP_Text noThanksText;
    public TMP_Text rewardText;

    private string iconName = AccountModule.CountryType switch
    {
        AccountModule.E_CountryType.BR => "3",
        AccountModule.E_CountryType.ID => "1",
        AccountModule.E_CountryType.US => "5",
        _ => "3"
    };

    public BonusRate bonusRate;
    public WithdrawProgress progress;

    public GameObject MaxDollarTip;

    private DoubleGetRewardPanel.E_UseScene _useScene;

    private bool isLookAd = false;
    private bool isNoCD;

    void Awake()
    {
        noThanksText.spriteAsset = commonSpriteAsset;

        claimBtn.onClick.AddListener(() => { LookAd(); });

        closeBtn.onClick.AddListener(() =>
        {
            LogLogger.LogInfo("胜利关闭广告 " + isLookAd);
            NumbericalStatistics.CheckCloseGetReward(E_AdPos.GetReward, dollarCount, null);
            if (_useScene == DoubleGetRewardPanel.E_UseScene.WinPanel)
            {
#if BIZZA_REAL_WITHDRAW && BIZZA_ENABLE_MAX
                TransparentBlock.AddBlock(this);
                GameUtils.DelayDo(() =>
                {
                    TransparentBlock.RemoveBlock(this);
                    var money = new ItemEntry();
                    money.Type = E_ItemType.Dollar;
                    money.Count = dollarCount;
                    money = ItemUtils.ToCorrect(money);
                    ItemUtils.AddItem(money, new AddItemParam
                    {
                        playAnim = true,
                        isAd = false,
                        startPos = BroadcastBarController.Instance.entryAObj.transform.position,
                        bUiPos = false,
                        source = DoubleGetRewardPanel.GetItemSource(DoubleGetRewardPanel.E_UseScene.CloseGetReward),
                    });
                    CustomWinClose();
                    CloseSelf();
                }, 0);
#else
                CustomWinClose();
#endif
            }
            else
            {
                LogLogger.LogInfo("正常关闭广告 " + isLookAd);
                CloseSelf();
            }
        });

        void LookAd()
        {
#if BIZZA_REAL_WITHDRAW && BIZZA_ENABLE_MAX
            BizzaSdk.Ad.ShowRewardAd(E_AdPos.AddCoin.ToString(), itemB.Count, (showAdResult) =>
             {
                 if (this == null || !this || gameObject == null) return;
                 bool success = showAdResult.success;
                 // 真正成功
                 callback?.Invoke(success);
                 isLookAd = success;
                 if (_useScene == DoubleGetRewardPanel.E_UseScene.WinPanel)
                 {
                     TransparentBlock.AddBlock(this);
                     GameUtils.DelayDo(() =>
                     {
                         if (this == null || !this || gameObject == null) return;
                         TransparentBlock.RemoveBlock(this);
                         CloseSelf();
                         CustomWinClose();
                     }, 0);

                 }
                 else
                 {
                     CloseSelf();
                 }
             });
#else
#endif
        }
    }

    protected override void OnClose()
    {
        if (_useScene == DoubleGetRewardPanel.E_UseScene.WinPanel)
        {
            callback?.Invoke(true);
        }
        callback = null;
    }

    private Action<bool> callback;

    protected override void OnOpen(ItemEntry a, ItemEntry b, DoubleGetRewardPanel.E_UseScene useScene,
        Action<bool> _callback)
    {
        LogLogger.LogVerbose(BaseConst.LOG_Game, $"打开了界面 GetRewardPanel");
        itemA = a;
        itemB = b;
        callback = _callback;
        _useScene = useScene;
        isLookAd = false;
        {
            curTime = 0;
            isRecover = false;
            claimBtn.interactable = false;
        }
        isNoCD = useScene == DoubleGetRewardPanel.E_UseScene.DailyTask;
        LevelObj.SetActive(false);
        OnRefresh();

    }

    private float timer = 0.5f;
    private float curTime = 0;
    private bool isRecover = false;
    [SerializeField] private TMP_SpriteAsset commonSpriteAsset;

    private void Update() // 这里是为了防止恭喜获得界面弹出， 玩家点击游戏物体不小心点击到按钮做的防误触
    {
        if (isRecover) return;

        curTime += Time.deltaTime;
        if (curTime >= timer)
        {
            claimBtn.interactable = true;
            isRecover = true;
        }
    }

    private float dollarCount = 0;
    /// <summary>
    /// 打开时的界面
    /// </summary>
    private void OnRefresh()
    {
        MaxDollarTip.gameObject.SetActive(true);
        dollarCount = itemB.Count;
        itemBTxt.text = ItemUtils.GetItemText(itemB); // 真网赚下会进行小数点修复

        // levelTxt.text = LanguageUtils.GetFormatText("Menu_LevelBtn", SaveDataUtils.GameData.playerSelectedLv);
        noThanksText.text = LanguageUtils.GetText("Btn_NoThanks");

        bool win = _useScene == DoubleGetRewardPanel.E_UseScene.WinPanel;
        if (win)
        {
#if BIZZA_REAL_WITHDRAW && BIZZA_ENABLE_MAX
            if (!ChannelConfig.Instance.real_CustomConfig.singleCurrencyMode)
            {
                itemB.Count = dollarCount * 2;
                rewardText.text = $"{rewardText.text}×2";
                SaveDataUtils.GameData.playerPassLv++;
                BizzaEventSystem.Emit(EventDefine.CustomGameEvent.SlotProgressChanged);
            }
            // SaveDataUtils.GameData.playerUnlockedLv++;
            itemBTxt.text = ItemUtils.GetItemText(itemB);
            BridgingUtil.GameWin();
            AccountModule.Instance.Request_UserReachLevelReportRequest(null);
            LevelObj.SetActive(true);
            levelTxt.text = LanguageUtils.GetFormatText("Menu_LevelBtn", SaveDataUtils.GameData.playerSelectedLv - 1);
            LogLogger.LogVerbose(BaseConst.LOG_Game, "上报 关卡");
            if (!Bizza.Sdk.ChannelConfig.Instance.real_CustomConfig.singleCurrencyMode)
            {
                string _clash = LanguageUtils.GetText("CurrencyToken") + WithdrawalUtil.GetCustomizedFloatByCountryType(dollarCount);
                noThanksText.text = $"{LanguageUtils.GetFormatText("GetRewardPage_NoThanks", iconName, _clash)}";
            }
#endif
            levelTips.SetObjActive(_useScene == DoubleGetRewardPanel.E_UseScene.WinPanel);
            SoundManager.Instance.PlaySFX("Win");
            SaveDataUtils.Save();
        }

#if BIZZA_REAL_WITHDRAW && BIZZA_ENABLE_MAX
        float value = AccountModule.Instance.GetMaxDrawithRatio(true);
        bonusRate.gameObject.SetActive(value > 0);
        bonusRate.Init($"{value}%", false, true);
#else
#endif
    }

    ///
    /// 自定义的游戏胜利
    private void CustomWinClose()
    {
        BridgingUtil.LoadNextLevel();
    }
}
#endif
