using System;
using System.Collections.Generic;
using cfg;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

[Preserve]
[GraphElementInfo(Category = "教学", Text = "教学是否完成", SupportTypes = new Type[] { typeof(ActionGraphBase) })]
[Obfuz.ObfuzIgnore]
public class Variable_Bool_TeachIsCompleted : Variable_Bool
{
    [GraphVariable(LabelText = "教学Key", TipsText = "默认为当前图标对应的上一个教学", Required = false)]
    public StringWrapper teachKey = new StringWrapper();

    public override string DebugName => "教学是否完成";


    public override object Clone()
    {
        return new Variable_Bool_TeachIsCompleted
        {
            teachKey = teachKey.Clone() as StringWrapper
        };
    }

    public static bool PrevIsCompleted(string key)
    {
        var list = Tables.Instance.TblTeach.DataList;
        string prevKey = string.Empty;
        for (int i = 0, l = list.Count; i < l; i++)
        {
            var teach = list[i];
            if (teach.Id == key)
            {
                if(string.IsNullOrEmpty(prevKey))
                {
                    return true;
                }
                var state = SaveDataUtils.TeachData.GetState(prevKey);
                return state >= 100;
            }
            prevKey = teach.Id;
        }

        return false;
    }

    public override bool GetValue(in ExecuteArgs executeArgs)
    {
        var key = teachKey.GetValue(executeArgs);
        return !string.IsNullOrEmpty(key)
            ? SaveDataUtils.TeachData.IsCompleted(key)
            : PrevIsCompleted(executeArgs.graph.graphName);
    }
}