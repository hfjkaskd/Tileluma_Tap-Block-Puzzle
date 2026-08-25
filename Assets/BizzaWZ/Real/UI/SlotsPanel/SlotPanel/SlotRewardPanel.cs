using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotRewardPanel : MonoBehaviour
{
    public GameObject coinObj;
    public TMP_Text coinText;

    public GameObject dollarObj;
    public TMP_Text dollarText;

    public CurrencyInfo coinTarget;
    public Transform coinTargetPos;
    public CurrencyInfo dollarTarget;
    public Transform dollarTargetPos;

    public List<SlotRewardIcon> slotRewardIcons = new List<SlotRewardIcon>()
    {
        new SlotRewardIcon() { image = null, e_WzIconType = E_WzIconType.StackMoney },
        new SlotRewardIcon() { image = null, e_WzIconType = E_WzIconType.PileMoney },
        new SlotRewardIcon() { image = null, e_WzIconType = E_WzIconType.HundredMoney },
        new SlotRewardIcon() { image = null, e_WzIconType = E_WzIconType.GoldCoin },
        new SlotRewardIcon() { image = null, e_WzIconType = E_WzIconType.PileGold },
        new SlotRewardIcon() { image = null, e_WzIconType = E_WzIconType.PileWealth }
    };

    private float coinValue;
    private float dollarValue;

    public GameObject btnObj;
    public Image resultImage;

    public void Init(float coin, float dollar, string resultType)
    {
        coinObj.SetActive(coin > 0);
        coinValue = coin;
        dollarValue = dollar;

        dollarObj.SetActive(dollar > 0);
        var value = ItemUtils.Get(E_ItemType.Dollar).Count;
        if (AccountModule.CountryType == AccountModule.E_CountryType.BR)
        {
            coinText.text = WithdrawalUtil.GetCustomizedIntByCountryType2(coin);
            dollarText.text = $"{LanguageUtils.GetText("CurrencyToken")}{WithdrawalUtil.GetCustomizedIntByCountryType2(dollar)}";
        }
        else
        {
            coinText.text = WithdrawalUtil.GetCustomizedValueByCountryType(coin);
            dollarText.text = $"{LanguageUtils.GetText("CurrencyToken")}{WithdrawalUtil.GetCustomizedFloatByCountryType(dollar)}";
        }
        
        Image image = GetSlotRewardImage(resultType);
        if (image != null)
        {
            image.gameObject.SetActive(true);
            UIUtils.SetWzSprite(resultImage, resultType);
        }
        
    }

    public Image GetSlotRewardImage(string resultType)
    {
        foreach (var item in slotRewardIcons)
        {
            item.image.gameObject.SetActive(false);
        }

        foreach (var item in slotRewardIcons)
        {
            if (item.e_WzIconType.ToString() == resultType)
            {
                UIUtils.SetWzSprite(item.image, resultType);
                return item.image;
            }
        }
        return null;
    }

    [Obfuz.ObfuzIgnore]
    public void OnClickClose()
    {
        // 钞票飞过去
       // float money = WithdrawalUtil.GetDollarCountByReward();
        ItemUtils.AddItem(
                new ItemEntry()
                {
                    Type = E_ItemType.Dollar,
                    Count = dollarValue,
                },
                new AddItemParam()
                {
                    playAnim = true,
                    isAd = false,
                    startPos = btnObj.transform.position,
                    bUiPos = false,
                    source = E_AddItemSource.UsSlot,
                    target = dollarTargetPos,
                    addFinishAction = () => dollarTarget.Refresh()
                });
       // dollarTarget.Refresh();

//         LogUtil.Error($"coinValue: {coinValue}");
        if (coinValue > 0)
        {
            // 金币飞过去
            ItemUtils.AddItem(
                    new ItemEntry()
                    {
                        Type = E_ItemType.Gold,
                        Count = coinValue,
                    },
                    new AddItemParam()
                    {
                        playAnim = true,
                        isAd = false,
                        startPos = btnObj.transform.position,
                        bUiPos = false,
                        source = E_AddItemSource.UsSlot,
                        target = coinTargetPos,
                        addFinishAction = () => coinTarget.Refresh()
                    });
            // coinTarget.Refresh();
        }
       gameObject.SetActive(false);
    }

}

[Serializable]
public struct SlotRewardIcon
{
    public Image image;
    public E_WzIconType e_WzIconType; 
}