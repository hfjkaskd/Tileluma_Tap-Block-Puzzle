using System;
using UnityEngine.Scripting;

[Preserve]
[GraphElementInfo(Category = "教学", Text = "通常条件", SupportTypes = new Type[] {typeof(ActionGraphBase)})]
[Obfuz.ObfuzIgnore]
public class Variable_Bool_NormalCheck : Variable_Bool
{
    public override string DebugName => "通常条件";


    public override object Clone()
    {
        return new Variable_Bool_NormalCheck();
    }

    public override bool GetValue(in ExecuteArgs executeArgs)
    {
        return TeachModule.Instance.RunningGraphCount == 0
           && Variable_Bool_TeachIsCompleted.PrevIsCompleted(executeArgs.graph.graphName);
    }
}