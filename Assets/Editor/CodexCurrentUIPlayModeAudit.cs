#if UNITY_EDITOR
using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
internal static class CodexCurrentUIPlayModeAudit
{
    private const string StartFlagName = ".codex-start-current-ui-audit";
    private const string WithdrawOnlyFlagName = ".codex-start-withdraw-only-audit";
    private const string ApplyBatch21FlagName = ".codex-apply-batch21-withdraw";
    private const string RequestFileName = ".codex-current-ui-capture.request";
    private const string PauseFlagName = ".codex-pause-current-ui-audit";
    private const string ActiveKey = "CodexCurrentUIPlayModeAudit.Active";
    private const string IndexKey = "CodexCurrentUIPlayModeAudit.Index";
    private const string StartIndexKey = "CodexCurrentUIPlayModeAudit.StartIndex";
    private const string StopIndexKey = "CodexCurrentUIPlayModeAudit.StopIndex";
    private const string OutputRootName = "CurrentUI_OneToOne_Audit_20260821";

    private static readonly CapturePage[] Pages =
    {
        new CapturePage("01_AddPropPanel", "UIPanel/AddPropPanel"),
        new CapturePage("02_CommonConfirmTipsPanel", "UIPanel/CommonConfirmTipsPanel"),
        new CapturePage("03_DailyMissionPanel", "UIPanel/DailyMissionPanel"),
        new CapturePage("04_DailyWithdrawPanel", "UIPanel/DailyWithdrawPanel"),
        new CapturePage("05_ExchangeRatePanel", "UIPanel/ExchangeRatePanel"),
        new CapturePage("06_FakeWithdrawPanel", "UIPanel/FakeWithdrawPanel"),
        new CapturePage("07_FAQPanel", "UIPanel/FAQPanel"),
        new CapturePage("08_GetRewardPanel", "UIPanel/GetRewardPanel"),
        new CapturePage("09_LoadingPanel", "UIPanel/LoadingPanel"),
        new CapturePage("10_LosePanel", "UIPanel/LosePanel"),
        new CapturePage("11_NewbieGiftPage", "UIPanel/NewbieGiftPage"),
        new CapturePage("12_PausePanel", "UIPanel/PausePanel"),
        new CapturePage("13_RealGamePanel", "Gameplay"),
        new CapturePage("14_RealWithdrawPanel", "UIPanel/RealWithdrawPanel"),
        new CapturePage("15_ServicePanel", "UIPanel/ServicePanel"),
        new CapturePage("16_ServiceSelectPanel", "UIPanel/ServiceSelectPanel"),
        new CapturePage("17_SlotFAQPanel", "UIPanel/SlotFAQPanel"),
        new CapturePage("18_SlotPanel", "UIPanel/SlotPanel"),
        new CapturePage("19_StarRatingPopup", "UIPanel/StarRatingPopup"),
        new CapturePage("20_UI_DailyTaskPage", "UIPanel/UI_DailyTaskPage"),
        new CapturePage("21_UI_TeachFinterMove", "UIPanel/UI_TeachFinterMove"),
        new CapturePage("22_UI_TeachMask", "UIPanel/UI_TeachMask"),
        new CapturePage("23_UI_TeachTip", "UIPanel/UI_TeachTip"),
        new CapturePage("24_UIBizzaAAA", "UIPanel/UIBizzaAAA"),
        new CapturePage("25_UITeachMaskFocusPage", "UIPanel/UITeachMaskFocusPage"),
        new CapturePage("26_UIWithdrawalConfirmPanel", "UIPanel/UIWithdrawalConfirmPanel"),
        new CapturePage("27_UIWithdrawalPanel", "UIPanel/UIWithdrawalPanel"),
        new CapturePage("28_WithdrawDanPanel", "UIPanel/WithdrawDanPanel"),
        new CapturePage("29_WithdrawHistory", "UIPanel/WithdrawHistory")
    };

    static CodexCurrentUIPlayModeAudit()
    {
        // Recompile hook for the current one-to-one withdraw verification pass.
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        EditorApplication.delayCall += ResumeOrStart;
    }

    [MenuItem("Tools/Codex/Capture Current UI Audit (PlayMode)")]
    private static void StartFromMenu()
    {
        Begin();
    }

    [MenuItem("Tools/Codex/Capture Withdraw Only (PlayMode)")]
    private static void CaptureWithdrawOnlyFromMenu()
    {
        BeginRange(13, 14, "Single current Unity PlayMode UI audit: RealWithdrawPanel");
    }

