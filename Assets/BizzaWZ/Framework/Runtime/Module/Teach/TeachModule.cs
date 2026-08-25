using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cfg;
using System;

#if UNITY_EDITOR
using UnityEditor;
public static class TeachEditorTool
{

    [MenuItem("Tools/教程开关/打开")]
    static void TeachMode_On()
    {
        EditorPrefs.SetBool("Editor_TeachMode", true);
    }

    [MenuItem("Tools/教程开关/关闭")]
    static void TeachMode_Off()
    {
        EditorPrefs.SetBool("Editor_TeachMode", false);
    }

    [MenuItem("Tools/教程开关/清除")]
    static void TeachMode_Clear()
    {
        EditorPrefs.DeleteKey("Editor_TeachMode");
    }
}
#endif

public class TeachModule : BaseGameModule<TeachModule>
{
    private readonly List<ActionGraph_Teach> GraphBases = new List<ActionGraph_Teach>();
    private readonly List<ActionGraph_Teach> RunningGraphBases = new();
    public int RunningGraphCount => RunningGraphBases.Count;

    public bool IsTeaching => isTeaching;
    private bool isTeaching = false;
    public bool teachEnable;

    public override void InitGameModule()
    {
        bool enable = true;
#if UNITY_EDITOR
        enable = UnityEditor.EditorPrefs.GetBool("Editor_TeachMode", false);
#endif
        teachEnable = enable;
        if (!enable)
        {
            return;
        }

        BizzaEventSystem.On(EventDefine.Frame.LoadingStageComplete, AddGraphBase);
    }

    private void AddGraphBase()
    {
        ActionGraph_Teach GraphBase;
        bool _ing = false;
        foreach (var teachConfig in Tables.Instance.TblTeach.DataList)
        {
            if (SaveDataUtils.TeachData.IsCompleted(teachConfig.Id))
            {
                LogLogger.LogInfo("teachConfig.Id" + teachConfig.Id);
                continue;
            }
            _ing = true;

            GraphBase = ActionGraphUtils.Get(teachConfig.Id, false) as ActionGraph_Teach;
            GraphBases.Add(GraphBase);
        }
        LogLogger.LogInfo("_ing.Id" + _ing);
        isTeaching = _ing;
        
    }

    void Update()
    {
        if (RemoteConfigModule.reviewMode)
        {
            return;
        }

        if (!isTeaching)
            return;

        for (int i = RunningGraphBases.Count - 1; i >= 0; i--)
        {
            if (!RunningGraphBases[i].Running)
            {
                RunningGraphBases.RemoveAt(i);
            }
        }

        for (int i = GraphBases.Count - 1; i >= 0; i--)
        {
            ActionGraph_Teach GraphBase = GraphBases[i];
            if (GraphBase?.conditionOverride != null)
            {
                var context = PoolUtil.GetClass<ActionContext_Teach>();
                var args = ActionGraphUtils.GetEventActionArgs(GraphBase, context);
                bool isRun = GraphBase.conditionOverride.GetValueWithDefault(args, false);
                if (isRun)
                {
                    ActionModule.Instance.Run(GraphBase, context);
                    GraphBases.Remove(GraphBase);
                    RunningGraphBases.Add(GraphBase);
                }
            }
        }
    }

    public override void ReleaseGameModule()
    {
        BizzaEventSystem.Off(EventDefine.Frame.LoadingStageComplete, AddGraphBase);
    }
}