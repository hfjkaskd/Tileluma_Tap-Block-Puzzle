using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using SRDebugger.Internal;
using UnityEngine;

public partial class SROptions
{
    private const string ItemCategory = "道具";

    [Category(ItemCategory)]
    [DisplayName("资源类型")]
    public E_ItemType resType { get; set; } = E_ItemType.Dollar;

    [Category(ItemCategory)]
    [DisplayName("资源数量")]
    public int resNum { get; set; } = 100;

    [Category(ItemCategory)]
    [DisplayName("增加指定资源")]
    public void AddItem()
    {
        ItemUtils.AddItem(resType, resNum);
    }

    [Category("广告")]
    [DisplayName("模拟广告完成")]
    public void GM_FiniashAd()
    {
        // AdModule.OnVideoAdEnd();
        // StatisticManager.Instance.OnFinishVideo(true, "");
    }

}
