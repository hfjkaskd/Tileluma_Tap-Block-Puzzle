using System.Collections;
using System.Collections.Generic;
using Bizza.Sdk;
using UnityEngine;

public class SdkDemo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Platform.Instance.AdSdk.ShowRewardAd(new ShowAdArgs());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
