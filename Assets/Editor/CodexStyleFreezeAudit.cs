#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

[InitializeOnLoad]
internal static class CodexStyleFreezeAudit
{
    private const string FlagName = "CodexStyleFreeze.audit";
    private const string DeliveryFolder = "ArtAndUI_StyleFrozen_20260814/03_MANIFESTS";

    private static readonly string[] PageAddresses =
    {
        "UIPanel/AddPropPanel",
        "UIPanel/CommonConfirmTipsPanel",
        "UIPanel/DailyMissionPanel",
        "UIPanel/DailyWithdrawPanel",
        "UIPanel/ExchangeRatePanel",
        "UIPanel/FakeWithdrawPanel",
        "UIPanel/FAQPanel",
        "UIPanel/GetRewardPanel",
        "UIPanel/LoadingPanel",
        "UIPanel/LosePanel",
        "UIPanel/NewbieGiftPage",
        "UIPanel/PausePanel",
        "UIPanel/RealGamePanel",
        "UIPanel/RealWithdrawPanel",
        "UIPanel/ServicePanel",
        "UIPanel/ServiceSelectPanel",
        "UIPanel/SlotFAQPanel",
        "UIPanel/SlotPanel",
        "UIPanel/StarRatingPopup",
        "UIPanel/UI_DailyTaskPage",
        "UIPanel/UI_TeachFinterMove",
        "UIPanel/UI_TeachMask",
        "UIPanel/UI_TeachTip",
        "UIPanel/UIBizzaAAA",
        "UIPanel/UITeachMaskFocusPage",
        "UIPanel/UIWithdrawalConfirmPanel",
        "UIPanel/UIWithdrawalPanel",
        "UIPanel/WithdrawDanPanel",
        "UIPanel/WithdrawHistory"
    };

    private static readonly string[] RuntimeDynamicRoots =
    {
        "Assets/GamePlayProject/Scenes/GamePlay.unity",
        "Assets/GamePlayProject/Resources/BlockTap/BlockTapVisualConfig.asset",
        "Assets/GamePlayProject/Resources/BlockTap/BlockTapPrefabConfig.asset",
        "Assets/GamePlayProject/Resources/BlockTap/Prefabs/BlockCell.prefab",
        "Assets/GamePlayProject/Resources/BlockTap/Prefabs/FlyingBlock.prefab",
        "Assets/GamePlayProject/Resources/BlockTap/Prefabs/GoalItem.prefab",
        "Assets/GamePlayProject/Resources/BlockTap/Prefabs/Spine.prefab"
    };

    static CodexStyleFreezeAudit()
    {
        EditorApplication.delayCall += RunIfRequested;
    }

    [MenuItem("Tools/Codex/Export Style-Freeze Dependencies")]
    private static void RunFromMenu()
    {
        RunAudit();
    }

    public static void ExecuteFromCommandLine()
    {
        RunAudit();
    }

    private static void RunIfRequested()
    {
        string flagPath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, FlagName);
        if (!File.Exists(flagPath))
            return;

