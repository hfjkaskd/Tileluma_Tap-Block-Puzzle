#if BIZZA_REAL_WITHDRAW
using System;
using System.Collections.Generic;
using UnityEngine;
using Bizza.Unity.Android;
using Bizza.Sdk;
using System.IO;
using System.Text;
[Obfuz.ObfuzIgnore]
public static class DeviceNativeBridge
{
    const string ClassName = "com.bangsawan.oceanshine.DeviceUtils";
    const string GetAndroidIdMethod = "GetAndroidId";

    const string GetBrandMethod = "GetBrand";
    const string GetModelMethod = "GetModel";
    const string GetOSVersionMethod = "GetOSVersion";
    const string GetGAIDMethod = "GetGAID";
    const string GetNetworkTypeMethod = "GetNetworkType";
    const string IsRootedMethod = "IsRooted";
    const string HasSimCardMethod = "HasSimCard";
    const string GetCountryCodeMethod = "GetCountryCode";
    const string GetChannelMethod = "GetChannel";

    private const string GetRootCallback = "GetRootCallback";

    private static readonly System.Random Random = new System.Random();

    /* ===================== 安卓ID ===================== */
    /// <summary>
    /// 安卓ID（Android Settings.Secure.ANDROID_ID）
    /// 需要 Android 原生实现
    /// </summary>
    public static string GetAndroidId()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        var temp = JavaBridgeUtils.CallStaticMethodCacheClass<string>(ClassName, GetAndroidIdMethod);
        LogLogger.LOGUserInstall("获取的 GetAndroidId 信息 ： " + temp);
        return temp;
#else
        string fakeAndroidId = GenerateFakeAndroidId();
        return fakeAndroidId;
#endif
    }

    private static string GenerateFakeAndroidId(string fileName = "android_ids.txt")
    {
        string directoryPath = ChannelConfig.Instance.real_CustomConfig.GenerateFakeAndroidId;
        const string prefix = "Bizza";
        const string suffix = "Bizza";
        const string chars = "0123456789abcdef";
        const int totalLength = 16;

        if (string.IsNullOrWhiteSpace(directoryPath))
        {
            Debug.LogError("directoryPath 不能为空");
            return string.Empty;
        }

        try
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            int middleLength = totalLength - prefix.Length - suffix.Length;
            if (middleLength < 0)
            {
                Debug.LogError("prefix 和 suffix 长度之和超过了 totalLength");
                return string.Empty;
            }

            var sb = new StringBuilder(totalLength);
            sb.Append(prefix);

            for (int i = 0; i < middleLength; i++)
            {
                sb.Append(chars[Random.Next(chars.Length)]);
            }

            sb.Append(suffix);

            string id = sb.ToString();
            string filePath = Path.Combine(directoryPath, fileName);

            // 不覆盖旧内容，直接追加一行
            File.AppendAllText(filePath, id + Environment.NewLine, Encoding.UTF8);

            return id;
        }
        catch (Exception ex)
        {
            Debug.LogError($"GenerateFakeAndroidId 失败: {ex}");
            return string.Empty;
        }
    }

    /* ===================== 手机品牌 ===================== */
    /// <summary>
    /// 手机品牌（Android Build.BRAND）
    /// 需要原生实现（Unity SystemInfo.deviceModel 不等价）
    /// </summary>
    public static string GetBrand()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        var temp = JavaBridgeUtils.CallStaticMethodCacheClass<string>(ClassName, GetBrandMethod);
        LogLogger.LogVerbose(LogTag.LOG_Device, "获取的 GetBrandMethod 信息 ： " + temp);
        return temp;
#else
        LogLogger.LogVerbose(LogTag.LOG_Device, "走的测试路径 ： ");
        return "UMIDIGI";
#endif
    }

    /* ===================== 设备型号 ===================== */
    /// <summary>
    /// 设备型号（Android Build.MODEL）
    /// 需要原生实现
    /// </summary>
    public static string GetModel()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        var temp = JavaBridgeUtils.CallStaticMethodCacheClass<string>(ClassName, GetModelMethod);
        LogLogger.LogVerbose(LogTag.LOG_Device, "获取的 GetModelMethod 信息 ： " + temp);
        return temp;
#else
        LogLogger.LogVerbose(LogTag.LOG_Device, "走的测试路径 ： ");
        return SystemInfo.deviceModel;
