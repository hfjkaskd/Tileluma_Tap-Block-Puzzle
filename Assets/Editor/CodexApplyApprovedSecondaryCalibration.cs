#if UNITY_EDITOR
using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
internal static class CodexApplyApprovedSecondaryCalibration
{
    private const string FlagName = ".codex-apply-structure-locked-secondary-calibration";
    private const string StatusRelativePath = "CurrentUI_OneToOne_Audit_20260821/APPLY_STRUCTURE_LOCKED_CALIBRATION.txt";

    static CodexApplyApprovedSecondaryCalibration()
    {
        EditorApplication.delayCall += TryApply;
    }

    private static void TryApply()
    {
        if (EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
        {
            EditorApplication.delayCall += TryApply;
            return;
        }

        string projectRoot = Directory.GetParent(Application.dataPath).FullName;
        string flagPath = Path.Combine(projectRoot, FlagName);
        if (!File.Exists(flagPath))
            return;

        File.Delete(flagPath);
        string statusPath = Path.Combine(projectRoot, StatusRelativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(statusPath));

        try
        {
            File.WriteAllText(statusPath, "Started=" + DateTime.Now.ToString("O") + "\n", new UTF8Encoding(false));
            OneToOneVisualRestorer.ApplyBatch09StructureLocked();
            File.AppendAllText(statusPath, "Completed=" + DateTime.Now.ToString("O") + "\n", new UTF8Encoding(false));
            Debug.Log("[CodexApplyApprovedSecondaryCalibration] Structure-locked Batch 09 calibration applied.");
        }
        catch (Exception exception)
        {
            File.AppendAllText(statusPath, "Failed=" + DateTime.Now.ToString("O") + "\n" + exception + "\n", new UTF8Encoding(false));
            Debug.LogException(exception);
        }
    }
}
#endif
