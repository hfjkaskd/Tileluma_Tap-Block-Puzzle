#if BIZZA_REAL_WITHDRAW
using System.Collections.Generic;

using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public partial class UIPageIds
{
    public static readonly PageId LosePanel = "LosePanel";
}

public class LosePanel : UIPageBase
{
    public TMP_Text levelText;

    public BizzaButton bizzaButton;

    private void Awake()
    {
        bizzaButton.onClick.AddListener(OnClickRestart);
    }


    protected override void OnClose()
    {
        
    }

    protected override void OnOpen()
    {
        levelText.text = LanguageUtils.GetFormatText("Menu_LevelBtn", SaveDataUtils.GameData.playerSelectedLv);
        BridgingUtil.GameOver();
    }

    private void OnClickRestart()
    {
        CloseSelf();

        BridgingUtil.ReloadCurrentLevel();
    }

}
#endif