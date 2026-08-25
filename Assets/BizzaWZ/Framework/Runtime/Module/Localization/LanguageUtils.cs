using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Bizza.Sdk;
using UnityEngine;
using UnityEngine.Networking;

public class LanguageUtils
{
// #if UNITY_EDITOR
//     [UnityEditor.MenuItem("Tools/选择语言（需要按ctrl+w) %w")]
//     public static void ChangeLanguage_ZH()
//     {
//         var cur = UnityEditor.EditorPrefs.GetInt("Editor_GameLanguage", 0);
//         UnityEditor.GenericMenu menu = new UnityEditor.GenericMenu();
//         foreach (var v in languageMap)
//         {
//             bool select = false;
//             if (cur > 0 && languageMap.TryGetValue((SystemLanguage)cur, out var val))
//             {
//                 if (val == v.Value)
//                 {
//                     select = true;
//                 }
//             }
//             menu.AddItem(new GUIContent(v.Value), select, OnSelectLanguage, v.Key);
//         }
//         menu.ShowAsContext();
//     }
//
//     [UnityEditor.MenuItem("Tools/清除选中语言")]
//     public static void ClearLanguage()
//     {
//         UnityEditor.EditorPrefs.DeleteKey("Editor_GameLanguage");
//     }

//     private static void OnSelectLanguage(object o)
//     {
//         var code = (int)o;
//         UnityEditor.EditorPrefs.SetInt("Editor_GameLanguage", code);
//         Debug.Log(code);
//     }
// #endif


    public static string SelectedLanguage
    {
        //先默认英语
        get => PlayerPrefs.GetString(nameof(SelectedLanguage), ChannelConfig.Instance.defaultLanguage);
        set
        {
            PlayerPrefs.SetString(nameof(SelectedLanguage), value);
            PlayerPrefs.Save();
            BizzaEventSystem.Emit(EventDefine.Frame.LanguageChange);
        }
    }

    public static bool HasReadLanguage => PlayerPrefs.HasKey(nameof(SelectedLanguage));

    public static string GetStandardLanguageCode()
    {
        // 1. 获取Unity识别的系统语言
        SystemLanguage unityLang = Application.systemLanguage;

        // 2. 获取系统默认的地区文化信息（包含地区代码）
        CultureInfo systemCulture = CultureInfo.CurrentCulture;
        string regionCode = systemCulture.Name.Split('-')[1]; // 提取地区代码（如US、CN）

        // 3. 映射为标准BCP 47格式（语言代码-地区代码）
        return unityLang switch
        {
            SystemLanguage.Chinese => "zh-CN", // 简体中文（默认中国地区）
            SystemLanguage.ChineseSimplified => "zh-CN",
            SystemLanguage.ChineseTraditional => "zh-CN", // 繁体中文（默认台湾地区）
            SystemLanguage.English => $"en-{regionCode}",
            SystemLanguage.Portuguese => $"pt-{regionCode}", // 葡萄牙语（如pt-PT、pt-BR）
            SystemLanguage.Indonesian => "id-ID",
            SystemLanguage.Russian => "ru-RU",
            SystemLanguage.Japanese => "ja-JP",
            SystemLanguage.Korean => "ko-KR",
            SystemLanguage.Spanish => $"es-{regionCode}", // 西班牙语（如es-ES、es-MX）
            SystemLanguage.Turkish => "tr-TR",
            SystemLanguage.Vietnamese => "vi-VN",


            // SystemLanguage.German => $"de-{regionCode}",
            // SystemLanguage.French => $"fr-{regionCode}",
            // SystemLanguage.Italian => "it-IT",
            _ => "en-US" // 默认返回英语（美国）
        };
    }

    // public static string GetFormatNum(int num)
    // {
    //     return system.GetFormatNum(num);
    // }

    public static string GetFormatText(string key, object arg1)
    {
        return string.Format(GetText(key), arg1);
    }

    public static string GetFormatText(string key, object arg1, object arg2)
    {
        return string.Format(GetText(key), arg1, arg2);
    }

    public static string GetFormatText(string key, object arg1, object arg2, object arg3)
    {
        return string.Format(GetText(key), arg1, arg2, arg3);
    }

    // public static string GetFormatText<T1>(string key, T1 arg1)
    // {
    //     return string.Format(GetText(key), arg1.ToString());
    // }
    //
    // public static string GetFormatText<T1, T2>(string key, T1 arg1, T2 arg2)
    // {
    //     return string.Format(GetText(key), arg1.ToString(), arg2.ToString());
    // }
    //
    // public static string GetFormatText<T1, T2, T3>(string key, T1 arg1, T2 arg2, T3 arg3)
    // {
    //     return string.Format(GetText(key), arg1.ToString(), arg2.ToString(), arg3.ToString());
    // }


