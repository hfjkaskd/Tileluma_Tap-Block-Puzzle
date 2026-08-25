using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 
[Serializable]
public class LevelStatisticData
{
    public int levelPlayTimes;
    public int levelPlaySeconds;
}


public partial class GameSaveData
{
    public int totalLevelTimes;
    public Dictionary<int, LevelStatisticData> levelStatisticData = new();
}

public class StatisticModule : BaseGameModule<StatisticModule>, IUpdate
{
    public bool Enabled => true;
    public static int recordInterval = 10;
    private float _lastRecordTime;

    public void OnUpdate(float delta)
    {
        // var playerData = SaveDataUtils.GameData;
        // if (playerData == null)
        // {
        //     return;
        // }
        // if (Time.realtimeSinceStartup - _lastRecordTime > recordInterval)
        // {
        //     _lastRecordTime = Time.realtimeSinceStartup;
        //     playerData.gameTimeSeconds += recordInterval;
        //     if (!AdModule.bPlayingAd)
        //     {
        //         playerData.gameTimeSecondsWithoutAd += recordInterval;
        //     }
        //
        //     if (playerData.gameTimeSecondsArray != null)
        //     {
        //         var idx = Mathf.Max(playerData.totalLoginDay - 1, 0);
        //         if (idx < playerData.gameTimeSecondsArray.Length)
        //         {
        //             playerData.gameTimeSecondsArray[idx] += recordInterval;
        //         }
        //     }
        //
        //     SaveDataUtils.gameStrategy.SaveData();
        // }
    }

    public void TeachEvent(int idx)
    {
        // Bizza.Analytics.Manager.AddCommonParams();
        // Bizza.Analytics.Manager.AddParam("teach_idx", idx);
        // Bizza.Analytics.Manager.SendCustomEvent("c_teach_finish");
    }

    private float _startTime;
    public void OnLevelStart(int levelId)
    {
        // _propUseTimes[E_ItemType.GameProp_Back] = 0;
        // _propUseTimes[E_ItemType.GameProp_Refresh] = 0;
        // _propUseTimes[E_ItemType.GameProp_ClearArea] = 0;
        // _startTime = Time.realtimeSinceStartup;
        //
        // SaveDataUtils.GameData.totalLevelTimes++;
        // SaveDataUtils.GameData.levelStatisticData.TryAdd(levelId, new LevelStatisticData());
        // SaveDataUtils.GameData.levelStatisticData[levelId].levelPlayTimes++;
        //
        // Bizza.Analytics.Manager.AddCommonParams();
        // AddLevelCommonParams(levelId);
        // Bizza.Analytics.Manager.SendCustomEvent("c_begin_game");
    }

    public void OnLevelFinish(int levelId, float progress, string endReason)
    {
        // var playTimeSeconds = Time.realtimeSinceStartup - _startTime;
        // SaveDataUtils.GameData.levelStatisticData[levelId].levelPlaySeconds += (int)playTimeSeconds;
        //
        // Bizza.Analytics.Manager.AddCommonParams();
        // AddLevelCommonParams(levelId);
        // Bizza.Analytics.Manager.AddParam("game_progress", Mathf.RoundToInt(progress * 100));
        // Bizza.Analytics.Manager.AddParam("settlement_state", endReason);
        // Bizza.Analytics.Manager.AddParam("item1_use_times", MatchManager.Instance._propUseTimes[E_ItemType.GameProp_Back]);
        // Bizza.Analytics.Manager.AddParam("item2_use_times", MatchManager.Instance._propUseTimes[E_ItemType.GameProp_ClearArea]);
        // Bizza.Analytics.Manager.AddParam("item3_use_times", MatchManager.Instance._propUseTimes[E_ItemType.GameProp_Refresh]);
        // Bizza.Analytics.Manager.AddParam("revive_times", MatchManager.Instance.reviveTimes);
        // Bizza.Analytics.Manager.SendCustomEvent("c_finish_game");
        //
        // UploadUseProperties();
    }

