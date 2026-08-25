using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MoreMountains.NiceVibrations;
using UnityEngine;
  [Obfuz.ObfuzIgnore]
 
public enum E_VibrateType
{
    None,
    Light,
    Medium,
    Heavy,
    // Short,
    // Middle,
    // Long,
}

public static class VibrationUtils
{
    public static void VibrateDuration(E_VibrateType vibrateType, float duration, float interval)
    {
        if (!SaveDataUtils.SettingData.enableVibrate)
        {
            return;
        }

        for (float t = 0; t < duration; t += interval)
        {
            GameUtils.DelayDo(() =>
            {
                Vibrate(vibrateType);
            }, t);
        }
    }

    public static void Vibrate(E_VibrateType vibrateType)
    {
        if (!SaveDataUtils.SettingData.enableVibrate)
        {
            return;
        }

        #if UNITY_EDITOR
        #endif

        switch (vibrateType)
        {
            case E_VibrateType.Light:
                MMVibrationManager.Haptic(HapticTypes.LightImpact);
                break;
            case E_VibrateType.Medium:
                MMVibrationManager.Haptic(HapticTypes.MediumImpact);
                break;
            case E_VibrateType.Heavy:
                MMVibrationManager.Haptic(HapticTypes.HeavyImpact);
                break;
        }

    }
}
