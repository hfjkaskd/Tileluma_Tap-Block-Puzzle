#if BIZZA_REAL_WITHDRAW
using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using System.Linq;
using Spine;
using Spine.Unity;

public static partial class UIPageIds
{
    public static readonly PageId RealGamePanel = nameof(RealGamePanel);
}


[Obfuz.ObfuzIgnore]
public class RealGamePanel : UIPageBase
{
    private const int ComboVfxTrackIndex = 0;

    public UIPropEntry[] propEntries;
    public BizzaButton taskBtn;
    public TMP_Text levelTxt;
    public ItemForCountry itemForCountry;

    public SkeletonGraphic comboVFX;

    void Awake()
    {
        taskBtn.onClick.AddListener(() =>
        {
            _ = UIModule.Instance.OpenPage(UIPageIds.UI_DailyTaskPage);
        });

        if (propEntries.Length <= 0)
        {
            propEntries = GetComponentsInChildren<UIPropEntry>();
            LogLogger.LogInfo(LogTag.Log_Prop, "道具数量 ： " + propEntries.Length);
        }

        foreach (var propEntry in propEntries)
        {
            propEntry.Init(OnClickProp);
        }
    }

    protected override void OnOpen()
    {
        LogLogger.LogVerbose(BaseConst.LOG_Game, "SeabedGamePanel OnOpen");
        NumbericalStatistics.InitProp();
        RefreshLevelText();
        itemForCountry.OnRefresh();

        BizzaEventSystem.Emit(EventDefine.CustomGameEvent.SlotProgressChanged);
    }

    protected override void OnClose()
    {
    }


    private void OnClickProp(E_ItemType itemType)
    {
        var _propUseTimes = NumbericalStatistics._propUseTimes;
        if (_propUseTimes.TryGetValue(itemType, out var curTimes))
        {
            if (curTimes >= TableUtils.Global.PropUseMaxTimes)
            {
                UIUtils.ShowLanguageTips("Tips_PropReachMaxTimes");
                return;
            }
        }

        if (ItemUtils.GetItemCount(itemType) > 0)
        {
            if (PropUtils.UseProp(itemType))
            {
                if (ItemUtils.TryReduceItem(itemType))
                {
                    LogLogger.LogInfo($"使用了道具:{itemType}");

                    if (!_propUseTimes.ContainsKey(itemType))
                    {
                        _propUseTimes.Add(itemType, 0);
                    }

                    _propUseTimes[itemType]++;
                    StatisticModule.Instance.UseItem(itemType, SaveDataUtils.GameData.playerSelectedLv, _propUseTimes[itemType]);
                }
            }
        }
        else
        {
            LogLogger.LogInfo($"道具数量不足，无法使用:{itemType}");
            UIModule.Instance.OpenPage(UIPageIds.AddPropPanel, itemType).Forget();
        }
    }

    

    public void RefreshLevelText()
    {
        levelTxt.text = $"{SaveDataUtils.GameData.playerSelectedLv}"; //LanguageUtils.GetFormatText("Menu_LevelBtn", SaveDataUtils.GameData.playerUnlockedLv);
    }

    public void PlayComboVFX(int count)
    {
        if (comboVFX == null)
        {
            return;
        }

        comboVFX.enabled = true;
        string animationName = "Great!";

        if (count == 2)
        {
            animationName = "Great!";
        }
        else if (count == 3)
        {
            animationName = "Perfect!";
        }

        comboVFX.AnimationState.SetAnimation(0, animationName, false);
    }


}
#endif