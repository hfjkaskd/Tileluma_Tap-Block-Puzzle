using Bizza;
using Cysharp.Threading.Tasks;

namespace Bizza.Loading
{
    /// <summary>
    /// 加载表格任务
    /// </summary>
    public class LoadTablesTask : LoadingTaskBase
    {
        public override LoadingTaskName TaskName => LoadingTaskName.LoadTables;
        public override float Weight => 0.3f;
        public override LoadingTaskName[] Dependencies => new[] { LoadingTaskName.LoadRemoteConfig }; // 依赖远程配置
        
        public override async UniTask Execute()
        {
            LogLogger.LogInfo($"{TaskName} start");
            SetProgress(0.9f);
            
            await TableUtils.LoadTableConfigs();

            
            BizzaEventSystem.Emit(EventDefine.Frame.DataTableLoaded);
            SetProgress(1f);
            
            LogLogger.LogInfo($"{TaskName} end");
        }
    }
}

