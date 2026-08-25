using Bizza;
using Cysharp.Threading.Tasks;

namespace Bizza.Loading
{
    [Obfuz.ObfuzIgnore]
    /// <summary>
    /// 加载游戏数据任务
    /// </summary>
    public class LoadGameDataTask : LoadingTaskBase
    {
        public override LoadingTaskName TaskName => LoadingTaskName.LoadGameData;
        public override float Weight => 0.3f;
        public override LoadingTaskName[] Dependencies => new LoadingTaskName[] { LoadingTaskName.LoadTables }; // 依赖表格加载
        
        public override async UniTask Execute()
        {
            LogLogger.LogInfo($"{TaskName} start");
            SetProgress(0.94f);
            
            //游戏数据在SaveDataModule模块加载，这里等待就好
            await UniTask.WaitUntil(() => SaveDataModule.Instance.LoadingFinish);
            
            //设置语言
            SetProgress(1f);
            
            LogLogger.LogInfo($"{TaskName} end");
        }
    }
}

