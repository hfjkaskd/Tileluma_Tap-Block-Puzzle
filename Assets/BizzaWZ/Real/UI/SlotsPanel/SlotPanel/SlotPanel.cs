using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public partial class UIPageIds
{
    public static readonly PageId SlotPanel = "SlotPanel";
}

public class SlotPanel : UIPageBase
{
    public SlotMachineManager slotMachineManager;

    public TMP_Text slotHintTxt;
    public GameObject canClickObj;
    public GameObject notCanClickObj;

    public SlotRewardPanel slotRewardPanel;

    private float coinValue;

    private bool isSloting = false; public bool IsSloting => isSloting;

    public BizzaButton closeBtn;
    public BizzaButton faqBtn;
    public BizzaButton slotBtn;

    protected override void OnAwake()
    {
        base.OnAwake();

        closeBtn.onClick.AddListener(OnClosePanel);
        faqBtn.onClick.AddListener(OnClickFQA);
        slotBtn.onClick.AddListener(PlaySlotBtn);
    }

    protected override void OnClose()
    {
        SoundManager.Instance.PlayBGM("SFX_BGM");
    }

    protected override void OnOpen()
    {
        isSloting = false;
        Refresh();
        SoundManager.Instance.PlayBGM("SlotBgm");
    }

    private void Refresh()
    {
        coinValue = 0;
        int slotCount = SaveDataUtils.GameData.playerPassLv;
        bool isCanclick = slotCount >= 5;
        canClickObj.SetActive(isCanclick);
        notCanClickObj.SetActive(!isCanclick);
        slotRewardPanel.gameObject.SetActive(false);
        if (isCanclick)
        {
            slotHintTxt.text = LanguageUtils.GetText("SlotPanel_HaveSpin");
        }
        else
        {
            slotHintTxt.text = LanguageUtils.GetText("SlotPanel_AdSpin");
        }
    }

    public void OnClosePanel()
    {
        CloseSelf();
    }

    public void OnClickFQA()
    {
        UIModule.Instance.OpenPage(UIPageIds.SlotFAQPanel);
    }

    [Obfuz.ObfuzIgnore(Obfuz.ObfuzScope.MethodName)]
    public void PlaySlotBtn()
    {
        if (isSloting) return;
        isSloting = true;
        if (SaveDataUtils.GameData.playerPassLv >= 5)
        {
            OnPlaySlot(false);
            SaveDataUtils.GameData.playerPassLv = 0;
        }
        else
        {
            BizzaSdk.Ad.ShowRewardAd(
                E_AdPos.USSlot.ToString(),
                WithdrawalUtil.GetDollarCountByReward(),
                OnAdResult
            );
        }

        Refresh();
        BizzaEventSystem.Emit(EventDefine.CustomGameEvent.SlotProgressChanged);
    }

    [Button("Test Slot Btn")]
    public void TestSlotBtn()
    {
        BizzaSdk.Ad.ShowRewardAd(
                E_AdPos.USSlot.ToString(),
                WithdrawalUtil.GetDollarCountByReward(),
                OnAdResult
            );
    }

    private void OnAdResult(Bizza.Sdk.ShowAdResult param)
    {
        bool isSuccess = param.success;
        AccountModule.OceanShineAdRevenueResponse response = param.response;
        if (!isSuccess || response == null)
        {
            isSloting = false;
            return;
        }
        coinValue = (float)response.GetBalance();
//         LogUtil.Error("OnAdResult coinValue: " + coinValue);
        OnPlaySlot(true);
    }

    private void OnPlaySlot(bool isAd)
    {
        float _dollar = isAd ? WithdrawalUtil.GetDollarCountByReward() : WithdrawalUtil.GetDollarCountBtFree();
        SoundManager.Instance.PlaySFX("SlotSpin");
        slotMachineManager.PlayAnim(
            isAd,
            (string type) => 
            {
                slotRewardPanel.gameObject.SetActive(true);
                slotRewardPanel.Init(coinValue, _dollar, type);
                isSloting = false;
            }
        );
    }

}
