#if UNITY_EDITOR
using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
internal static class CodexOriginalScreenshotLayoutAudit
{
    private const string ActiveKey = "CodexOriginalScreenshotLayoutAudit.Active";
    private const string IndexKey = "CodexOriginalScreenshotLayoutAudit.Index";
    private const string RequestFileName = ".codex-current-ui-capture.request";
    private const string OutputFolderName = "OriginalScreenshotLayoutValidation_20260825";

    private static readonly CapturePage[] Pages =
    {
        new CapturePage("01_DailyTasks.png", "UIPanel/UI_DailyTaskPage"),
        new CapturePage("02_BonusSpin.png", "UIPanel/SlotPanel"),
        new CapturePage("03_LevelComplete.png", "UIPanel/GetRewardPanel"),
        new CapturePage("04_SlotRewards.png", "UIPanel/SlotFAQPanel")
    };

    static CodexOriginalScreenshotLayoutAudit()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        EditorApplication.delayCall += Resume;
    }

    [MenuItem("Tools/Codex/Capture Original Screenshot Layouts (PlayMode)")]
    private static void StartFromMenu()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            return;
        }

        Directory.CreateDirectory(OutputRoot);
        SessionState.SetBool(ActiveKey, true);
        SessionState.SetInt(IndexKey, 0);
        File.WriteAllText(
            ProgressPath,
            "Original screenshot layout validation\nStarted=" + DateTime.Now.ToString("O") + "\n",
            new UTF8Encoding(false));
        StartNext();
    }

    private static string ProjectRoot => Directory.GetParent(Application.dataPath).FullName;
    private static string RequestPath => Path.Combine(ProjectRoot, RequestFileName);
    private static string OutputRoot => Path.Combine(ProjectRoot, OutputFolderName);
    private static string ProgressPath => Path.Combine(OutputRoot, "CAPTURE_PROGRESS.txt");

    private static void Resume()
    {
        if (!SessionState.GetBool(ActiveKey, false) ||
            EditorApplication.isCompiling ||
            EditorApplication.isPlayingOrWillChangePlaymode)
        {
            return;
        }

        StartNext();
    }

    private static void StartNext()
    {
        int index = SessionState.GetInt(IndexKey, 0);
        if (index >= Pages.Length)
        {
            SessionState.SetBool(ActiveKey, false);
            if (File.Exists(RequestPath))
            {
                File.Delete(RequestPath);
            }

            File.AppendAllText(
                ProgressPath,
                "Completed=" + DateTime.Now.ToString("O") + "\nCount=" + Pages.Length + "\n",
                new UTF8Encoding(false));
            AssetDatabase.Refresh();
            Debug.Log("[CodexOriginalScreenshotLayoutAudit] Completed PlayMode validation at " + OutputRoot);
            return;
        }

        CapturePage page = Pages[index];
        string outputPath = Path.Combine(OutputRoot, page.FileName);
        File.WriteAllText(
            RequestPath,
            outputPath + Environment.NewLine + page.Address + Environment.NewLine,
            new UTF8Encoding(false));
        Debug.Log("[CodexOriginalScreenshotLayoutAudit] Capturing " + (index + 1) + "/" + Pages.Length + ": " + page.Address);
        EditorApplication.isPlaying = true;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (!SessionState.GetBool(ActiveKey, false) || state != PlayModeStateChange.EnteredEditMode)
        {
            return;
        }

        int index = SessionState.GetInt(IndexKey, 0);
        if (index < Pages.Length)
        {
            CapturePage page = Pages[index];
            string outputPath = Path.Combine(OutputRoot, page.FileName);
            File.AppendAllText(
                ProgressPath,
                page.FileName + "\t" + (File.Exists(outputPath) ? "OK" : "MISSING") + "\n",
                new UTF8Encoding(false));
        }

        SessionState.SetInt(IndexKey, index + 1);
        EditorApplication.delayCall += StartNext;
    }

    private readonly struct CapturePage
    {
        public readonly string FileName;
        public readonly string Address;

        public CapturePage(string fileName, string address)
        {
            FileName = fileName;
            Address = address;
        }
    }
}
#endif