#endif
    }

    /* ===================== 系统版本 ===================== */
    /// <summary>
    /// 系统版本（Android Build.VERSION.RELEASE / iOS systemVersion）
    /// 需要原生实现
    /// </summary>
    public static string GetOSVersion()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        var temp = JavaBridgeUtils.CallStaticMethodCacheClass<string>(ClassName, GetOSVersionMethod);
        LogLogger.LogVerbose(LogTag.LOG_Device, "获取的 GetOSVersionMethod 信息 ： " + temp);
        return temp;
#else
        LogLogger.LogVerbose(LogTag.LOG_Device, "走的测试路径 ： ");
        return SystemInfo.operatingSystem;
#endif
    }

    public static void GetGoogleId()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
    JavaBridgeUtils.CallStaticMethodCacheClass(ClassName, GetGAIDMethod);
        return;
#else
        string fakeGaid = GenerateFakeGaid();
        LogLogger.LogVerbose(LogTag.LOG_Device, "fakeGaid " + fakeGaid);
        BizzaEventSystem.Emit(FrameEvent.Android.UnityJavaObjectGoogleMessage, fakeGaid);
#endif
    }

    private static string GenerateFakeGaid()
    {
        return System.Guid.NewGuid().ToString().ToLower();
    }

    /* ===================== 网络类型 ===================== */
    /// <summary>
    /// 网络类型（WIFI / MOBILE / NONE）
    /// Unity 无法准确获取，需原生
    /// </summary>
    public static string GetNetworkType()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        var temp = JavaBridgeUtils.CallStaticMethodCacheClass<string>(ClassName, GetNetworkTypeMethod);
        LogLogger.LogVerbose(LogTag.LOG_Device, "获取的 GetNetworkTypeMethod 信息 ： " + temp);
        return temp;
#else
        LogLogger.LogVerbose(LogTag.LOG_Device, "走的测试路径 ： ");
        return Application.internetReachability.ToString();
#endif
    }

    /* ===================== Root 检测 ===================== */
    /// <summary>
    /// 是否 Root（1=是 0=否）
    /// 需要原生实现
    /// </summary>
    public static int IsRooted()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        BizzaEventSystem.On(FrameEvent.Android.UnityJavaObjectRootMessage, SetRootCallback);
        JavaBridgeUtils.CallStaticMethodCacheClass(ClassName, GetRootCallback);
        return 0;
#else
        LogLogger.LogVerbose(LogTag.LOG_Device, "走的测试路径 ： ");
        return 0;
#endif
    }

    private static void SetRootCallback(int result)
    {
        LogLogger.LogVerbose(LogTag.LOG_Device, "SetRootCallback " + result);
        DeviceInfoUtil.deviceInfoDataForCloud.Os_Rtt = result;
        BizzaEventSystem.Off(FrameEvent.Android.UnityJavaObjectRootMessage, SetRootCallback);
    }

    /* ===================== SIM 卡 ===================== */
    /// <summary>
    /// 是否有 SIM 卡（1=有 0=无）
    /// 需要原生 + 权限
    /// </summary>
    public static int HasSimCard()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        BizzaEventSystem.On(FrameEvent.Android.UnityJavaObjectSimCardMessage, SetHasSimCard);
        JavaBridgeUtils.CallStaticMethodCacheClass(ClassName, "GetHasSimCardMethod");
        return 0;
#else
        LogLogger.LogVerbose(LogTag.LOG_Device, "走的测试路径 ： ");
        return 0;
