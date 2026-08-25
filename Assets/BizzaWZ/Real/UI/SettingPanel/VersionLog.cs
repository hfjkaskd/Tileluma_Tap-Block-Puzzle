using System.Collections;
using System.Collections.Generic;
using Bizza.Sdk;
using TMPro;
using UnityEngine;

public class VersionLog : MonoBehaviour
{
    public TMP_Text tMP_Text;
    private void Start()
    {
        if (tMP_Text == null)
        {
            tMP_Text = GetComponent<TMP_Text>();
        }
        if (tMP_Text== null )return;
        tMP_Text.text = $"{ChannelConfig.Instance.real_CustomConfig.versionLog}_{(ChannelConfig.IsRelese_Mode ? 0 : 1)}";
    }
}
