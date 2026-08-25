// using System.Collections;
// using System.Collections.Generic;
// using System.ComponentModel;
// using UnityEngine;
//
// public partial class SROptions
// {
//     private const string UICategory = "UI";
//
//     [Category(UICategory)]
//     [DisplayName("签到界面")]
//     public void OpenPage_SignIn()
//     {
//         UiManager.Instance.ShowPanel(UIPanelType.SignInPanel);
//     }
//
//     [Category(UICategory)]
//     [DisplayName("任务界面")]
//     public void OpenPage_Task()
//     {
//         UiManager.Instance.ShowPanel(UIPanelType.TaskPanel);
//     }
//
//
//     [Category(UICategory)]
//     [DisplayName("幸运转盘")]
//     public void OpenPage_LuckyWheel()
//     {
//         UiManager.Instance.ShowPanel(UIPanelType.LuckyWheelPanel);
//     }
//
//     [Category(UICategory)]
//     [DisplayName("钞票翻倍")]
//     public void BingoPanel()
//     {
//         UiManager.Instance.GetPanel<BingoAdPanel>(UIPanelType.BingoAdPanel).SetCurDollarReward(5);
//         UiManager.Instance.ShowPanel(UIPanelType.BingoAdPanel);
//     }
//
//     [Category(UICategory)]
//     [DisplayName("翻倍转盘界面")]
//     public void WheelPanel()
//     {
//         UiManager.Instance.ShowPanel(UIPanelType.NumberRewardAdPanel);
//     }
//
//     [Category(UICategory)]
//     [DisplayName("3宝箱界面")]
//     public void BoxPanel()
//     {
//         UiManager.Instance.ShowPanel(UIPanelType.BingoBoxAdPanel);
//         BingoBoxAdPanel scripts = UiManager.Instance.GetPanel<BingoBoxAdPanel>(UIPanelType.BingoBoxAdPanel);
//         scripts.InitReward(2.5f); //���¸���50%
//     }
//
//
//     [Category(UICategory)]
//     [DisplayName("翻卡广告")]
//     public void CardReward()
//     {
//         UiManager.Instance.ShowPanel(UIPanelType.CardRewardPanel);
//     }
//
//     [Category(UICategory)]
//     [DisplayName("老虎机")]
//     public void Slot()
//     {
//         UiManager.Instance.ShowPanel(UIPanelType.NumberBoxAdPanel);
//     }
//
//     [Category("功能测试")]
//     [DisplayName("开启自动叫号")]
//     public void SwitchAutoClick()
//     {
//         GameManager.Instance.autoClick = !GameManager.Instance.autoClick;
//     }
//
//     [Category("功能测试")]
//     [DisplayName("固定宝箱")]
//     public void Chest()
//     {
//         ItemUtils.ShowChestPanel(E_AdPos.MainChest);
//     }
//
//     [Category("功能测试")]
//     [DisplayName("发奖")]
//     public void SendReward()
//     {
//         UiManager.Instance.GetPanel<GamePanel>(UIPanelType.GamePanel).SetReward();
//     }
// }
