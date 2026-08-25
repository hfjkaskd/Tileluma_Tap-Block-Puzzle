using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;


public class RemoteConfigModule : BaseGameModule<RemoteConfigModule>
{
    // ===== 对外状态 =====
    public static string curVersion => Application.version;
    public static string reviewVersion;
    public static int dataValue = 1; // 1 为 A 面, 2 为 B 面
    public static bool reviewMode = true;
    public static bool isTestMode = false;

    private const float InitTimeoutSeconds = 5f;
    private const float SignInTimeoutSeconds = 5f;
    private const int DefaultDataValue = 1;
    private const bool DefaultReviewMode = true;
    private const int RandomDataValueMarker = -1;
    private const string DataValueKey = "Data_value";
    private const string ReviewVersionKey = "reviewVersion";
    private const string DataValueRemoteKey = "Data_Value";
    private const string TestModeRemoteKey = "is_TestMode";

    // 防并发匿名登录
    private static Task _signInTask;

    // Remote Config attributes
    public struct userAttributes { public bool expansionFlag; }
    public struct appAttributes { public int level; public int score; public string appVersion; }

    private bool _loadingRemoteConfig;
    private bool _finishedOnce;

    // 渐进超时（秒）
    private static readonly float[] FetchTimeouts = { 5f, 8f, 10f };

    private static async Task<bool> TryRunWithTimeout(Task task, float seconds)
    {
        if (await Task.WhenAny(task, Task.Delay(TimeSpan.FromSeconds(seconds))) != task)
            return false;

        await task;
        return true;
    }

    public async UniTask LoadRemoteConfig()
    {
        if (_loadingRemoteConfig)
        {
            Debug.LogWarning("[RC] LoadRemoteConfig 正在执行，忽略重复调用");
            return;
        }

        _loadingRemoteConfig = true;
        _finishedOnce = false;
        Debug.Log($"[RC] 开始 remote config 请求，version={curVersion}");

        try
        {
            await TryInitializeUnityServices();
            await TrySignInAnonymous();

            for (int attempt = 0; attempt < FetchTimeouts.Length; attempt++)
            {
                int index = attempt + 1;
                float timeout = FetchTimeouts[attempt];
                Debug.Log($"[RC] 开始第 {index} 次拉取（超时 {timeout}s）");

                // if (await TryFetchConfig(index, timeout))
                //     return;

                if (attempt < FetchTimeouts.Length - 1)
                {
                    Debug.Log($"[RC] 准备重试，将进行第 {index + 1} 次拉取");
                    await Task.Delay(500);
                }
            }

            ApplyFallbackConfig();
            Debug.LogError("[RC] 三次拉取均失败，使用默认配置");
        }
        finally
        {
            OnRemoteConfigLoadFinish();
        }
    }

    private async UniTask TryInitializeUnityServices()
    {
        // if (UnityServices.State == ServicesInitializationState.Initialized)
        //     return;

        // Debug.Log("[RC] 开始初始化 UGS");
        // try
        // {
        //     var initTask = UnityServices.InitializeAsync();
        //     if (!await TryRunWithTimeout(initTask, InitTimeoutSeconds))
        //         throw new TimeoutException($"UGS 初始化超时 {InitTimeoutSeconds}s");

        //     Debug.Log("[RC] UGS 初始化成功");
        // }
        // catch (Exception e)
        // {
        //     Debug.LogWarning($"[RC] UGS 初始化失败：{e.Message}（继续尝试拉取配置）");
        // }
    }

    private async UniTask TrySignInAnonymous()
    {
        // if (AuthenticationService.Instance.IsSignedIn)
        // {
        //     Debug.Log("[RC] 已登录，跳过匿名登录");
        //     return;
        // }

        // Debug.Log("[RC] 尝试匿名登录");
        // try
        // {
        //     if (_signInTask == null || _signInTask.IsCompleted)
        //     {
        //         _signInTask = AuthenticationService.Instance.SignInAnonymouslyAsync(
        //             new SignInOptions { CreateAccount = true }
        //         );
        //     }

        //     if (!await TryRunWithTimeout(_signInTask, SignInTimeoutSeconds))
        //         throw new TimeoutException($"匿名登录超时 {SignInTimeoutSeconds}s");

        //     Debug.Log($"[RC] 匿名登录成功，PlayerId={AuthenticationService.Instance.PlayerId}");
        // }
        // catch (Exception e)
        // {
        //     Debug.LogWarning($"[RC] 匿名登录失败：{e.Message}（继续尝试拉取配置）");
        // }
    }

    // private async UniTask<bool> TryFetchConfig(int index, float timeout)
    // {
//         try
//         {
//             var ua = new userAttributes();
//             var aa = new appAttributes { appVersion = curVersion };
//             var fetchTask = RemoteConfigService.Instance.FetchConfigsAsync(ua, aa);

//             if (!await TryRunWithTimeout(fetchTask, timeout))
//             {
//                 Debug.LogWarning($"[RC] 第 {index} 次拉取超时（>{timeout}s）");
//                 return false;
//             }

//             reviewVersion = RemoteConfigService.Instance.appConfig.GetString(ReviewVersionKey, curVersion);
//             reviewMode = (reviewVersion == curVersion);
// #if UNITY_EDITOR
//             if (UnityEditor.EditorPrefs.HasKey("Editor_ReviewMode"))
//             {
//                 reviewMode = UnityEditor.EditorPrefs.GetBool("Editor_ReviewMode");
//             }
// #endif

//             dataValue = RemoteConfigService.Instance.appConfig.GetInt(DataValueRemoteKey, DefaultDataValue);
//             if (dataValue == RandomDataValueMarker)
//             {
//                 dataValue = UnityEngine.Random.Range(1, 3);
//             }
//             else if (dataValue != 1 && dataValue != 2)
//             {
//                 dataValue = DefaultDataValue;
//             }

//             isTestMode = RemoteConfigService.Instance.appConfig.GetBool(TestModeRemoteKey, false);
//             SetDataValue();

//             Debug.Log($"[RC] 第 {index} 次拉取成功，reviewVersion='{reviewVersion}', cur='{curVersion}', reviewMode={reviewMode}, dataValue={dataValue}, isTestMode={isTestMode}");
//             return true;
//         }
//         catch (Exception e)
//         {
//             Debug.LogWarning($"[RC] 第 {index} 次拉取异常：{e.Message}");
//             return false;
//         }
    // }

    private void ApplyFallbackConfig()
    {
        reviewVersion = curVersion;
        reviewMode = DefaultReviewMode;
#if UNITY_EDITOR
        if (UnityEditor.EditorPrefs.HasKey("Editor_ReviewMode"))
        {
            reviewMode = UnityEditor.EditorPrefs.GetBool("Editor_ReviewMode");
        }
#endif
        dataValue = DefaultDataValue;
        isTestMode = false;
        SetDataValue();
    }

    private void OnRemoteConfigLoadFinish()
    {
        if (_finishedOnce) return;
        _finishedOnce = true;

        _loadingRemoteConfig = false;
        Debug.Log($"[RC] 流程结束，当前 reviewMode={reviewMode}, dataValue={dataValue}, isTestMode={isTestMode}");
    }

    private void SetDataValue()
    {
        if (!PlayerPrefs.HasKey(DataValueKey))
        {
            PlayerPrefs.SetInt(DataValueKey, dataValue);
            PlayerPrefs.Save();
        }
    }
}
