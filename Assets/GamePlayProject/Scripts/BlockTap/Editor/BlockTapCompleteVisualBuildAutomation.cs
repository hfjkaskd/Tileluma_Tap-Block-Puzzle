#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace BlockTap.EditorTools
{
    [InitializeOnLoad]
    internal static class BlockTapCompleteVisualBuildAutomation
    {
        private const string OutputArgument = "-completeVisualBuildOutput";
        private static bool buildScheduled;

        static BlockTapCompleteVisualBuildAutomation()
        {
            string outputPath = GetArgument(Environment.GetCommandLineArgs(), OutputArgument);
            if (string.IsNullOrWhiteSpace(outputPath) || buildScheduled)
                return;

            buildScheduled = true;
            EditorApplication.delayCall += BuildNow;
        }

        public static void BuildNow()
        {
            string outputPath = GetArgument(Environment.GetCommandLineArgs(), OutputArgument);
            if (string.IsNullOrWhiteSpace(outputPath))
                return;

            try
            {
                BuildPlayerAt(outputPath);
                EditorApplication.Exit(0);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                EditorApplication.Exit(1);
            }
        }

        [MenuItem("Tools/Block Tap/One-to-One Restore/Build Visual Runtime")]
        public static void BuildFromOpenEditor()
        {
            try
            {
                BuildQaRuntime();
                EditorUtility.DisplayDialog("Visual Runtime", "Build completed successfully.", "OK");
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                EditorUtility.DisplayDialog("Visual Runtime", "Build failed. See Console for details.", "OK");
            }
        }

        public static void BuildQaRuntime()
        {
            string outputPath = Path.GetFullPath(Path.Combine(
                Application.dataPath,
                "..",
                "..",
                "..",
                "_unity_visual_validation",
                "ApprovedSecondaryStyleRuntime",
                "TapBlock_Stars.exe"));
            BuildPlayerAt(outputPath);
        }

        private static void BuildPlayerAt(string outputPath)
        {
            outputPath = Path.GetFullPath(outputPath);
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneWindows64)
            {
                if (!EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64))
                    throw new InvalidOperationException("Could not switch to StandaloneWindows64.");
            }
            EditorUserBuildSettings.selectedStandaloneTarget = BuildTarget.StandaloneWindows64;
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.Mono2x);

            // The player build clears StreamingAssets/aa. Rebuild Addressables
            // first so every runtime screenshot resolves the freshly modified
            // panel prefabs instead of falling back to an empty gameplay view.
            AddressableAssetSettings.BuildPlayerContent(out AddressablesPlayerBuildResult addressablesResult);
            if (!string.IsNullOrWhiteSpace(addressablesResult.Error))
                throw new InvalidOperationException("Addressables build failed: " + addressablesResult.Error);

            string[] candidateScenes =
            {
                "Assets/Game/Resources/Scenes/InitWZ.unity",
                "Assets/GamePlayProject/Scenes/GamePlay.unity"
            };
            string[] scenes = candidateScenes.Where(File.Exists).ToArray();
            if (scenes.Length == 0)
                throw new FileNotFoundException("No runtime scenes were found for the visual capture build.");

            EditorBuildSettings.scenes = scenes
                .Select(scenePath => new EditorBuildSettingsScene(scenePath, true))
                .ToArray();

            var options = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = outputPath,
                target = BuildTarget.StandaloneWindows64,
                targetGroup = BuildTargetGroup.Standalone,
                options = BuildOptions.None
            };

            AssetDatabase.SaveAssets();
            BuildReport report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
                throw new InvalidOperationException("Player build failed: " + report.summary.result);

            Debug.Log("[BlockTapCompleteVisualBuildAutomation] Built visual capture player: " + outputPath);
        }

        private static string GetArgument(string[] args, string name)
        {
            for (int index = 0; index + 1 < args.Length; index++)
            {
                if (string.Equals(args[index], name, StringComparison.OrdinalIgnoreCase))
                    return args[index + 1];
            }
            return null;
        }
    }
}
#endif
