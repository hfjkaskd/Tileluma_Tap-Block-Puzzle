#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Restores the production prefabs that produced the initially approved UI:
/// the compact daily-task cards, the complete slot machine, and the original
/// Congratulations reward overlay. The snapshots live outside Assets so they
/// are not imported as duplicate prefabs.
/// </summary>
internal static class OriginalScreenshotLayoutRestorer
{
    private readonly struct RestoreEntry
    {
        public RestoreEntry(string backup, string target)
        {
            Backup = backup;
            Target = target;
        }

        public string Backup { get; }
        public string Target { get; }
    }

    private static readonly RestoreEntry[] Entries =
    {
        new RestoreEntry(
            "CurrentUI_OneToOne_Audit_20260821/00_PreBatch08Prefabs/Assets/BizzaWZ/MenuSystem/Common/Task/UIDailyTaskPage.prefab",
            "Assets/BizzaWZ/MenuSystem/Common/Task/UIDailyTaskPage.prefab"),
        new RestoreEntry(
            "CurrentUI_OneToOne_Audit_20260821/00_PreBatch08Prefabs/Assets/BizzaWZ/MenuSystem/Common/Task/UIDailyTaskElement.prefab",
            "Assets/BizzaWZ/MenuSystem/Common/Task/UIDailyTaskElement.prefab"),
        new RestoreEntry(
            "ArtAndUI_OneToOneRestore_20260818/00_CurrentBaseline/Assets/BizzaWZ/Real/UI/SlotsPanel/SlotPanel/SlotPanel.prefab",
            "Assets/BizzaWZ/Real/UI/SlotsPanel/SlotPanel/SlotPanel.prefab"),
        new RestoreEntry(
            "ArtAndUI_OneToOneRestore_20260818/00_CurrentBaseline/Assets/BizzaWZ/Real/UI/GetRewardPanel/GetRewardPanel.prefab",
            "Assets/BizzaWZ/Real/UI/GetRewardPanel/GetRewardPanel.prefab"),
        new RestoreEntry(
            "ArtAndUI_OneToOneRestore_20260818/00_CurrentBaseline/Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/SlotPanel/SlotOther/Slot.png",
            "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/SlotPanel/SlotOther/Slot.png")
    };

    [MenuItem("Tools/Block Tap/Restore Initially Approved Layouts")]
    public static void Apply()
    {
        string projectRoot = Directory.GetParent(Application.dataPath)?.FullName
            ?? throw new InvalidOperationException("Unable to resolve the Unity project root.");

        foreach (RestoreEntry entry in Entries)
        {
            string source = Path.Combine(projectRoot, entry.Backup.Replace('/', Path.DirectorySeparatorChar));
            string destination = Path.Combine(projectRoot, entry.Target.Replace('/', Path.DirectorySeparatorChar));
            if (!File.Exists(source))
            {
                throw new FileNotFoundException("Approved prefab snapshot is missing.", source);
            }

            File.Copy(source, destination, true);
            AssetDatabase.ImportAsset(entry.Target, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        Debug.Log("[OriginalScreenshotLayoutRestorer] Initially approved Daily Tasks, SlotPanel, and GetRewardPanel prefabs restored.");
    }
}
#endif