    private static string ProjectRoot => Directory.GetParent(Application.dataPath).FullName;
    private static string StartFlagPath => Path.Combine(ProjectRoot, StartFlagName);
    private static string WithdrawOnlyFlagPath => Path.Combine(ProjectRoot, WithdrawOnlyFlagName);
    private static string ApplyBatch21FlagPath => Path.Combine(ProjectRoot, ApplyBatch21FlagName);
    private static string RequestPath => Path.Combine(ProjectRoot, RequestFileName);
    private static string PauseFlagPath => Path.Combine(ProjectRoot, PauseFlagName);
    private static string OutputRoot => Path.Combine(ProjectRoot, OutputRootName);
    private static string RuntimeRoot => Path.Combine(OutputRoot, "03_AfterFix01_PlayMode");
    private static string ProgressPath => Path.Combine(OutputRoot, "CAPTURE_PROGRESS.txt");

    private static void ResumeOrStart()
    {
        if (EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
        {
            EditorApplication.delayCall += ResumeOrStart;
            return;
        }

        if (File.Exists(ApplyBatch21FlagPath))
        {
            File.Delete(ApplyBatch21FlagPath);
            OneToOneVisualRestorer.ApplyBatch21WithdrawCorrectAspectMatch();
            EditorApplication.delayCall += ResumeOrStart;
            return;
        }

        if (File.Exists(WithdrawOnlyFlagPath))
        {
            File.Delete(WithdrawOnlyFlagPath);
            BeginRange(13, 14, "Single current Unity PlayMode UI audit: RealWithdrawPanel");
            return;
        }

        if (File.Exists(PauseFlagPath))
            return;

        if (File.Exists(StartFlagPath))
        {
            File.Delete(StartFlagPath);
            Begin();
            return;
        }

        if (SessionState.GetBool(ActiveKey, false))
            StartNext();
    }

    private static void Begin()
    {
        BeginRange(0, Pages.Length, "Current Unity PlayMode UI audit");
    }

    private static void BeginRange(int startIndex, int stopIndex, string progressTitle)
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            return;

        if (File.Exists(PauseFlagPath))
            File.Delete(PauseFlagPath);

        Directory.CreateDirectory(RuntimeRoot);
        SessionState.SetBool(ActiveKey, true);
        SessionState.SetInt(IndexKey, startIndex);
        SessionState.SetInt(StartIndexKey, startIndex);
        SessionState.SetInt(StopIndexKey, stopIndex);
        File.WriteAllText(ProgressPath,
            progressTitle + "\nStarted=" + DateTime.Now.ToString("O") + "\n",
            new UTF8Encoding(false));
        StartNext();
    }

    private static void StartNext()
    {
        if (!SessionState.GetBool(ActiveKey, false) || EditorApplication.isPlayingOrWillChangePlaymode)
            return;

        int index = SessionState.GetInt(IndexKey, 0);
        int startIndex = SessionState.GetInt(StartIndexKey, 0);
        int stopIndex = SessionState.GetInt(StopIndexKey, Pages.Length);
        if (index >= stopIndex || index >= Pages.Length)
        {
            SessionState.SetBool(ActiveKey, false);
            if (File.Exists(RequestPath))
                File.Delete(RequestPath);
            File.AppendAllText(ProgressPath,
                "Completed=" + DateTime.Now.ToString("O") + "\nCount=" + Math.Max(0, Math.Min(stopIndex, Pages.Length) - startIndex) + "\n",
                new UTF8Encoding(false));
            AssetDatabase.Refresh();
            Debug.Log("[CodexCurrentUIPlayModeAudit] Completed PlayMode capture range " + startIndex + ".." + stopIndex + " at " + RuntimeRoot);
            return;
        }

        CapturePage page = Pages[index];
        string outputPath = Path.Combine(RuntimeRoot, page.FileName + ".png");
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
        File.WriteAllText(RequestPath, outputPath + Environment.NewLine + page.Address + Environment.NewLine,
            new UTF8Encoding(false));
        Debug.Log("[CodexCurrentUIPlayModeAudit] Capturing " + (index + 1) + "/" + Pages.Length + ": " + page.Address);
        EditorApplication.isPlaying = true;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (!SessionState.GetBool(ActiveKey, false) || state != PlayModeStateChange.EnteredEditMode)
            return;

        int index = SessionState.GetInt(IndexKey, 0);
        if (index < Pages.Length)
        {
            CapturePage page = Pages[index];
            string outputPath = Path.Combine(RuntimeRoot, page.FileName + ".png");
            File.AppendAllText(ProgressPath,
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
