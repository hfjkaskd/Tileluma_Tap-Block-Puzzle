using System.Collections.Generic;
using BlockTap.Config;
using UnityEngine;

namespace BlockTap.Importing
{
    public static class BlockTapDefaultContent
    {
        public static BlockTapLevelDatabaseSO LoadOrCreateDatabase()
        {
            var database = Resources.Load<BlockTapLevelDatabaseSO>("BlockTap/BlockTapLevelDatabase");
            if (database != null)
            {
                return database;
            }

            database = ScriptableObject.CreateInstance<BlockTapLevelDatabaseSO>();
            if (BlockTapSourceImporter.TryImport(out var importedLevels))
            {
                var levelAssets = new List<BlockTapLevelConfigSO>(importedLevels.Count);
                foreach (var importedLevel in importedLevels)
                {
                    var levelAsset = ScriptableObject.CreateInstance<BlockTapLevelConfigSO>();
                    levelAsset.pass = importedLevel.pass;
                    levelAsset.type = importedLevel.type;
                    levelAsset.num = importedLevel.num;
                    levelAsset.collect = importedLevel.collect;
                    levelAssets.Add(levelAsset);
                }

                database.SetLevels(levelAssets);
                return database;
            }

            database.SetLevels(new List<BlockTapLevelConfigSO>
            {
                CreateFallbackLevel(0, 2, 2, new[] { 8, 8 }),
                CreateFallbackLevel(1, 2, 2, new[] { 8, 8 }),
                CreateFallbackLevel(2, 2, 2, new[] { 10, 10 }),
            });
            return database;
        }

        public static BlockTapGameplayConfigSO CreateGameplayConfig()
        {
            var config = Resources.Load<BlockTapGameplayConfigSO>("BlockTap/BlockTapGameplayConfig");
            return config != null ? config : ScriptableObject.CreateInstance<BlockTapGameplayConfigSO>();
        }

        public static BlockTapVisualConfigSO CreateVisualConfig()
        {
            var config = Resources.Load<BlockTapVisualConfigSO>("BlockTap/BlockTapVisualConfig");
            return config != null ? config : ScriptableObject.CreateInstance<BlockTapVisualConfigSO>();
        }

        public static BlockTapAudioConfigSO CreateAudioConfig()
        {
            var config = Resources.Load<BlockTapAudioConfigSO>("BlockTap/BlockTapAudioConfig");
            return config != null ? config : ScriptableObject.CreateInstance<BlockTapAudioConfigSO>();
        }

        public static BlockTapPrefabConfigSO CreatePrefabConfig()
        {
            var config = Resources.Load<BlockTapPrefabConfigSO>("BlockTap/BlockTapPrefabConfig");
            return config != null ? config : ScriptableObject.CreateInstance<BlockTapPrefabConfigSO>();
        }

        private static BlockTapLevelConfigSO CreateFallbackLevel(int pass, int type, int num, int[] collect)
        {
            var level = ScriptableObject.CreateInstance<BlockTapLevelConfigSO>();
            level.pass = pass;
            level.type = type;
            level.num = num;
            level.collect = collect;
            return level;
        }
    }
}
