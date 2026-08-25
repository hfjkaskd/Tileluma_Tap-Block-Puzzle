using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public partial class SROptions
{
    private const string TimeCategory = "时间";

    [Category(TimeCategory)]
    [DisplayName("新的一天(重新运行游戏生效)")]
    public void NewDay()
    {
        // PlayerDataManager.Instance.playerData.lastLoginDay = 0;
        // PlayerDataManager.Instance.SaveData();
        SaveDataUtils.GameData.lastLoginTimeStamp = 0;
        SaveDataUtils.Save();
    }
}
