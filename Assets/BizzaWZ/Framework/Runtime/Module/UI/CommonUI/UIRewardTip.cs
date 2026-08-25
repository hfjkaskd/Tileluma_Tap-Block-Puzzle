// using System.Collections;
// using System.Collections.Generic;
// using Bizza.Unity;
// using BizzaCommon;
// using cfg;
// using UnityEngine;
// using UnityEngine.EventSystems;
//
// public class UIRewardTip : UIPageBase<List<ResSave>, RectTransform>
// {
//     public override UIPageIds PageType => UIPageIds.UI_RewardTip;
//     public CmptRes Prefab_Res;
//     public Transform Parent;
//     public RectTransform PinPoint;
//
//     private List<CmptRes> m_ResList = new();
//     private List<ResSave> m_ResSaveList = new();
//     private void OnEnable()
//     {
//         BizzaEventSystem.On(E_CustomEvent.ChangeGameMode, ChangeGameMode);
//     }
//     private void ChangeGameMode(EGameModeType eGameModeType) {
//         if (this == null) return;
//         this.CloseSelf();
//     }
//     private void OnDisable()
//     {
//         if (m_ResList.Count > 0)
//         {
//             for (int i = 0; i < m_ResList.Count; i++)
//             {
//                 GameObjectPoolUtility.ReleaseToPool(m_ResList[i]);
//             }
//             m_ResList.Clear();
//         }
//         BizzaEventSystem.On(E_CustomEvent.ChangeGameMode, ChangeGameMode);
//     }
//
//     private void Update()
//     {
//         if (Input.GetMouseButtonDown(0) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended))
//         {
//             if (EventSystem.current.currentSelectedGameObject != gameObject)
//             {
//                 if (this == null) return;
//                 this.CloseSelf();
//             }
//         }
//     }
//
//     protected override void OnClose()
//     {
//     }
//
//     protected override void OnHide()
//     {
//     }
//
//     protected override void OnOpen(List<ResSave> resSaves, RectTransform trans)
//     {
//         m_ResSaveList = resSaves;
//         ShowRewardTip();
//
//         transform.position = trans.transform.position;
//         (transform as RectTransform).anchoredPosition += new Vector2(0, trans.sizeDelta.y / 2);
//         (transform as RectTransform).anchoredPosition += -PinPoint.anchoredPosition;
//         UIUtil.ClampInScreen(transform as RectTransform);
//         PinPoint.position = new Vector3(trans.position.x, PinPoint.position.y, PinPoint.position.z);
//     }
//     protected override void OnShow()
//     {
//     }
//
//     private void ShowRewardTip()
//     {
//         if (m_ResList.Count > 0)
//         {
//             for (int i = 0; i < m_ResList.Count; i++)
//             {
//                 GameObjectPoolUtility.ReleaseToPool(m_ResList[i]);
//             }
//             m_ResList.Clear();
//         }
//
//         if (m_ResSaveList.Count < 0)
//             return;
//
//         for (int i = 0; i < m_ResSaveList.Count; i++)
//         {
//             var ResIns = GameObjectPoolUtility.GetFromPool(Prefab_Res);
//             ResIns.transform.SetParent(Parent);
//             ResIns.transform.localScale = Vector3.one * 0.6f;
//             ResIns.Init(m_ResSaveList[i]);
//             ResIns.gameObject.SetObjActive(true);
//
//             m_ResList.Add(ResIns);
//         }
//     }
//
//
// }
