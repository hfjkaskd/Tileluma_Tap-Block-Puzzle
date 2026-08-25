#if BIZZA_REAL_WITHDRAW
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class WithdrawLevelItem : MonoBehaviour
{
    public TMP_Text levelTxt;
    public TMP_Text rateText;

    public TMP_Text balanceText;

    public GameObject shadowObj;
    public GameObject selectedObj;
    public GameObject selectObj;
    private bool isLock;

    public Material material;
    public Image[] images;

    private int startLevel; public int StartLevel => startLevel;
    private int endLevel; public int EndLevel => endLevel;
    public int gapLevel => startLevel - SaveDataUtils.GameData.playerSelectedLv;

    private RealWithdrawPanel page;
    private string Itembalance; public string ItemBalance => Itembalance;
    private float baseRate = 2.5f;
    private double rateValue;

    private int realityLevel;

    [Header("按钮")]
    [SerializeField] private BizzaButton clickBtn;

    private void Awake()
    {
        clickBtn.onClick.AddListener(() => { OnClick(); });
    }

    public void Init(RealWithdrawPanel page, int index, float balance, AccountModule.OceanShineWithdrawalPageResponse.WithdrawalRatio data, bool isMaxState, Action<float, WithdrawLevelItem> ratioHint)
    {
        this.page = page;
        int currentLevel = SaveDataUtils.GameData.playerSelectedLv;
        selectedObj.gameObject.SetActive(false);
        shadowObj.gameObject.SetActive(false);

        startLevel = data.Os_Ben;
        endLevel = data.Os_End;
        double rate = data.Os_Wro;

        bool isMax = isMaxState; // ⚠️确认这条规则是否正确
        bool isLight = index % 2 == 0;
        realityLevel = startLevel - 1;

        bool isStateLevel = isMax
            ? currentLevel >= startLevel
            : (currentLevel >= startLevel && currentLevel <= endLevel);

        bool isLevelInSection = currentLevel >= startLevel && currentLevel <= endLevel; // 玩家当前等级是否在区间内
        bool isLevelExSection = currentLevel > endLevel; // 玩家当前等级是否超出区间
        bool isLevelNoSection = currentLevel < startLevel; // 玩家当前等级是否在区间外
                                                           // Debug.LogError($"玩家关卡 {currentLevel}，区间 {startLevel} - {endLevel}，是否在区间内 {isLevelInSection}，是否超出区间 {isLevelExSection}，是否在区间外 {isLevelNoSection}");

        if (isMax)
        {
            isLevelInSection = true;
        }

        // levelTxt.text = $"{LanguageUtils.GetFormatText("RealPage_PassLevel", startLevel)}";
        if (AccountModule.CountryType == AccountModule.E_CountryType.ID)
        {
            rateValue = rate * 1000f / baseRate;
        }
        else if (AccountModule.CountryType == AccountModule.E_CountryType.US)
        {
            rateValue = rate * 10000f / baseRate;
        }
        else if (AccountModule.CountryType == AccountModule.E_CountryType.BR)
        {
            rateValue = rate * 100f / baseRate;
        }

        if (AccountModule.CountryType == AccountModule.E_CountryType.ID)
        {
            rateText.text = $"{rateValue/10:F1}X";
        }
        else
        {
            rateText.text = $"{rateValue:F1}X";
        }
        
        // float itemBalance = (float)((double)balance * rate);
        float itemBalance = (float)Math.Round((double)(balance * rate), 2, MidpointRounding.AwayFromZero);
        Itembalance = $"{LanguageUtils.GetText("CurrencyToken")}{WithdrawalUtil.GetCustomizedValueByCountryType(itemBalance)}";
        balanceText.text = Itembalance;

        isLock = isLevelNoSection;
        shadowObj.SetActive(isLock);
        SetSelect(isLevelInSection);
        Selected(isLevelExSection, isLevelInSection);

        if (index == 0)
        {
            levelTxt.text = $"{LanguageUtils.GetFormatText("RealPage_CurrentLevel", startLevel)}";
        }
        else
        {
            levelTxt.text = $"{LanguageUtils.GetFormatText("RealPage_PassLevel", realityLevel)}";
        }

        if (isLevelInSection)
        {
            balanceText.text = $"{LanguageUtils.GetText("CurrencyToken")}{AccountModule.Instance.Get_S_Ewl()}";
            ratioHint?.Invoke((float)rate, this);
        }
    }

    public void OnClick()
    {
        page.OnClickWithdrawItem(this);
        float quota = (float)rateValue * baseRate;
        string quotaValue = $"{LanguageUtils.GetText("CurrencyToken")}{WithdrawalUtil.GetCustomizedValueByCountryType(quota)}";
        page.RefreshProgress(realityLevel, endLevel, Itembalance, quotaValue);
    }

    public void ResetContent()
    {
        selectObj.gameObject.SetActive(true);
        float quota = (float)rateValue * baseRate;
        if (AccountModule.CountryType == AccountModule.E_CountryType.ID)
        {
            quota = quota * 10;
        }
        else if (AccountModule.CountryType == AccountModule.E_CountryType.US)
        {
            quota = quota * 100;
        }
        string quotaValue = $"{LanguageUtils.GetText("CurrencyToken")}{WithdrawalUtil.GetCustomizedValueByCountryType(quota)}";
        page.RefreshProgress(startLevel, endLevel, Itembalance, quotaValue);
    }

    public void SetSelect(bool isSelect)
    {
        selectObj.gameObject.SetActive(isSelect);
    }

    public void Selected(bool isSelected, bool isLevelInSection)
    {
        if (isLevelInSection) return;
        if (!isSelected)
        {
            shadowObj.SetActive(true);
            return;
        }

        foreach (var image in images)
        {
            image.material = material;
        }
        selectedObj.SetActive(true);
    }

    public bool OnIsLock()
    {
        return isLock;
    }
}

#if UNITY_EDITOR
[Serializable]
public class WithdrawLevelItemData
{
    public string minLevel;
    public string maxLevel;
    public string rate;
    public Sprite sprite;
}
#endif
#endif