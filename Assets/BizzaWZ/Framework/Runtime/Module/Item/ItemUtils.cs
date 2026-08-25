using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[Obfuz.ObfuzIgnore]
public enum E_AddItemSource
{
    None,
    Match,
    MatchAd,
    DailyTask,
    Settlement,
    Flow,
    Dan, // 段位任务
    UsSlot,
}
[Obfuz.ObfuzIgnore]
public struct AddItemParam
{
    public bool playAnim;
    public bool isAd;
    public Vector3 startPos;
    public bool bUiPos;
    public bool showCurrencyBar;
    public E_AddItemSource source;
    public Transform target;

    public Action addFinishAction;
}


[Serializable]
public partial struct ItemEntry
{
    public static ItemEntry None = new ItemEntry() { Type = E_ItemType.None, Count = 0f };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ItemEntry operator *(ItemEntry item, float rate) => new ItemEntry()
    {
        Type = item.Type,
        Count = item.Count * rate,
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ItemEntry operator +(ItemEntry item, float num) => new ItemEntry()
    {
        Type = item.Type,
        Count = item.Count + num,
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ItemEntry operator +(ItemEntry item1, ItemEntry item2)
    {
        if (item1.Type != item2.Type) throw new InvalidOperationException();

        return new ItemEntry()
        {
            Type = item1.Type,
            Count = item1.Count + item2.Count,
        };
    }
}

[Obfuz.ObfuzIgnore]

public class ItemUtils
{
    private static List<RewardConfig> _cachedList = new();

    // public static void AddItem(string itemType, float amount)
    // {
    //     PlayerDataManager.Instance.ChangeItemCount(itemType, amount);
    // }

    //public static void ShowVipCardPanel(E_AdPos adPos)
    //{
    //    ItemUtils.ShowGetItemPanel(ItemUtils.GetReward(TableUtils.Global.ChestReward), adPos);
    //}

    public static ItemEntry ToCorrect(ItemEntry item)
    {
        return item;
        var config = TableUtils.Tables.TblItem.GetOrDefault(item.Type);
        item.Count *= Random.Range(TableUtils.Global.DollarRange.X, TableUtils.Global.DollarRange.Y);
        if (config != null)
        {
            var factor = 1f;
            for (int i = config.NumRange2.Count - 1; i >= 0; i--)
            {
                if (GetOriginItemCount(item.Type) >= config.NumRange2[i])
                {
                    factor = config.MultFactors[i];
                    break;
                }
            }

            item.Count *= factor;
        }

        if (item.Type == E_ItemType.Dollar)
        {
            var mult = TableUtils.Global.MultDollarPoss > Random.Range(0, 100);
            if (!SaveDataUtils.GameData.firstTimeMult)
            {
                mult = true;
                SaveDataUtils.GameData.firstTimeMult = true;
                SaveDataUtils.gameStrategy.SaveData();
            }
            if (mult)
            {
                var factor = Random.Range(TableUtils.Global.MultDollarRange.X, TableUtils.Global.MultDollarRange.Y);
                item.Count *= factor;
            }
        }

        if (item.Type == E_ItemType.Dollar)
        {
            item.Count = ((int)(item.Count * 100)) / 100f;
        }

        return item;
    }

    private static Dictionary<string, float> _langugeCurrencyRates = null;
    public static string FormatCount(ItemEntry item)
    {
#if BIZZA_REAL_WITHDRAW
        if (item.Type == E_ItemType.Dollar || item.Type == E_ItemType.Gold)
        {
            string info = LanguageUtils.GetText("CurrencyToken") + WithdrawalUtil.GetCustomizedValueByCountryType(item.Count);
            return info;
        }
        else
        {
            string info = $"{item.Count}";
            return info;
        }

#endif
        var num = item.Count;
        //Debug.LogError("num." + num + item.Count);
        if (item.Type == E_ItemType.Dollar || item.Type == E_ItemType.WithDrawDanDollar)
        {
            //计算汇率
            //Debug.LogError("计算汇率.");
            if (_langugeCurrencyRates == null)
            {
                _langugeCurrencyRates = new Dictionary<string, float>();
                foreach (var v in TableUtils.Global.LanguageInfos)
                {
                    _langugeCurrencyRates[v.Key] = v.FloatVal;
                }
            }

            if (!_langugeCurrencyRates.TryGetValue(LanguageUtils.SelectedLanguage, out var rate))
            {
                rate = 1;
            }

            num *= rate;
            // num = GameUtils.FormatDollar(num);
            return LanguageUtils.GetText("CurrencyToken") + GetFormatDollar(num);
        }
        else if (item.Type == E_ItemType.Gold)
        {
            //Debug.LogError("item.Count" + item.Count);
            return item.Count.ToString();
        }
        else
        {
            return Mathf.RoundToInt(item.Count).ToString();
        }

    }

    public static Sprite GetSprite(string path)
    {
        var asset = AssetUtils.LoadAssetSync<Sprite>(path);
        return asset;
    }
    public static float FormatCountFloat(ItemEntry item)
    {
        var num = item.Count;
        if (item.Type == E_ItemType.Dollar || item.Type == E_ItemType.WithDrawDanDollar)
        {
            //计算汇率
            if (_langugeCurrencyRates == null)
            {
                _langugeCurrencyRates = new Dictionary<string, float>();
                foreach (var v in TableUtils.Global.LanguageInfos)
                {
                    _langugeCurrencyRates[v.Key] = v.FloatVal;
                }
            }

            if (!_langugeCurrencyRates.TryGetValue(LanguageUtils.SelectedLanguage, out var rate))
            {
                rate = 1;
            }

            num *= rate;
            // num = GameUtils.FormatDollar(num);
            return num;
        }
        else
        {
            return item.Count;
        }
    }

    static string[] suffixes = { "", "K", "M", "B", "T" };
    public static string GetFormatDollar(float dollar)
    {
        int suffixIndex = 0;
        double value = Math.Abs(dollar);

        // 只有当数值大于等于10000时才使用缩写
        while (value >= 10000 && suffixIndex < suffixes.Length - 1)
        {
            value /= 1000;
            suffixIndex++;
        }

        // 如果不需要缩写（suffixIndex为0）
        if (suffixIndex == 0)
        {
            // 对于整数部分为0的小数，保留2位小数
            if (Math.Abs(value) < 1 && Math.Abs(value) > 0)
            {
                return value.ToString("0.##"); // 最多2位小数
            }
            // 对于1-9999的数值，如果是整数就显示整数，有小数就保留最多2位小数
            else if (Math.Abs(value) < 10000)
            {
                // 检查是否为整数
                if (Math.Abs(value - Math.Round(value)) < 0.001)
                {
                    return ((int)Math.Round(value)).ToString();
                }
                else
                {
                    double v = Math.Round(value, 3, MidpointRounding.AwayFromZero);

                    double abs = Math.Abs(v);

                    if (abs >= 1000)
                    {
                        return v.ToString("0");
                    }
                    else if (abs >= 100)
                    {
                        return v.ToString("0.#");
                    }
                    else if (abs >= 10)
                    {
                        return v.ToString("0.##");
                    }
                    else
                    {
                        return v.ToString("0.###");
                    }
                }
            }
        }

        // 需要缩写的数值格式化（10000及以上）
        string format;
        string result;

        // 根据数值大小选择格式
        if (value >= 100 || value <= -100)
        {
            // 三位数或以上，不需要小数
            format = "0";
            result = value.ToString(format) + suffixes[suffixIndex];
        }
        else if (value >= 10 || value <= -10)
        {
            // 两位数，最多保留1位小数
            format = "0.#";
            result = value.ToString(format) + suffixes[suffixIndex];
        }
        else
        {
            // 一位数，最多保留2位小数
            format = "0.##";
            result = value.ToString(format) + suffixes[suffixIndex];
        }

        // 确保结果不超过5个字符
        if (result.Length > 5)
        {
            // 如果超过5个字符，尝试减少小数位数
            if (format == "0.##")
            {
                result = value.ToString("0.#") + suffixes[suffixIndex];
            }
            else if (format == "0.#")
            {
                result = value.ToString("0") + suffixes[suffixIndex];
            }

            // 如果仍然超过5个字符，强制截断
            if (result.Length > 5)
            {
                result = result.Substring(0, 5);
            }
        }
        return result;
    }


    //
    // public static ItemEntry ToCorrectItem(ItemEntry item)
    // {
    //     if (item.Type == E_ItemType.Dollar)
    //     {
    //         item.Count = ((int)(item.Count * 100)) / 100f;
    //     }
    //
    //     return item;
    // }

    public static ItemEntry Get(E_ItemType itemType)
    {
        if (!SaveDataUtils.ItemData.itemMap.TryGetValue(itemType, out var item))
        {
            item = new ItemEntry();
            item.Type = itemType;
        }

        return item;
    }

    public static string GetItemText(E_ItemType itemType)
    {
        return GetItemText(Get(itemType));
    }

    public static string GetItemText(ItemEntry item)
    {
        if (item.Type == E_ItemType.Dollar)
        {
            return ItemUtils.FormatCount(item);
        }
        else
        {
            return ItemUtils.FormatCount(item);
        }
    }

    public static Sprite GetItemIcon(ItemEntry item)
    {
        return GetItemIcon(item.Type);
    }

    public static Sprite GetItemFragIcon(ItemEntry item)
    {
        return null;
        // return GetItemIcon(item.Type);
        // return BingoConfig.Instance.FragmentSprite[item.Type - E_ItemType.Fragment_0];
    }

    public static Sprite GetItemIcon(E_ItemType itemType)
    {
        var itemConfig = TableUtils.Tables.TblItem.GetOrDefault(itemType);
        if (itemConfig == null)
        {
            return null;
        }
        return AssetUtils.LoadAssetSync<Sprite>(itemConfig.Icon);
        // return null;
        // return ImageHelper.Instance.itemRewardIm[(int)itemType];
    }

    public static void AddItem(ItemEntry inItem, AddItemParam inParam)
    {
#if BIZZA_REAL_WITHDRAW
        bool isSingleMode = Bizza.Sdk.ChannelConfig.Instance.real_CustomConfig.singleCurrencyMode;
           
        if (isSingleMode && (inItem.Type == E_ItemType.Dollar || inItem.Type == E_ItemType.WithDrawDanDollar))
        {
            return;
        }
#endif

        if (!inParam.playAnim)
        {
            // PlayerDataManager.Instance.ChangeItemCount(itemType, amount);
            SaveDataUtils.ItemData.itemMap.TryAdd(inItem.Type, new ItemEntry() { Type = inItem.Type });

            var item = SaveDataUtils.ItemData.itemMap[inItem.Type];
            var prevItem = item;
            item.Count += inItem.Count;
            item.Count = Mathf.Max(0, item.Count);
            SaveDataUtils.ItemData.itemMap[inItem.Type] = item;

            BizzaEventSystem.Emit(EventDefine.Item.ItemChangedWithData, prevItem, item);

            if (inParam.source != E_AddItemSource.None)
            {
                StatisticModule.Instance.OnMoneyChange(inItem.Type, inItem.Count, inParam.isAd, inParam.source.ToString());
            }

            SaveDataUtils.itemStrategy.SaveData();
        }
        else
        {
            if (inParam.showCurrencyBar)
            {
                OnMoneyFlyStart();
            }

            VFXUtils.PlayItemCollectFx(inItem.Type, inParam.startPos, () =>
            {
                if (inParam.showCurrencyBar)
                {
                    OnMoneyFlyFinish();
                }
                inParam.playAnim = false;
                AddItem(inItem, inParam);
                if (inItem.Type == E_ItemType.Gold)
                {
                    SoundManager.Instance.PlaySFX("SFX_GetGold");
                }
                inParam.addFinishAction?.Invoke();
            }, bUiPos: inParam.bUiPos, target: inParam.target);
        }

        BizzaEventSystem.Emit(EventDefine.Item.ItemChanged);
    }

    public static void AddItem(ItemEntry item, bool playAnim = false, bool isAd = false, Vector3 startPos = default, bool showCurrencyBar = false, float rate = 1.0f)
    {
        AddItem(item.Type, item.Count * rate, playAnim, isAd, startPos, showCurrencyBar);
    }

    public static void AddItem(E_ItemType itemType, float amount, bool playAnim = false, bool isAd = false, Vector3 startPos = default, bool bUiPos = true, bool showCurrencyBar = false)
    {
        AddItem(new ItemEntry()
        {
            Type = itemType,
            Count = amount,
        }, new AddItemParam()
        {
            playAnim = playAnim,
            isAd = isAd,
            bUiPos = bUiPos,
            startPos = startPos,
            showCurrencyBar = showCurrencyBar,
        });
    }

    private static int _showCurrencyBar;
    private static void OnMoneyFlyStart()
    {
        _showCurrencyBar++;
        if (_showCurrencyBar > 0)
        {
            BizzaEventSystem.Emit(EventDefine.Frame.ShowCurrencyBar, true);
        }
        // UiManager.Instance.GetPanel<PlayerFramePanel>(UIPanelType.PlayerFramePanel).GetComponent<Canvas>()
        // .overrideSorting = _showCurrencyBar > 0;
    }

    private static void OnMoneyFlyFinish()
    {
        _showCurrencyBar--;
        if (_showCurrencyBar <= 0)
        {
            GameUtils.DelayDo(() =>
            {
                BizzaEventSystem.Emit(EventDefine.Frame.ShowCurrencyBar, false);
            }, 1);
        }
    }

    public static bool TryReduceItem(E_ItemType itemType, int count = 1)
    {
        if (GetItemCount(itemType) >= count)
        {
            AddItem(itemType, -count);
            return true;
        }

        return false;
    }
    public static bool TryReduceItemFloat(E_ItemType itemType, float count = 1)
    {
        if (GetItemCount(itemType) >= count)
        {
            AddItem(itemType, -count);
            return true;
        }

        return false;
    }
    public static float GetOriginItemCount(E_ItemType itemType)
    {
        return Get(itemType).Count;
    }

    public static float GetItemCount(E_ItemType itemType)
    {
        return Get(itemType).Count;
    }

    public static int GetItemCountInt(E_ItemType itemType)
    {
        return Mathf.RoundToInt(GetItemCount(itemType));
    }
    //
    // private static Dictionary<E_ItemType, RemoteItemConfig> _remoteConfigItems;
    public static int GetItemWeight(RewardConfig config)
    {
        return 1;
        // if (_remoteConfigItems == null)
        // {
        //     _remoteConfigItems = new();
        //     foreach (var v in RemoteConfigModule.remoteConfigData.items)
        //     {
        //         _remoteConfigItems[v.id] = v;
        //     }
        // }
        //
        // var weight = config.Weight * 100;
        // var itemConfig = TableUtils.Tables.TblItem.GetOrDefault(config.ItemType);
        // if (_remoteConfigItems.TryGetValue(config.ItemType, out var remoteItemConfig) && remoteItemConfig.range != null && remoteItemConfig.range.Count > 0)
        // {
        //     var curNum = ItemUtils.GetItemCount(config.ItemType);
        //     var factor = 1f;
        //     for (var i = 0; i < remoteItemConfig.range.Count; i++)
        //     {
        //         var v = remoteItemConfig.range[i];
        //         if (curNum > v)
        //         {
        //             factor = remoteItemConfig.factor[i];
        //         }
        //     }
        //
        //     weight = Mathf.RoundToInt(weight * factor);
        // }
        // else if (itemConfig.NumRange.Count > 0)
        // {
        //     var curNum = ItemUtils.GetItemCount(config.ItemType);
        //     var factor = 1f;
        //     for (var i = 0; i < itemConfig.NumRange.Count; i++)
        //     {
        //         var v = itemConfig.NumRange[i];
        //         if (curNum > v)
        //         {
        //             factor = itemConfig.WeightFactors[i];
        //         }
        //     }
        //
        //     weight = Mathf.RoundToInt(weight * factor);
        // }
        //
        // return weight;
    }

    public static ItemEntry GetReward(int group, E_ItemType typeFilter = E_ItemType.None)
    {
        if (RemoteConfigModule.reviewMode)
        {
            return new ItemEntry()
            {
                Type = E_ItemType.Dollar,
                Count = Random.Range(2f, 10f),
            };
        }

        _cachedList.Clear();
        foreach (var v in TableUtils.Tables.TblReward.DataList)
        {
            if (v == null)
            {
                continue;
            }

            if (typeFilter != E_ItemType.None && v.ItemType != typeFilter)
            {
                continue;
            }
            if (v.GroupId == group)
            {
                _cachedList.Add(v);
            }
        }

        var randomReward = _cachedList.RandomByWeight(GetItemWeight);

        var ret = new ItemEntry();
        ret.Type = randomReward.ItemType;
        ret.Count = randomReward.ItemNum;

        //衰减
        {
            if (ret.Type == E_ItemType.Dollar)
            {
                // ret.Count = InfoHelper.cDollar(ret.Count);
                //TODO:
            }
        }

        return ret;
    }

    public static List<ItemEntry> GetRewards(List<int> groups)
    {
        var list = new List<ItemEntry>();
        foreach (var v in groups)
        {
            list.Add(GetReward(v));
        }

        return list;
    }

    public static void AddItem(ItemEntry a, ItemEntry b)
    {

    }
}
