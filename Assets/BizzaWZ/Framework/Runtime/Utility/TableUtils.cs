using System;
using System.Collections;
using System.Collections.Generic;
using cfg;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class D_ConfigPath
{
    private const string Config = "Config/";
    public const string Tables = Config + nameof(Tables);
    public const string Tables01 = Tables + "/01";
    public const string Tables02 = Tables + "/02";
    public const string Tables03 = Tables + "/03";
}


public static class TableUtils
{
    public static TblGlobal Global => Tables.TblGlobal;

    public static Tables Tables => Tables.Instance;
    
    public static readonly string[] TablePaths = new[] { "Table", "Table01", string.Empty, "Table02" };

    public static string GetTablesPath()
    {
        int group = 0;//todo:表格配置用户分组
        return group switch
        {
            0 => "ConfigAssets/TableJson/Table01",
            1 => "ConfigAssets/TableJson/Table02",
            3 => "ConfigAssets/TableJson/Table03",
            _ => "ConfigAssets/TableJson/Table01",
        };
    }

    //提前加载多语言表
    public static async UniTask LoadLanguageTable()
    {
        var path = TableUtils.GetTablesPath();
        // var handle = AssetUtils.LoadAssetAsync<TextAsset>(path + "/tbllanguage");
        // var file = handle.AsTask().Result;
        var file = Resources.Load<TextAsset>(path + "/tbllanguage");
        Tables.Instance.InitTables(new Dictionary<string, TextAsset>()
        {
            {file.name, file},
        });
    }
    
    public static async UniTask LoadTableConfigs()
    {
        LogLogger.LogInfo($"{nameof(LoadTableConfigs)} start");

        var path = TableUtils.GetTablesPath();
        // var result = await AssetModule.LoadAssetAsync<TextAssetList>(path);
        // var handle = AssetUtils.LoadAssetsAsync<TextAsset>(path);
        //await handle;
        //if (handle.Status == UniTaskStatus.Succeeded)
        //{
        //     List<KeyValuePair<string, TextAsset>> list = new();
        //     foreach (var v in handle.AsTask().Result)
        //     {
        //         list.Add(new(v.name, v));
        //     }

        //     Tables.Instance.InitTables(list);

        //     foreach (var v in handle.AsTask().Result)
        //     {
        //         Resources.UnloadAsset(v);    
        //     }
        // }
        var result = Resources.LoadAll<TextAsset>(path);
        if (result != null)
        {
            List<KeyValuePair<string, TextAsset>> list = new();
            foreach (var v in result)
            {
                list.Add(new(v.name, v));
            }
            Tables.Instance.InitTables(list);
            foreach (var v in result)
            {
                Resources.UnloadAsset(v);
            }
        }
        else
        {
            LogLogger.LogError($"load table failed:{path}");
        }
    }
    
    public static LevelConfig GetLevelConfig(int _level = -1)
    {
        LevelConfig cfg;
        if (_level == 0)
        {
             cfg = TableUtils.Tables.TblLevel.GetOrDefault(_level);
             return cfg;
        }

        
        int actuallyLevel = Mathf.Clamp(_level, 1, TableUtils.Tables.TblLevel.DataList.Count - 1);
        cfg = TableUtils.Tables.TblLevel.GetOrDefault(actuallyLevel);
        // var curLevel = SaveDataUtils.GameData.playerSelectedLv;
        // bool isExtent = curLevel >= TableUtils.Tables.TblLevel.DataList.Count;
        // LogUtil.Error($"curLevel {curLevel} --- {TableUtils.Tables.TblLevel.DataList.Count}");
        // int level = isExtent ? TableUtils.Tables.TblLevel.DataList.Count - 1 : curLevel;
        // cfg = TableUtils.Tables.TblLevel.GetOrDefault(level);
        return cfg;
    }

    public static int GetCorrectLevel(int level = -1)
    {
        if (level == -1)
        {
            level = SaveDataUtils.GameData.playerSelectedLv;
        }

        return Mathf.Min(level, TableUtils.Tables.TblLevel.DataList[^1].Id);
    }
}
