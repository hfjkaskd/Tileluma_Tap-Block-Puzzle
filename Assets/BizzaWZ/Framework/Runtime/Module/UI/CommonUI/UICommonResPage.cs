// using Bizza.Analytics;
// using cfg;
// using TMPro;
// using UnityEngine;
// using UnityEngine.UI;
//
// public class UICommonResPage : UIPageBase<string, string, ResSave>
// {
//     public override UIPageIds PageType => UIPageIds.UI_CommonResPage;
//     public TMP_Text TitleTxt;
//     public TMP_Text DescTxt;
//     public CmptRes Res;
//     public UITweenButton AdsGetResBtn;
//
//     private ResSave m_ResSave;
//     private int m_ResId;
//
//     [Obfuz.ObfuzIgnore]
//     protected override void OnClose()
//     {
//     }
//
//     protected override void OnHide()
//     {
//     }
//
//     protected override void OnOpen(string title, string desc, ResSave resSave)
//     {
//         AdsGetResBtn.onClick.RemoveAllListeners();
//         AdsGetResBtn.onClick.AddListener(OnAdsGetResBtn);
//
//         TitleTxt.text = title;
//         DescTxt.text = desc;
//         m_ResSave = resSave;
//         Res.Init(new ResInfo()
//         {
//             type = resSave.type,
//             id = resSave.id,
//             count = resSave.count
//         });
//     }
//
//     protected override void OnShow()
//     {
//     }
//
//     private void OnAdsGetResBtn()
//     {
// #if Bizza_Platform_Monkeyfly
//         AdsUtils.OpenAds($"ads_get_res_{m_ResId}", adsID, OnSeeAdsCompleted);
// #else
//         AdsUtils.OpenAds($"ads_get_res_{m_ResId}", "", OnSeeAdsCompleted, "");
// #endif
//     }
//
//     private void OnSeeAdsCompleted(bool isSuccess)
//     {
//         if (!isSuccess)
//             return;
//         Bizza.Analytics.Manager.SendShiFengVideoEvent("strength");
//         var info = SaveDataModule.GameSave.AddRes(m_ResSave);
//         _ = UIModule.Instance.OpenPage(UIPageIds.UI_GetRes, new UI_GetResPage.InitParam { resList = info });
//         SaveDataModule.Instance.gameStrategy.SaveData();
//
//         var config = Tables.Instance.TblCurrency.GetById(m_ResSave.id);
//         BizzaEventSystem.Emit(E_CustomEvent.AdsGetResource, config.Type);
//
//         CloseSelf();
//     }
// }
