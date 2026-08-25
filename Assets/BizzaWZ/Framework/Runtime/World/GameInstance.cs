#define PUBLISH_FOR_DEBUG
using System.Collections.Generic;
using Bizza.Sdk;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

public class GameInstance : BaseGameInstance
{
    // public GMTool gmPrefab;

    private void Awake()
    {
        UniTaskScheduler.UnobservedTaskException += (e) =>
        {
            // 全局捕获未被处理的异常
            Debug.LogError($"未捕获异常: {e}");
        };
        base.Awake();
        // QualitySettings.vSyncCount = 0; 
        Application.targetFrameRate = 60;
        DontDestroyOnLoad(gameObject);
        Input.multiTouchEnabled = false;
    }

    protected override void InitGameModule() 
    {
        GraphicsSettings.lightsUseLinearIntensity = true;
        GraphicsSettings.lightsUseColorTemperature = true;

        var attachedModules = GetComponentsInChildren<BaseGameModule>();
        foreach (var v in attachedModules)
        {
            InternalAddGameModule(v);
        }
        // Module 加载可能有依赖关系 注意保持顺序
        //
        InternalAddGameModule<SaveDataModule>();
        InternalAddGameModule<GameSystemModule>();
        InternalAddGameModule<PrefabPoolModule>();
        // InternalAddGameModule<TeachModule>();
        // InternalAddGameModule<TimeModule>();
        // // ↑↑↑ Add Game Module Here If Need ↑↑↑

        var gamePlayModule = InternalAddGameModule<GamePlayModule>();
        gamePlayModule.GameStart();
    }
}
