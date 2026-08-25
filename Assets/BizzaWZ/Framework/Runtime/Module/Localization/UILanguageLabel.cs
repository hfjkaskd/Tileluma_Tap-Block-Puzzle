using System.Collections;
using System.Collections.Generic;
using Bizza.Sdk;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class UILanguageLabel : MonoBehaviour
{
    public string key;
    private TMP_Text _tmp;
    public bool bounce;

    void OnEnable()
    {
        OnLanguageChange();
        BizzaEventSystem.Set(EventDefine.Frame.LanguageChange, OnLanguageChange, true);
    }

    void OnDisable()
    {
        BizzaEventSystem.Set(EventDefine.Frame.LanguageChange, OnLanguageChange, false);
    }

    void OnLanguageChange()
    {
        if (_tmp == null)
        {
            _tmp = GetComponent<TMP_Text>();
        }

        if (_tmp != null && !string.IsNullOrEmpty(key))
        {
            var text = LanguageUtils.GetText(key);
            if (bounce)
            {
                text = $"<bounce>{text}</bounce>";
            }
            _tmp.text = text;
        }
    }
}
