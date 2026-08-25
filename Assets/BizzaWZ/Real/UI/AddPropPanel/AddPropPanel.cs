#if BIZZA_REAL_WITHDRAW
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class UIPageIds
{
    public static readonly PageId AddPropPanel = "AddPropPanel";
}
public class AddPropPanel : UIPageBase<E_ItemType>
{
    public BizzaButton adBuyBtn;
    public BizzaButton closeBtn;
    public Image propIcon;
    public TMP_Text propName;
    public TMP_Text limitTxt;
    public Sprite busAwaySortPropIcon;
    public Sprite busAwayShufflePropIcon;

    private E_ItemType _itemType;

    void Awake()
    {
        adBuyBtn.onClick.AddListener(() =>
        {
            BizzaSdk.Ad.ShowRewardAd(_itemType.ToString(), WithdrawalUtil.GetDollarCountByReward(), OnAdBuyFinish);
        });

        closeBtn.onClick.AddListener(() => { CloseSelf(); });
    }

    private void OnAdBuyFinish(Bizza.Sdk.ShowAdResult showAdResult)
    {
        bool success = showAdResult.success;
        if (success)
        {
            //NumbericalStatistics.AddPropTimesByEnum(_itemType);
            AddPropAndClose();
        }
    }

    private void AddPropAndClose()
    {
        ItemUtils.AddItem(_itemType, 1, true, true, propIcon.transform.position, false);
        CloseSelf();
    }

    protected override void OnOpen(E_ItemType itemType)
    {
        _itemType = itemType;
        var icon = ItemUtils.GetItemIcon(itemType);
        var config = TableUtils.Tables.TblItem.GetOrDefault(itemType);
        if (config != null)
        {
            propName.text = LanguageUtils.GetText(config.Name);
            propIcon.sprite = ItemUtils.GetItemIcon(itemType);

        }

         var _propUseTimes = NumbericalStatistics._propUseTimes;
        var maxTimes = TableUtils.Global.PropUseMaxTimes;
        var curTimes = _propUseTimes[itemType];
        limitTxt.text = LanguageUtils.GetFormatText("Limit_Tip", curTimes, maxTimes);
    }

    protected override void OnClose()
    {
    }
}
#endif