#if BIZZA_REAL_WITHDRAW
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIPropEntry : MonoBehaviour
{
    public BizzaButton btn;
    public E_ItemType itemType;
    private Action<E_ItemType> _onClickProp;
    public TMP_Text itemNumTxt;
    public GameObject addTips;
    public Image propIcon;

    void Awake()
    {
        btn.onClick.AddListener(() =>
        {
            _onClickProp?.Invoke(itemType);
        });
    }

    void OnEnable()
    {
        UIItemUtils.Bind(itemType, itemNumTxt);
        BizzaEventSystem.Set(EventDefine.Item.ItemChanged, Action, true);
        Action();
    }

    private void Action()
    {
        UIItemUtils.Bind(itemType, itemNumTxt);
        addTips.SetActive(ItemUtils.GetItemCount(itemType) == 0);
    }

    void OnDisable()
    {
        UIItemUtils.Unbind(itemNumTxt);
        BizzaEventSystem.Set(EventDefine.Item.ItemChanged, Action, false);
    }

    public void Init(Action<E_ItemType> onClickProp)
    {
        _onClickProp = onClickProp;
    }
}
#endif