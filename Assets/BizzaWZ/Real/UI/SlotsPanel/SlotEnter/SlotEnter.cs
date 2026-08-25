#if BIZZA_REAL_WITHDRAW
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotEnter : MonoBehaviour
{
    private int slotLimit = 5;
    private int levelIndex => SaveDataUtils.GameData.playerPassLv;
    private string progressValue => $"{levelIndex % slotLimit}/{slotLimit}";

    //public TMP_Text titleTxt;

    public Image progressImag;
    public TMP_Text progressTxt;

    public GameObject slotCanHintObj;

    [SerializeField] private BizzaButton bizza;

    private void OnEnable()
    {
        bizza.onClick.AddListener(OnClick);
        BizzaEventSystem.Set(EventDefine.CustomGameEvent.SlotProgressChanged, OnWinRefresh, true);
        OnRefresh();
    }

    private void OnDisable()
    {
        bizza.onClick.RemoveListener(OnClick);
        BizzaEventSystem.Set(EventDefine.CustomGameEvent.SlotProgressChanged, OnWinRefresh, false);
    }

    private void OnWinRefresh()
    {
        OnRefresh();
    }

    public void OnRefresh()
    {
        progressTxt.text = levelIndex >= slotLimit ? $"{slotLimit}/{slotLimit}" : $"{levelIndex % slotLimit}/{slotLimit}";
        progressImag.fillAmount = levelIndex >= slotLimit ? 1 : (float)(levelIndex % slotLimit / (float)slotLimit);
        slotCanHintObj.SetActive(levelIndex >= slotLimit);

        LogLogger.LogInfo("SlotEnter OnRefresh " + "SaveDataUtils.GameData.playerUnlockedLv_" + levelIndex
            + "slotLimit_" + slotLimit + "progressValue_" + levelIndex / slotLimit);
    }

    public void OnClick()
    {
        UIModule.Instance.OpenPage(UIPageIds.SlotPanel);
    }
}
#endif