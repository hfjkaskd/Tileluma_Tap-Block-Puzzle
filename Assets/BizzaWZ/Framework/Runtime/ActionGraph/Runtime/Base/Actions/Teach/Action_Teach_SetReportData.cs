using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Scripting;


 
[Preserve]
[GraphElementInfo(Category = "教学", Text = "发送教学报告", SupportTypes = new Type[] {typeof(ActionGraphBase)})]
[Obfuz.ObfuzIgnore]
public class Action_Teach_SetReportData : ActionNodeBase
{
    [GraphVariable(TipsText = "报告序号", Required = true)]
    public FloatWrapper teachIndex = new Variable_Float_Direct();

    public override object Clone()
    {
        var clone = new Action_Teach_SetReportData();
        clone.teachIndex = (FloatWrapper)teachIndex?.Clone();
        return clone;
    }

    protected override E_ExecuteState OnExecute(in ExecuteArgs executeArgs)
    {
        var indexValue = teachIndex.GetValueInt(executeArgs);
        //TODO: 发送教学报告  可以在这里自定义事件，当这个教学步骤完成可以发送实丰事件或者特殊处理。
        string textValue = "神的随波逐流发送教学报告" + indexValue;
        // BizzaEventSystem.Emit(EventDefine.Teach.SentTeachReport, indexValue);
        StatisticModule.Instance.TeachEvent(indexValue);
        LogLogger.LogInfo($"[{Time.frameCount}]<color=cyan>{textValue}</color>");
        return E_ExecuteState.Success;
    }
}