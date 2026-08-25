#if BIZZA_REAL_WITHDRAW
using System;
using System.Collections.Generic;
using cfg;
using Newtonsoft.Json;
using UnityEngine;


// 主要涉及 引导和提现相关 的存档

public partial class GameSaveData : ISaveData
{
    #region 引导相关

    public bool customTutorialEnd = false; // 自定义教程是否结束
    public bool customTutorialCanPlay = false;
    #endregion 

    #region 客服相关

    public List<ChatInfo> chatInfos = new List<ChatInfo>();

    public bool serviceAlter = false;
    public int serviceInfoCount = 0;

    #endregion

    #region 广告相关

    public int userTodayLookAdCount;
    public int dailyWithdrawProgressState;
    public DateTime lastGetDailyRewardTime;
    public int totalFinishAdTimes;

    #endregion

    #region 提现相关

    public bool fakeWithdrawPanelFirstWithdraw = false;
    #endregion

    #region 关卡相关

    public int playerPassLv = 0;
    
    #endregion

    #region 
    public bool newPlayerGiveProp = true;
    #endregion
}
#endif