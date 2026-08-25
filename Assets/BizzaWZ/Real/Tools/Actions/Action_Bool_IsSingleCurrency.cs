#if BIZZA_REAL_WITHDRAW
using System;
using Bizza.Sdk;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting;


[Obfuz.ObfuzIgnore]
[Preserve]
[GraphElementInfo(Category = "CustomAction", Text = "是否纯真提现", SupportTypes = new Type[] { typeof(ActionGraphBase) })]
public class Action_Bool_IsSingleCurrency : Variable_Bool
{
    public override object Clone()
    {
        var clone = new Action_Bool_IsSingleCurrency();
        return clone;
    }

    public override bool GetValue(in ExecuteArgs executeArgs)
    {
        bool isSingleCurrency = ChannelConfig.Instance.real_CustomConfig.singleCurrencyMode; 
        return isSingleCurrency;
    }
}
#endif