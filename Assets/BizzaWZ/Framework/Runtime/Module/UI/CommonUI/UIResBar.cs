// using System;
// using Bizza;
// using Bizza.Unity;
// using cfg;
// using TMPro;
// using UnityEngine;
// using UnityEngine.UI;
//
// public class UIResBar : MonoBehaviour
// {
//     public TMP_Text coinTxt;
//     public E_Currency CurrencyType = E_Currency.Coin;
//
//     private int m_CurrencyId;
//
//
//     private void Awake()
//     {
//         BizzaEventSystem.On(E_CustomEvent.ItemUpdate, OnItemUpdate);
//     }
//
//
//     private void OnEnable()
//     {
//         var config = Tables.Instance.TblCurrency.GetByType(CurrencyType);
//         var count = SaveDataModule.GameSave.GetCurrency(config.Id, 0).count;
//         m_CurrencyId = config.Id;
//         OnItemUpdate(new ResSave()
//         {
//             type = E_ResType.Currency,
//             id = config.Id,
//             lv = 0,
//             count = count
//         });
//     }
//
//     private void OnDestroy()
//     {
//         BizzaEventSystem.Off(E_CustomEvent.ItemUpdate, OnItemUpdate);
//     }
//
//     private void OnItemUpdate(ResSave resSave)
//     {
//         if (m_CurrencyId != resSave.id)
//             return;
//
//         var count = SaveDataModule.GameSave.GetCurrency(m_CurrencyId, 0).count;
//         string resNum = LanguageUtil.GetFormatNum(count);
//
//         if (CurrencyType == E_Currency.Power)
//         {
//             resNum += "/" + 30; // TODO 全局表
//         }
//         coinTxt.text = resNum;
//     }
// }
