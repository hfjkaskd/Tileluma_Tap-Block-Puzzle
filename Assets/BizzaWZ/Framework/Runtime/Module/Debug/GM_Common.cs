using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public partial class SROptions
{
    private const string CommonCategory = "通用";

    [Category(CommonCategory)]
    [DisplayName("目标语言")]
    public string targetLanguage { get; set; }

    [Category(CommonCategory)]
    [DisplayName("设置语言")]
    public void SetLanguage()
    {
        // PlayerDataManager.Instance.playerData.lastLoginDay = 0;
        // PlayerDataManager.Instance.SaveData();
        LanguageUtils.SelectedLanguage = targetLanguage;
        SaveDataUtils.Save();
    }
}