    private void UploadUseProperties()
    {
        // foreach (var v in SaveDataUtils.GameData.levelStatisticData)
        // {
        //     AnalyticsHelper.SetUserProperty($"level{v.Key}_succeed_seconds", v.Value.levelPlaySeconds);
        // }
        // foreach (var v in SaveDataUtils.GameData.levelStatisticData)
        // {
        //     AnalyticsHelper.SetUserProperty($"level{v.Key}_succeed_count", v.Value.levelPlayTimes);
        // }
        //
        // AnalyticsHelper.SetUserProperty("login_day", SaveDataUtils.GameData.totalLoginDay);
        // AnalyticsHelper.SetUserProperty("max_level", SaveDataUtils.GameData.playerUnlockedLv);
        // AnalyticsHelper.SetUserProperty("history_total_money", ItemUtils.GetItemCountInt(E_ItemType.Dollar));
        // AnalyticsHelper.SetUserProperty("total_finishad_times", SaveDataUtils.GameData.totalFinishAdTimes);
        //
        // var playerData = SaveDataUtils.GameData;
        // AnalyticsHelper.SetUserProperty("gametime_seconds", playerData.gameTimeSeconds);
        // if (playerData.gameTimeSecondsArray != null)
        // {
        //     for (int i = 0; i < playerData.gameTimeSecondsArray.Length; i++)
        //     {
        //         AnalyticsHelper.SetUserProperty($"gametime_seconds_d{i}", playerData.gameTimeSecondsArray[i]);
        //     }
        // }
        //
        // AnalyticsHelper.UploadUserProperty();
    }

    public void OnMoneyChange(E_ItemType moneyType, float moneyValue, bool isAd, string adPos)
    {
        // Bizza.Analytics.Manager.AddCommonParams();
        // Bizza.Analytics.Manager.AddParam("money_type", moneyType.ToString());
        // Bizza.Analytics.Manager.AddParam("money_change_num", moneyValue);
        // Bizza.Analytics.Manager.AddParam("money_num_chang_after", ItemUtils.GetItemCount(moneyType));
        // Bizza.Analytics.Manager.AddParam("is_ad", isAd);
        // Bizza.Analytics.Manager.AddParam("money_scene", adPos);
        // Bizza.Analytics.Manager.SendCustomEvent("c_change_money_num");
    }

    public void BuyItem(ItemEntry costItem, ItemEntry getItem)
    {
        // Bizza.Analytics.Manager.AddCommonParams();
        // Bizza.Analytics.Manager.AddParam("cost_item_detail", new Dictionary<string, object>()
        // {
        //     {"item_name", costItem.Type.ToString()},
        //     {"item_num", costItem.Count},
        // });
        // Bizza.Analytics.Manager.AddParam("get_item_detail", new Dictionary<string, object>()
        // {
        //     {"item_name", getItem.Type.ToString()},
        //     {"item_num", getItem.Count},
        // });
        // Bizza.Analytics.Manager.SendCustomEvent("c_buy_items");
    }

    // private Dictionary<E_ItemType, int> _propUseTimes = new();
    public void UseItem(E_ItemType itemType, int levelId, int useTimes)
    {
        // Bizza.Analytics.Manager.AddCommonParams();
        // AddLevelCommonParams(levelId);
        // Bizza.Analytics.Manager.AddParam("item_id", itemType.ToString());
        // Bizza.Analytics.Manager.AddParam("total_use_count", useTimes);
        // Bizza.Analytics.Manager.SendCustomEvent("c_use_items");
    }

    private void AddLevelCommonParams(int levelId)
    {
        // Bizza.Analytics.Manager.AddParam("round_id", SaveDataUtils.GameData.totalLevelTimes);
        // Bizza.Analytics.Manager.AddParam("round_level", levelId);
        // Bizza.Analytics.Manager.AddParam("is_first", SaveDataUtils.GameData.levelStatisticData[levelId].levelPlayTimes <= 1);
    }
}
