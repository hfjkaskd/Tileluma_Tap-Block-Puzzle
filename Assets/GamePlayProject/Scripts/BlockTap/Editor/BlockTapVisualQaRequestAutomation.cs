#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace BlockTap.EditorTools
{
    [InitializeOnLoad]
    internal static class BlockTapVisualQaRequestAutomation
    {
        private const string RequestFileName = ".codex_visual_qa_request";
        private static bool scheduled;

        static BlockTapVisualQaRequestAutomation()
        {
            string requestPath = GetRequestPath();
            if (!File.Exists(requestPath) || scheduled)
                return;

            scheduled = true;
            EditorApplication.delayCall += Run;
        }

        private static void Run()
        {
            string requestPath = GetRequestPath();
            try
            {
                // Prefab structure and visual references are now authored and
                // transplanted directly. Never run a layout-calibration pass
                // from this automatic request hook.
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
                AssetDatabase.SaveAssets();
                BlockTapCompleteVisualBuildAutomation.BuildQaRuntime();
                File.Delete(requestPath);
                Debug.Log("[BlockTapVisualQaRequestAutomation] Refreshed the structure-locked assets and rebuilt the QA runtime.");
            }
            catch (Exception exception)
            {
                scheduled = false;
                Debug.LogException(exception);
            }
        }

        private static string GetRequestPath()
        {
            return Path.GetFullPath(Path.Combine(Application.dataPath, "..", RequestFileName));
        }
    }
}
#endif
