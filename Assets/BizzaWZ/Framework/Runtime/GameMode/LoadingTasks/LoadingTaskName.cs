namespace Bizza.Loading
{
    /// <summary>
    /// 任务标识
    /// </summary>
      [Obfuz.ObfuzIgnore]
    public enum LoadingTaskName
    {
        None = 0,
        LoadRemoteConfig,
        LoadTables,
        LoadGameData,
        InitPlatform,
        ReadLanguage,
        LoadGameRes,
        ShowSplashAd,
        PreLoadAsset,
        
        LoadUserId,
        LoadUserInfo,

        CheckNetwork,


        TestWait3s,
    }
}

