using System;
using System.Collections.Generic;
using Bizza;
using Bizza.Loading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 游戏加载阶段
/// 加载数据表=>加载主场景
/// </summary>
public class GameMode_Loading : GameModeBase
{
    public override E_GameModeType GameModeType => E_GameModeType.Loading;

    #region Loading UI

    private float progress;
    [LabelText("加载速度")]
    [Range(0f, 1f)]
    public float progressSpeed = 0.2f;

    private LoadingProcedure _loadingProcedure;
    protected override void OnInit()
    {
        base.OnInit();
        TableUtils.LoadLanguageTable();
#if BIZZA_REAL_WITHDRAW
        DeviceInfoUtil.GetDeviceInfoDataForCloud();
#endif
        _loadingProcedure = new LoadingProcedure();

        _loadingProcedure.AddTask(new InitPlatformTask());
#if BIZZA_REAL_WITHDRAW
        _loadingProcedure.AddTask(new CheckNetworkTask());
        _loadingProcedure.AddTask(new LoadUserDataTask());
#endif

        // 添加默认加载任务
        _loadingProcedure.AddTask(new LoadGameResTask());
        _loadingProcedure.AddTask(new PreLoadAssetsTask());
        _loadingProcedure.AddTask(new LoadRemoteConfigTask());
        // _loadingProcedure.AddTask(new ReadLanguageTask());
        _loadingProcedure.AddTask(new LoadTablesTask());
        _loadingProcedure.AddTask(new LoadGameDataTask());

       //  _loadingProcedure.AddTask(new ShowSplashAdTask());

        // 外部可以在这里添加更多任务
        // _loadingProcedure.AddTask(new YourCustomTask());

        RunLoadingSafe().Forget();
        UIModule.Instance.OpenPage(UIPageIds.LoadingPanel);
    }

    protected override void OnRelease()
    {
        base.OnRelease();
        UIModule.Instance.ClosePage(UIPageIds.LoadingPanel);
    }


    private void OnLoadingFinish()
    {
        LogLogger.LogInfo("OnLoadingFinish");
        // var needShowTips = PlayerPrefs.GetInt("AlreadyShowWithdrawTips", 0) == 0 && !RemoteConfigModule.reviewMode;
#if BIZZA_REAL_WITHDRAW
        LogLogger.LOGUserInstall($" 当前关卡数值为 ： {SaveDataUtils.GameData.playerSelectedLv} + 数据对比 {AccountModule.Instance.Os_Current_Uso.Os_Rts} + SaveDataUtils.GameData.playerUnlockedLv {SaveDataUtils.GameData.playerSelectedLv}");
        SaveDataUtils.GameData.playerSelectedLv = Mathf.Max(AccountModule.Instance.Os_Current_Uso.Os_Rts, SaveDataUtils.GameData.playerSelectedLv);
        LogLogger.LOGUserInstall($" 实际关卡数值为 ： {SaveDataUtils.GameData.playerSelectedLv}");
#endif
        if (false)
        {

        }
        else
        {
            StartGame();
        }
    }

    private async UniTaskVoid RunLoadingSafe()
    {
        try
        {
            await _loadingProcedure.Run(OnLoadingFinish);
        }
        catch (Exception e)
        {
            LogLogger.LogError($"加载流程失败: {e}");
            var args = new CommonConfirmTipsPanel.Args
            {
                isLanguage = true,
                des = "HTTPNetworkProblem",
                onFinish = Application.Quit,
            };
            await UIModule.Instance.OpenPage(UIPageIds.CommonConfirmTipsPanel, args);
        }
    }

    public void StartGame()
    {
        //进入战斗测试场景
#if UNITY_EDITOR
        if (UnityEditor.EditorPrefs.GetBool("Editor_BattleTest"))
        {
            LogLogger.LogInfo("进入[战斗测试场景]");
            World.Current.ChangeGameMode(E_GameModeType.BattleTest);
            UnityEditor.EditorPrefs.SetBool("Editor_BattleTest", false);
            return;
        }
#endif
        LogLogger.LogInfo($"ReadyChangeScene {TeachModule.Instance.teachEnable} {SaveDataUtils.TeachData.GetState("Teach_01")}");
        TransitionBlock.ToGamePlay();

        BizzaEventSystem.Emit(EventDefine.Frame.LoadingStageComplete);

        SoundManager.Instance.MuteBGM(!SaveDataUtils.SettingData.enableMusic);
        GameUtils.DelayDo(() =>
        {
            SoundManager.Instance.PlayBGM("SFX_BGM");
        }, 1f);
    }

    private void Update()
    {
        progress += Time.deltaTime * progressSpeed;
        float min = 0;
        float max = _loadingProcedure != null ? _loadingProcedure.progress : 0;
        progress = Mathf.Clamp(progress, min, max);
        // if (loadingProgress != null) loadingProgress.fillAmount = progress;
        // if (loadingProgressText != null) loadingProgressText.text = progress.ToString("P0");
        BizzaEventSystem.Emit(EventDefine.Frame.LoadingProgress, progress);
    }

    #endregion
}
