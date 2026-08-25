using System;
using Bizza.Sdk;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildVersionRecorder : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        try
        {
            WriteVersionLogToSO();
        }
        catch (Exception e)
        {
            Debug.LogError($"[BuildVersionRecorder] 写入 versionLog 失败：{e}");
        }
    }

    private static void WriteVersionLogToSO()
    {
        if (ChannelConfig.Instance == null)
        {
            Debug.LogError("[BuildVersionRecorder] ChannelConfig.Instance 为 null，无法写入 versionLog。");
            return;
        }

        string path = AssetDatabase.GetAssetPath(ChannelConfig.Instance);
        Debug.Log($"[BuildVersionRecorder] ChannelConfig path: {path}");

        string version = PlayerSettings.bundleVersion;
        string timeString = DateTime.Now.ToString("MMddHHmm");
        string versionLog = $"{timeString}_{version}";

        var config = ChannelConfig.Instance;

        var customConfig = config.real_CustomConfig;
        customConfig.versionLog = versionLog;
        config.real_CustomConfig = customConfig;

        EditorUtility.SetDirty(config);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"[BuildVersionRecorder] versionLog 写入成功：{config.real_CustomConfig.versionLog}");
    }
}