    /// <summary>
    /// 如果没有存储语言，则通过 IP 自动识别
    /// </summary>
    public static IEnumerator InitLanguageFromIP()
    {
        if (!string.IsNullOrEmpty(SelectedLanguage))
        {
            Debug.Log("[Language] 使用存储语言: " + SelectedLanguage);
            yield break;
        }

        Debug.Log("[Language] 开始通过 IP 自动检测语言...");

        string countryCode = "US"; // 默认美国
        bool success = false;

        // 👉 多个稳定国外 API（不依赖中国网络）
        string[] urls =
        {
            "https://ipapi.co/json/",     // 最稳定，日本/韩国/Wifi 也稳定
            "https://api.ip.sb/geoip",    // Cloudflare，全球非常快
            "https://ipinfo.io/json",     // 备用
        };

        foreach (string url in urls)
        {
            Debug.Log($"[Language] 尝试 API: {url}");

            UnityWebRequest req = UnityWebRequest.Get(url);
            req.timeout = 5;

            yield return req.SendWebRequest();

            Debug.Log($"[Language] Result = {req.result}, ResponseCode = {req.responseCode}");

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning("[Language] 请求失败: " + req.error);
                continue;
            }

            string json = req.downloadHandler.text;
            Debug.Log("[Language] 返回 JSON: " + json);

            try
            {
                // ========== 1. ipapi ======================
                if (url.Contains("ipapi.co"))
                {
                    var data = JsonUtility.FromJson<IPApiCoData>(json);
                    if (!string.IsNullOrEmpty(data.country))
                    {
                        countryCode = data.country;
                        Debug.Log("[Language] 成功解析 (ipapi.co): " + countryCode);
                        success = true;
                        break;
                    }
                }

                // ========== 2. ip.sb =======================
                if (url.Contains("ip.sb"))
                {
                    var data = JsonUtility.FromJson<IPSbData>(json);
                    if (!string.IsNullOrEmpty(data.country_code))
                    {
                        countryCode = data.country_code;
                        Debug.Log("[Language] 成功解析 (ip.sb): " + countryCode);
                        success = true;
                        break;
                    }
                }

                // ========== 3. ipinfo.io ===================
                if (url.Contains("ipinfo.io"))
                {
                    var data = JsonUtility.FromJson<IPInfoData>(json);
                    if (!string.IsNullOrEmpty(data.country))
                    {
                        countryCode = data.country;
                        Debug.Log("[Language] 成功解析 (ipinfo.io): " + countryCode);
                        success = true;
                        break;
                    }
                }

                Debug.LogWarning("[Language] JSON 解析成功但找不到国家码");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("[Language] JSON 解析异常: " + e.Message);
            }
        }

        if (!success)
        {
            Debug.LogWarning("[Language] 所有 API 均失败，国家码设置为 US");
        }

        Debug.Log("[Language] 最终国家码: " + countryCode);

        SelectedLanguage = MapCountryToLanguage(countryCode);
        Debug.Log("[Language] 自动设置语言: " + SelectedLanguage);
    }


    /// <summary>
    /// 国家码映射到语言
    /// </summary>
    private static string MapCountryToLanguage(string country)
    {
        switch (country)
        {
            case "BR": return "pt-BR";
            case "ID": return "id-ID";
            case "RU": return "ru-RU";
            case "JP": return "ja-JP";
            case "KR": return "ko-KR";

            case "ES":
            case "MX":
            case "AR":
            case "CO":
            case "CL":
            case "PE":
            case "VE":
                return "es-ES";

            case "TR": return "tr-TR";
            case "VN": return "vi-VN";

            case "US":
            case "GB":
            case "CA":
            case "AU":
            case "NZ":
            case "PH":
                return "en-US";

            default:
                return "en-US";
        }
    }

    // ===================== 各 API 的数据结构 ========================

    [System.Serializable]
    private class IPApiCoData   // https://ipapi.co/json
    {
        public string country; // CN / JP / KR ...
    }

    [System.Serializable]
    private class IPSbData      // https://api.ip.sb/geoip
    {
        public string country_code;
    }

    [System.Serializable]
    private class IPInfoData    // https://ipinfo.io/json
    {
        public string country; // CN / JP / KR ...
    }

    // ======================= 语言表读取 ==========================
    public static string GetText(string key)
    {
        var table = TableUtils.Tables.TblLanguage;
        if (table == null || table.DataMap == null)
        {
            #if UNITY_EDITOR
            LogLogger.LogInfo($"GetText failed table is null:{key}");
            #endif
            return key;
        }
        var dict = table.GetOrDefault(key);
        if (dict == null) return key;
        var curLanguage = SelectedLanguage;
        return dict.Dict.GetValueOrDefault(curLanguage, key);
    }

    public static string GetFormatText(string key, params object[] args)
    {
        return string.Format(GetText(key), args);
    }
}