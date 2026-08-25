#if BIZZA_REAL_WITHDRAW
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowByCountryType : MonoBehaviour
{
    // 
    public AccountModule.E_CountryType showCountryType;

    private void Awake()
    {
        if (AccountModule.CountryType != AccountModule.E_CountryType.None)
        {
            Show();
        }
    }

    private void OnEnable()
    {
        BizzaEventSystem.On(EventDefine.Login.InitContentByCountry, Show);
    }

    private void OnDisable()
    {
        BizzaEventSystem.Off(EventDefine.Login.InitContentByCountry, Show);
    }

    private void Show()
    {
        // var type = AccountModule.CountryType;
        // gameObject.SetActive(type == showCountryType);
    }
}
#endif