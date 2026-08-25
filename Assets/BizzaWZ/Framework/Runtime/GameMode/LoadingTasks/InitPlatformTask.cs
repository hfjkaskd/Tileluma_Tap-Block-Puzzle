using System;
using Bizza;
using Bizza.Sdk;
using Cysharp.Threading.Tasks;

namespace Bizza.Loading
{
    [Obfuz.ObfuzIgnore]
    /// <summary>
    /// 初始化Sdk平台
    /// </summary>
    public class InitPlatformTask : LoadingTaskBase
    {
        public override LoadingTaskName TaskName => LoadingTaskName.InitPlatform;
        public override float Weight => 0.3f;
        public override LoadingTaskName[] Dependencies => Array.Empty<LoadingTaskName>();

        public override async UniTask Execute()
        {
            LogLogger.LogInfo($"{TaskName} start");
            SetProgress(0.9f);

            Platform.InitPlatform();

            SetProgress(1f);

            LogLogger.LogInfo($"{TaskName} end");
        }
    }
}