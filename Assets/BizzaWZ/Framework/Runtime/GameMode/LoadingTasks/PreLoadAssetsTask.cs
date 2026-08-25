using System;
using Bizza;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Bizza.Loading
{
    public class PreLoadAssetsTask : LoadingTaskBase
    {
        public override LoadingTaskName TaskName => LoadingTaskName.PreLoadAsset;
        public override float Weight => 0.3f;
        public override LoadingTaskName[] Dependencies => Array.Empty<LoadingTaskName>(); // 依赖远程配置
        
        public override async UniTask Execute()
        {
            LogLogger.LogInfo($"加载任务：{TaskName} start");
            SetProgress(0.9f);
            
            AssetUtils.LoadAssetAsync<GameObject>("UIPanel/UI_TeachFinterMove").Forget();
            AssetUtils.LoadAssetAsync<GameObject>("UIPanel/UI_TeachMask").Forget();
            AssetUtils.LoadAssetAsync<GameObject>("UIPanel/UI_TeachTip").Forget();
            AssetUtils.LoadAssetAsync<GameObject>("UIPanel/NewbieGiftPage").Forget();
            AssetUtils.LoadAssetAsync<GameObject>("UIPanel/RealWithdrawPanel").Forget();
            AssetUtils.LoadAssetAsync<GameObject>("UIPanel/RealGamePanel").Forget();

            SetProgress(1f);
            
            LogLogger.LogInfo($"加载任务：{TaskName} end");
        }
    }
}
