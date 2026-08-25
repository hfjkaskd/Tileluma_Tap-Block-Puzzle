using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace BlockTap.Importing
{
    public static class BlockTapSourceImporter
    {
        private const string DefaultSourcePath = @"E:\Unpack\BlockTap\Block+Tap_1.0.3_apkcombo.com_restored_js\main\index.js";
        private static readonly Regex TableRegex = new Regex(
            "e\\(\"GameConfigTable\",(\\[\\{pass:0.*?\\])\\),e\\(\"CashReward_US\"",
            RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex EntryRegex = new Regex(
            "\\{pass:(\\d+),type:(\\d+),num:(\\d+),collect:\\[([0-9,]+)\\]\\}",
            RegexOptions.Compiled);

        public static bool TryImport(out List<BlockTapRawLevelData> levels, string sourcePath = DefaultSourcePath)
        {
            levels = null;

            if (!File.Exists(sourcePath))
            {
                Debug.LogWarning($"BlockTap source file not found: {sourcePath}");
                return false;
            }

            var text = File.ReadAllText(sourcePath);
            var tableMatch = TableRegex.Match(text);
            if (!tableMatch.Success)
            {
                Debug.LogWarning("Failed to locate GameConfigTable in unpacked BlockTap source.");
                return false;
            }

            var tableText = tableMatch.Groups[1].Value;
            var matches = EntryRegex.Matches(tableText);
            levels = new List<BlockTapRawLevelData>(matches.Count);
            foreach (Match match in matches)
            {
                var collectParts = match.Groups[4].Value.Split(',');
                var collect = new int[collectParts.Length];
                for (var i = 0; i < collectParts.Length; i++)
                {
                    collect[i] = int.Parse(collectParts[i]);
                }

                levels.Add(new BlockTapRawLevelData
                {
                    pass = int.Parse(match.Groups[1].Value),
                    type = int.Parse(match.Groups[2].Value),
                    num = int.Parse(match.Groups[3].Value),
                    collect = collect,
                });
            }

            return levels.Count > 0;
        }
    }

    public class BlockTapRawLevelData
    {
        public int pass;
        public int type;
        public int num;
        public int[] collect;
    }
}