        try
        {
            RunAudit();
            File.Delete(flagPath);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            File.WriteAllText(flagPath + ".error.txt", exception.ToString());
        }
    }

    private static void RunAudit()
    {
        string projectRoot = Directory.GetParent(Application.dataPath).FullName;
        string outputRoot = Path.Combine(projectRoot, DeliveryFolder);
        Directory.CreateDirectory(outputRoot);

        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
            throw new InvalidOperationException("AddressableAssetSettingsDefaultObject.Settings is null.");

        var addressEntries = settings.groups
            .Where(group => group != null)
            .SelectMany(group => group.entries)
            .Where(entry => entry != null)
            .GroupBy(entry => entry.address, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.Ordinal);

        var dependencyRows = new List<DependencyRow>();
        var pageRows = new List<PageRow>();

        for (int index = 0; index < PageAddresses.Length; index++)
        {
            string address = PageAddresses[index];
            if (!addressEntries.TryGetValue(address, out AddressableAssetEntry entry))
            {
                pageRows.Add(new PageRow(index + 1, address, string.Empty, "MISSING_ADDRESS", 0, 0, 0));
                continue;
            }

            string prefabPath = AssetDatabase.GUIDToAssetPath(entry.guid);
            string[] dependencies = AssetDatabase.GetDependencies(prefabPath, true)
                .Where(path => path.StartsWith("Assets/", StringComparison.Ordinal))
                .Distinct(StringComparer.Ordinal)
                .OrderBy(path => path, StringComparer.Ordinal)
                .ToArray();

            int textureCount = 0;
            int spineCount = 0;
            foreach (string dependency in dependencies)
            {
                string extension = Path.GetExtension(dependency).ToLowerInvariant();
                string kind = Classify(dependency, extension);
                if (kind == "PNG" || kind == "JPG" || kind == "PSD" || kind == "TGA")
                    textureCount++;
                if (kind.StartsWith("SPINE", StringComparison.Ordinal))
                    spineCount++;

                dependencyRows.Add(new DependencyRow(index + 1, address, prefabPath, dependency, kind));
            }

            pageRows.Add(new PageRow(index + 1, address, prefabPath, "OK", dependencies.Length, textureCount, spineCount));
        }

        WriteCsv(Path.Combine(outputRoot, "PREFAB_DEPENDENCIES.csv"),
            "Index,Address,PrefabPath,DependencyPath,Kind",
            dependencyRows.Select(row => string.Join(",", Csv(row.Index), Csv(row.Address), Csv(row.PrefabPath), Csv(row.DependencyPath), Csv(row.Kind))));

        WriteCsv(Path.Combine(outputRoot, "PREFAB_PAGE_SUMMARY.csv"),
            "Index,Address,PrefabPath,Status,DependencyCount,TextureCount,SpineCount",
            pageRows.Select(row => string.Join(",", Csv(row.Index), Csv(row.Address), Csv(row.PrefabPath), Csv(row.Status), Csv(row.DependencyCount), Csv(row.TextureCount), Csv(row.SpineCount))));

        var textureTargets = dependencyRows
            .Where(row => row.Kind == "PNG" || row.Kind == "JPG" || row.Kind == "PSD" || row.Kind == "TGA")
            .GroupBy(row => row.DependencyPath, StringComparer.Ordinal)
            .OrderBy(group => group.Key, StringComparer.Ordinal)
            .Select(group =>
            {
                string absolutePath = Path.Combine(projectRoot, group.Key.Replace('/', Path.DirectorySeparatorChar));
                string hash = File.Exists(absolutePath) ? ComputeSha256(absolutePath) : string.Empty;
                string pages = string.Join("|", group.Select(row => row.Index.ToString("D2", CultureInfo.InvariantCulture)).Distinct());
                return string.Join(",", Csv(group.Key), Csv(Path.GetFileName(group.Key)), Csv(group.First().Kind), Csv(pages), Csv(hash));
            });

        WriteCsv(Path.Combine(outputRoot, "PNG_REPLACEMENT_TARGETS.csv"),
            "AssetPath,FileName,Kind,ReferencedByPages,OriginalSHA256",
            textureTargets);

        var spineTargets = dependencyRows
            .Where(row => row.Kind.StartsWith("SPINE", StringComparison.Ordinal))
            .GroupBy(row => row.DependencyPath, StringComparer.Ordinal)
            .OrderBy(group => group.Key, StringComparer.Ordinal)
            .Select(group => string.Join(",", Csv(group.Key), Csv(group.First().Kind), Csv(string.Join("|", group.Select(row => row.Index.ToString("D2", CultureInfo.InvariantCulture)).Distinct()))));

        WriteCsv(Path.Combine(outputRoot, "SPINE_REPLACEMENT_TARGETS.csv"),
            "AssetPath,Kind,ReferencedByPages",
            spineTargets);

        var runtimeDependencyRows = new List<RuntimeDependencyRow>();
        foreach (string root in RuntimeDynamicRoots)
        {
            if (AssetDatabase.LoadMainAssetAtPath(root) == null)
            {
                runtimeDependencyRows.Add(new RuntimeDependencyRow(root, string.Empty, "MISSING_ROOT"));
                continue;
            }

            foreach (string dependency in AssetDatabase.GetDependencies(root, true)
                         .Where(path => path.StartsWith("Assets/", StringComparison.Ordinal))
                         .Distinct(StringComparer.Ordinal)
                         .OrderBy(path => path, StringComparer.Ordinal))
            {
                runtimeDependencyRows.Add(new RuntimeDependencyRow(root, dependency,
                    Classify(dependency, Path.GetExtension(dependency).ToLowerInvariant())));
            }
        }

        WriteCsv(Path.Combine(outputRoot, "RUNTIME_DYNAMIC_DEPENDENCIES.csv"),
            "RootAsset,DependencyPath,Kind",
            runtimeDependencyRows.Select(row => string.Join(",", Csv(row.RootAsset), Csv(row.DependencyPath), Csv(row.Kind))));

        var runtimeTextureTargets = runtimeDependencyRows
            .Where(row => row.Kind == "PNG" || row.Kind == "JPG" || row.Kind == "PSD" || row.Kind == "TGA")
            .GroupBy(row => row.DependencyPath, StringComparer.Ordinal)
            .OrderBy(group => group.Key, StringComparer.Ordinal)
            .Select(group =>
            {
                string absolutePath = Path.Combine(projectRoot, group.Key.Replace('/', Path.DirectorySeparatorChar));
                string hash = File.Exists(absolutePath) ? ComputeSha256(absolutePath) : string.Empty;
                string roots = string.Join("|", group.Select(row => row.RootAsset).Distinct(StringComparer.Ordinal));
                return string.Join(",", Csv(group.Key), Csv(Path.GetFileName(group.Key)), Csv(group.First().Kind), Csv(roots), Csv(hash));
            });

        WriteCsv(Path.Combine(outputRoot, "RUNTIME_DYNAMIC_PNG_TARGETS.csv"),
            "AssetPath,FileName,Kind,ReferencedByRoots,OriginalSHA256",
            runtimeTextureTargets);

        File.WriteAllText(Path.Combine(outputRoot, "PREFAB_DEPENDENCY_AUDIT.done.txt"),
            $"CompletedUtc={DateTime.UtcNow:O}\nPages={PageAddresses.Length}\nDependencies={dependencyRows.Count}\n", new UTF8Encoding(false));

        AssetDatabase.Refresh();
        Debug.Log($"[CodexStyleFreezeAudit] Exported {dependencyRows.Count} dependency rows for {PageAddresses.Length} pages to {outputRoot}.");
    }

    private static string Classify(string path, string extension)
    {
        string lower = path.ToLowerInvariant();
        if (extension == ".png") return "PNG";
        if (extension == ".jpg" || extension == ".jpeg") return "JPG";
        if (extension == ".psd") return "PSD";
        if (extension == ".tga") return "TGA";
        if (extension == ".json" && lower.Contains("spine")) return "SPINE_JSON";
        if (extension == ".skel") return "SPINE_SKEL";
        if (lower.EndsWith(".atlas.txt", StringComparison.Ordinal)) return "SPINE_ATLAS";
        if (extension == ".mat" && lower.Contains("spine")) return "SPINE_MATERIAL";
        if (extension == ".prefab") return "PREFAB";
        if (extension == ".mat") return "MATERIAL";
        if (extension == ".ttf" || extension == ".otf") return "FONT";
        return extension.TrimStart('.').ToUpperInvariant();
    }

    private static string ComputeSha256(string path)
    {
        using (var stream = File.OpenRead(path))
        using (var sha = System.Security.Cryptography.SHA256.Create())
            return BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", string.Empty);
    }

    private static void WriteCsv(string path, string header, IEnumerable<string> rows)
    {
        var builder = new StringBuilder();
        builder.AppendLine(header);
        foreach (string row in rows)
            builder.AppendLine(row);
        File.WriteAllText(path, builder.ToString(), new UTF8Encoding(true));
    }

    private static string Csv(object value)
    {
        string text = Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
        return "\"" + text.Replace("\"", "\"\"") + "\"";
    }

    private readonly struct DependencyRow
    {
        public readonly int Index;
        public readonly string Address;
        public readonly string PrefabPath;
        public readonly string DependencyPath;
        public readonly string Kind;

        public DependencyRow(int index, string address, string prefabPath, string dependencyPath, string kind)
        {
            Index = index;
            Address = address;
            PrefabPath = prefabPath;
            DependencyPath = dependencyPath;
            Kind = kind;
        }
    }

    private readonly struct PageRow
    {
        public readonly int Index;
        public readonly string Address;
        public readonly string PrefabPath;
        public readonly string Status;
        public readonly int DependencyCount;
        public readonly int TextureCount;
        public readonly int SpineCount;

        public PageRow(int index, string address, string prefabPath, string status, int dependencyCount, int textureCount, int spineCount)
        {
            Index = index;
            Address = address;
            PrefabPath = prefabPath;
            Status = status;
            DependencyCount = dependencyCount;
            TextureCount = textureCount;
            SpineCount = spineCount;
        }
    }

    private readonly struct RuntimeDependencyRow
    {
        public readonly string RootAsset;
        public readonly string DependencyPath;
        public readonly string Kind;

        public RuntimeDependencyRow(string rootAsset, string dependencyPath, string kind)
        {
            RootAsset = rootAsset;
            DependencyPath = dependencyPath;
            Kind = kind;
        }
    }
}
#endif
