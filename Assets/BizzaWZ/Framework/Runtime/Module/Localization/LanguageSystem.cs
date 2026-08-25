// using System;
// using System.Collections.Generic;
// using System.Globalization;
// using UnityEngine;
// using cfg;
//
// public abstract class LanguageSystem
// {
//     protected Dictionary<string, LanguageConfig> dictionary;
//     public LanguageSystem()
//     {
//         dictionary = Tables.Instance.TblLanguage.DataMap;
//     }
//     public abstract string GetText(string key);
//
//     #region Format Number
//     protected List<int> numbers;
//     protected List<string> texts;
//     public virtual string GetFormatNum(float num)
//     {
//         float abs = Mathf.Abs(num);
//         for (int i = numbers.Count - 1; i > -1; i--)
//         {
//             if (abs >= numbers[i])
//             {
//                 float final = num / numbers[i];
//                 final = Mathf.Floor(final * 10) / 10;
//                 return final + texts[i];
//             }
//         }
//         return Mathf.Floor(num).ToString(CultureInfo.InvariantCulture);
//     }
//     #endregion
//
//     public abstract void OnRelease();
// }
//
// public class DefaultLanguageSystem : LanguageSystem
// {
//     public DefaultLanguageSystem()
//     {
//         numbers = new List<int>()
//         {
//             1, 1000, 1000^2, 1000^3,
//         };
//         texts = new List<string>()
//         {
//             "", "K", "M", "B",
//         };
//     }
//
//     public override string GetText(string key)
//     {
//         return key;
//     }
//
//     public override void OnRelease()
//     {
//
//     }
// }
//
// public class LanguageSystemZHCN : LanguageSystem
// {
//     public LanguageSystemZHCN() : base()
//     {
//         var config = cfg.Tables.Instance.TblLangNum.GetOrDefault("ZH_CN");
//         numbers = config.NumList;
//         texts = config.TextList;
//     }
//
//     public override string GetText(string key)
//     {
//         if (dictionary.TryGetValue(key, out var data))
//         {
//             return data.ZHCN;
//         }
//         return key;
//     }
//
//     public override void OnRelease()
//     {
//     }
// }
//
// public class LanguageSystemZHHK : LanguageSystem
// {
//     public LanguageSystemZHHK() : base()
//     {
//         var config = cfg.Tables.Instance.TblLangNum.GetOrDefault("ZH_HK");
//         numbers = config.NumList;
//         texts = config.TextList;
//     }
//
//     public override string GetText(string key)
//     {
//         if (dictionary.TryGetValue(key, out var data))
//         {
//             return data.ZHHK;
//         }
//         return key;
//     }
//
//     public override void OnRelease()
//     {
//     }
// }
//
// public class LanguageSystemEN : LanguageSystem
// {
//     public LanguageSystemEN() : base()
//     {
//         var config = cfg.Tables.Instance.TblLangNum.GetOrDefault("EN");
//         numbers = config.NumList;
//         texts = config.TextList;
//     }
//
//     public override string GetText(string key)
//     {
//         if (dictionary.TryGetValue(key, out var data))
//         {
//             return data.EN;
//         }
//         return key;
//     }
//
//     public override void OnRelease()
//     {
//     }
// }
