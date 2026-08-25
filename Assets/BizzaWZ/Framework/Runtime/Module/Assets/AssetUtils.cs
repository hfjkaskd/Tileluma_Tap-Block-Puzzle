using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public static class AssetUtils
{
    [Obfuz.ObfuzIgnore]
    public enum E_LoadAssetType
    {
        Resources,
        Addressable,
    }

    public static E_LoadAssetType LoadAssetType = E_LoadAssetType.Addressable;

    #region 通用接口
    public static void LoadAsset<T>(string resPath, Action<string, T> callback) where T : Object
    {
        if (LoadAssetType == E_LoadAssetType.Resources)
        {
            var res = LoadAssetFromResources<T>(resPath);
            callback?.Invoke(resPath, res);
        }
        else
        {
            var res = LoadAssetFromAddressableSync<T>(resPath);
            callback?.Invoke(resPath, res);
        }
    }

    public static T LoadAssetSync<T>(string resPath) where T : Object
    {
        if (LoadAssetType == E_LoadAssetType.Resources)
        {
            var res = LoadAssetFromResources<T>(resPath);
            return res;
        }
        else
        {
            var res = LoadAssetFromAddressableSync<T>(resPath);
            return res;
        }
    }

    public static async UniTask<T> LoadAssetAsync<T>(string resPath) where T : Object
    {
        if (LoadAssetType == E_LoadAssetType.Resources)
        {
            var res = LoadAssetFromResources<T>(resPath);
            var asset = await UniTask.FromResult(res);
            return asset;
        }
        else
        {
            // LoadAssetAsync();
            return await LoadAssetFromAddressable<T>(resPath);
        }
    }

    public static async UniTask<IList<T>> LoadAssetsAsync<T>(string resPath) where T : Object
    {
        if (LoadAssetType == E_LoadAssetType.Resources)
        {
            var res = LoadAssetsFromResources<T>(resPath);
            return await UniTask.FromResult(res);
        }
        else
        {
            // LoadAssetAsync();
            return await LoadAssetsFromAddressable<T>(resPath);
        }
    }

    #endregion

    #region 加载场景

    public static async void LoadSceneWithBlock(string resPath, Action onLoadCompleted = null,
        bool waitUnloadFinish = false)
    {
        await LoadSceneWithBlockAsync(
            resPath,
            onLoadCompleted == null
                ? null
                : new Func<UniTask>(() =>
                {
                    onLoadCompleted();
                    return UniTask.CompletedTask;
                }),
            waitUnloadFinish);
    }

    public static async UniTask LoadSceneWithBlockAsync(string resPath, Func<UniTask> onLoadCompletedAsync = null,
        bool waitUnloadFinish = false)
    {
        var blockReason = "loadScene:::" + resPath;
        TransparentBlock.AddBlock(blockReason);
        try
        {
            if (waitUnloadFinish)
            {
                await LoadScene("Empty");
            }

            await LoadScene(resPath);

            if (onLoadCompletedAsync != null)
            {
                await onLoadCompletedAsync();
            }
        }
        finally
        {
            TransparentBlock.RemoveBlock(blockReason);
        }
    }

    private static string _prevSceneName = "InitWZ";
    public static async UniTask LoadScene(string resPath, LoadSceneMode loadSceneMode = LoadSceneMode.Additive,
        Action<string, SceneInstance> onLoaded = null)
    {
        await SceneManager.LoadSceneAsync(resPath, loadSceneMode);
        await UnloadCurScene();
        _prevSceneName = resPath;
    }

    public static async UniTask UnloadCurScene()
    {
        if (!string.IsNullOrEmpty(_prevSceneName))
        {
            var scene = SceneManager.GetSceneByName(_prevSceneName);
            if (!scene.IsValid() || !scene.isLoaded)
            {
                LogLogger.LogVerbose(LogTag.LOG_Asset, $"unload scene error:{_prevSceneName}");
                return;
            }
            var handle = SceneManager.UnloadSceneAsync(scene);
            if (handle != null)
            {
                await handle;
            }
        }
        _prevSceneName = "";
    }
    #endregion

    private static T LoadAssetFromResources<T>(string resPath) where T : Object
    {
        var res = Resources.Load<T>(resPath);
        if (res == null)
        {
            LogLogger.LogVerbose(LogTag.LOG_Asset, $"res load failed:{resPath}");
        }

        return res;
    }

    private static IList<T> LoadAssetsFromResources<T>(string resPath) where T : Object
    {
        var res = Resources.LoadAll<T>(resPath);
        if (res == null)
        {
            LogLogger.LogVerbose(LogTag.LOG_Asset, $"res load failed:{resPath}");
        }

        return res;
    }

    private static T LoadAssetFromAddressableSync<T>(string resPath) where T : Object
    {
        try
        {
            var handle = Addressables.LoadAssetAsync<T>(resPath);
            T res = handle.WaitForCompletion();
            if (res == null)
            {
                LogLogger.LogVerbose(LogTag.LOG_Asset, $"res load failed:{resPath}");
            }
            return res;
        }
        catch (Exception e)
        {
            LogLogger.LogVerbose(LogTag.LOG_Asset, $"[AssetModule] LoadAsset Exception:::{e}");
        }

        return null;
    }

    private static async UniTask<T> LoadAssetFromAddressable<T>(string resPath) where T : Object
    {
        try
        {
            var handle = Addressables.LoadAssetAsync<T>(resPath);
            await handle;
            await UniTask.SwitchToMainThread();
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                return handle.Result;
            }
        }
        catch (Exception e)
        {
            LogLogger.LogVerbose(LogTag.LOG_Asset, $"[AssetModule] LoadAsset Exception:::{e}");
        }

        return null;
    }

    private static async UniTask<IList<T>> LoadAssetsFromAddressable<T>(string resPath) where T : Object
    {
        try
        {
            var handle = Addressables.LoadAssetsAsync<T>(resPath, null);
            await handle;
            await UniTask.SwitchToMainThread();
            
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                return handle.Result;
            }
        }
        catch (Exception e)
        {
            LogLogger.LogVerbose(LogTag.LOG_Asset, $"[AssetModule] LoadAsset Exception:::{e}");
        }

        return null;
    }
}
