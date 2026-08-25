#if BIZZA_REAL_WITHDRAW
using System;
using System.Collections.Generic;
using UnityEngine;
using Bizza.Unity.Android;

 
public class TipNativeBridge : MonoBehaviour
{
    const string ClassName = "com.bangsawan.oceanshine.SdkHelper";
    const string ShowTip = "ShowTips";
    
    public static void ShowTips(string tips)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        // TODO: Android 原生实现
        // return AndroidPlugin.GetAndroidId();
        JavaBridgeUtils.CallStaticMethodCacheClass(ClassName, ShowTip, tips);
#else
    
#endif
    }
}
#endif