#endif
    }

    private static void SetHasSimCard(int result)
    {
        LogLogger.LogVerbose(LogTag.LOG_Device, "SetHasSimCard " + result);
        DeviceInfoUtil.deviceInfoDataForCloud.Os_Sbi = result;
        BizzaEventSystem.Off(FrameEvent.Android.UnityJavaObjectSimCardMessage, SetHasSimCard);
    }

    /* ===================== 国家码 ===================== */
    /// <summary>
    /// 国家码（ISO 3166，如 CN / US）
    /// Unity兜底：CultureInfo
    /// 原生可用 SIM / Network 国家码更准
    /// </summary>
    public static string GetCountryCode()
    {
        string country = ChannelConfig.Instance.real_CustomConfig.CountryName;
        LogLogger.LOGCountry($"oversea中的国家设置为 ： {country} —— ChannelConfig.Instance.real_CustomConfig.CountryName: {ChannelConfig.Instance.real_CustomConfig.CountryName}");

        #if !DEBUG_MODE
        country = "";
        LogLogger.LOGCountry($"被正式包强制修改为 默认国家 ");
        #endif

#if UNITY_ANDROID && !UNITY_EDITOR
        if (string.Equals(AccountModule.E_CountryType.None.ToString(), country))
        {
            country = "";
        }
        LogLogger.LOGCountry(
            $"给JAVA 传入的国家为 ： {country} —— ChannelConfig.Instance.real_CustomConfig.CountryName: {ChannelConfig.Instance.real_CustomConfig.CountryName}"
            );
        var temp = JavaBridgeUtils.CallStaticMethodCacheClass<string>(ClassName, GetCountryCodeMethod, country);
        LogLogger.LOGCountry( 
            $"JAVA 传回的国家为 ： {temp} —— ChannelConfig.Instance.real_CustomConfig.CountryName: {ChannelConfig.Instance.real_CustomConfig.CountryName}"
            );
        LogLogger.LogVerbose(LogTag.LOG_Device, "获取的 GetCountryCodeMethod 信息 ： " + temp);
        return temp;
#else
        if (string.IsNullOrEmpty(country) || string.Equals(AccountModule.E_CountryType.None, country))
        {
            country = AccountModule.E_CountryType.BR.ToString();
        }
        LogLogger.LOGCountry( $"编辑器下国家选择为 ： {country} —— ChannelConfig.Instance.real_CustomConfig.CountryName: {ChannelConfig.Instance.real_CustomConfig.CountryName}"
            );
        return country;
#endif
    }

    /* ===================== 渠道 ===================== */
    /// <summary>
    /// 来源渠道（如：googleplay / taptap / huawei）
    /// 通常来自：打包宏 / 配置文件 / 服务器
    /// </summary>
    public static string GetChannel()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        var temp = JavaBridgeUtils.CallStaticMethodCacheClass<string>(ClassName, GetChannelMethod);
        LogLogger.LogVerbose(LogTag.LOG_Device, "获取的 GetChannelMethod 信息 ： " + temp);
        return temp;
#else
        LogLogger.LogVerbose(LogTag.LOG_Device, "走的测试路径 ： ");
        return "editor";
#endif
    }

    /* ===================== 用户ID ===================== */
    /// <summary>
    /// 用户ID（登录成功后由业务层赋值）
    /// 这里仅兜底
    /// </summary>
    public static string GetUserId()
    {
        return "";
    }

    /// <summary>
    /// 获取 Android 软键盘高度（返回：屏幕像素高度 & Canvas高度）
    /// </summary>
    public static Dictionary<string, int> AndroidGetKeyboardHeight(int canvasHeight)
    {
        var dic = new Dictionary<string, int>();

#if UNITY_ANDROID && !UNITY_EDITOR
        using (AndroidJavaClass unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject activity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject player = activity.Get<AndroidJavaObject>("mUnityPlayer");
            AndroidJavaObject view = player.Call<AndroidJavaObject>("getView");

            using (AndroidJavaObject rect = new AndroidJavaObject("android.graphics.Rect"))
            {
                view.Call("getWindowVisibleDisplayFrame", rect);
                int androidRectH = rect.Call<int>("height");           // 非键盘部分可见高度
                int h = Screen.height - androidRectH;                  // 键盘高度（屏幕像素）

                // 兼容 Unity 某些版本的输入 Dialog 高度
                try
                {
                    AndroidJavaObject dialog = player.Get<AndroidJavaObject>("mSoftInputDialog");
                    if (dialog != null && !TouchScreenKeyboard.hideInput)
                    {
                        AndroidJavaObject decorView = dialog.Call<AndroidJavaObject>("getWindow")
                                                           .Call<AndroidJavaObject>("getDecorView");
                        if (decorView != null)
                        {
                            int h1 = decorView.Call<int>("getHeight");
                            h += h1;
                            dic["dialogH"] = h1;
                        }
                    }
                }
                catch (Exception)
                {
                    dic["dialogH"] = -1;
                }

                dic["androidRectH"] = androidRectH;
                dic["keyboardInScreenHeight"] = Mathf.Max(0, h);

                int canvasH = (int)(dic["keyboardInScreenHeight"] * (float)canvasHeight / Screen.height);
                dic["keyboardInCanvasHeight"] = Mathf.Max(0, canvasH);
            }
        }
#else
        // 编辑器/非安卓：返回 0
        dic["androidRectH"] = Screen.height;
        dic["keyboardInScreenHeight"] = 0;
        dic["keyboardInCanvasHeight"] = 0;
        dic["dialogH"] = 0;
#endif

        return dic;
    }
}
#endif