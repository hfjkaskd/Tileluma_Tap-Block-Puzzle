#if BIZZA_REAL_WITHDRAW
using System;
using System.Collections;
using System.Collections.Generic;
using Bizza.Channel;
using Bizza.Sdk;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// 设备信息收集： 负责收集并提供所有设备信息
/// </summary>
[Obfuz.ObfuzIgnore]
public static class DeviceInfoUtil
{
    public static DeviceInfoData deviceInfoData;
    private static string identity;

    public static string Identity
    {
        get
        {
            if (string.IsNullOrEmpty(identity))
            {
                WaitForGetDeviceIdentity();
            }

            return identity;
        }
    }

    public static DeviceInfoData GetData()
    {
        deviceInfoData = new DeviceInfoData();
        //TODO: 初始化基础参数
        deviceInfoData.w = UnityEngine.Device.Screen.width;
        deviceInfoData.h = UnityEngine.Device.Screen.height;
        deviceInfoData.dpi = Mathf.RoundToInt(UnityEngine.Device.Screen.dpi);
        deviceInfoData.bundle = Identity;
        deviceInfoData.vc = Convert.ToInt32(Application.version.Replace(".", ""));
        deviceInfoData.vn = Application.version;
        if (PlayerPrefs.HasKey("cdid"))
        {
            deviceInfoData.smdid = PlayerPrefs.GetString("cdid");
        }
        else
        {
            deviceInfoData.smdid = "";
        }

        Debug.Log("测试" + deviceInfoData.smdid);
#if UNITY_IOS
        infoData.os = "IOS";
        infoData.adb = "false";
        infoData.root = "false";
        infoData.ally = "false";
        infoData.mcc = IOSPluginAdapter.GetCountryCodeFromIOS();
        
        infoData.vpn = IOSPluginAdapter.IsVPNConnected();
        infoData.ue = IOSPluginAdapter.IsCallBackSimul();
        infoData.svm = IOSPluginAdapter.IsCallBackSysVNMatch();
        infoData.jbp = IOSPluginAdapter.IsCallBackIsDJbPath();
        infoData.jbu = IOSPluginAdapter.IsCallBackIsDJbUSche();
        infoData.jbl = IOSPluginAdapter.IsCallBackIsDJbLib();
        infoData.tam = IOSPluginAdapter.IsCallBackIdTamper();
        infoData.inj = IOSPluginAdapter.CheckInjectedLibraries();
        Debug.Log($"测试" + infoData.vpn +"-"+infoData.ue+"-"+infoData.ue+"-"+infoData.svm+"-"+infoData.jbp+"-"+infoData.jbu+"-"+infoData.jbl+"-"+infoData.tam+"-"+infoData.inj );
        deviceInfoData.idfv = UnityEngine.iOS.Device.vendorIdentifier;
#else
        // infoData.os = "Android";
        // infoData.adb = "false";
        // infoData.root = "false";
        // infoData.vpn = "false";
        // infoData.ally = "false";
        // infoData.mcc = “”;
#endif
        deviceInfoData.osv = SystemInfo.operatingSystem;
        deviceInfoData.language = "en";
        deviceInfoData.brand = SystemInfo.deviceName;
        deviceInfoData.model = SystemInfo.deviceModel;
        deviceInfoData.ue = 0;
        deviceInfoData.idfv = "dasd"; // UnityEngine.Android.Device.vendorIdentifier;
        deviceInfoData.tz = TimeZoneInfo.Local.Id;
        deviceInfoData.ouid = IOSPluginAdapter.GetOpenUDIDFromiOS();
        if (!string.IsNullOrEmpty(identity))
        {
            deviceInfoData.ifa = identity;
        }
        else
        {
            AsyncLoadIdentity(2).Forget();
        }

        Debug.Log(deviceInfoData.ouid);

        return deviceInfoData;
    }

    public static string UrlInfo()
    {
        var reachability = Application.internetReachability;
        switch (reachability)
        {
            case NetworkReachability.ReachableViaCarrierDataNetwork:
                deviceInfoData.contype = "4G"; break;
            case NetworkReachability.ReachableViaLocalAreaNetwork:
                deviceInfoData.contype = "WIFI"; break;
        }

        deviceInfoData.ts = TimeUtil.GetCurrentTime(ETimeUnit.MILLISECONDS);
        deviceInfoData.vs = GetVerifySign();
        deviceInfoData.st = HttpUtil.serverTime;
        //加密返回
        string jsonStr = JsonConvert.SerializeObject(deviceInfoData);
        Debug.Log(jsonStr);
        string urlEncoding =
            EncryptionUtil.UrlSafeBase64EncodeWithAes(jsonStr);
        return
            $"info={urlEncoding}&bundle={ChannelConfig.Instance.httpConfig.bundle_name}&svn={ChannelConfig.Instance.httpConfig.doc_version}";
    }

    private static string GetVerifySign()
    {
        return EncryptionUtil.MD5Encoding(deviceInfoData.bundle +
                                          ChannelConfig.Instance.httpConfig.doc_version +
                                          deviceInfoData.vn + deviceInfoData.ts +
                                          ChannelConfig.Instance.httpConfig.salt);
    }

    public static async UniTask AsyncLoadIdentity(float seconds)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(seconds));
        WaitForGetDeviceIdentity().Forget();
    }

    // TODO 这里要变成 安卓和IOS设备的身份id辨别点
    public static async UniTask WaitForGetDeviceIdentity()
    {
        var isDone = false;
        string errorMsg = null;
        const float timeout = 10f;
        float startTime = Time.time;

#if UNITY_EDITOR
        // 编辑器环境下模拟IDFA获取
        Debug.Log("[IDFA] 编辑器环境 - 使用模拟IDFA");
        identity = Guid.NewGuid().ToString();
        deviceInfoData.ifa = identity;
        isDone = true;
#else
        // 真机环境下实际获取IDFA
        Debug.Log("[IDFA] 开始获取IDFA");

        // 使用UniTask来处理回调
        await UniTask.WaitUntil(() => isDone || (Time.time - startTime) > timeout);

        // IOSPluginAdapter.GetIDFAString((r, ifa) =>
        // {
        //     if (!r)
        //     {
        //         errorMsg = "获取IDFA失败";
        //         Debug.LogError("[IDFA] " + errorMsg);
        //     }
        //     else if (string.IsNullOrEmpty(ifa))
        //     {
        //         errorMsg = "获取到的IDFA为空";
        //         Debug.LogWarning("[IDFA] " + errorMsg);
        //     }
        //     else
        //     {
        //         infoData.ifa = ifa;
        //         idfa = ifa;
        //         Debug.Log("[IDFA] 成功获取IDFA");
        //     }
        //
        //     isDone = true;
        // });
#endif

        // 如果超时，则设置错误消息
        if (!isDone)
        {
            errorMsg = "获取IDFA超时";
            Debug.LogError("[IDFA] " + errorMsg);
        }

        Debug.Log($"[设备身份 Identity] 获取完成 : {identity}, 错误: {errorMsg ?? "无"}");
    }


    #region 玩客云的 设备信息

    public static AccountModule.OceanShineUserLoginRequest deviceInfoDataForCloud;

    public static AccountModule.OceanShineUserLoginRequest GetDeviceInfoDataForCloud()
    {
        if (deviceInfoDataForCloud == null ||
            deviceInfoDataForCloud.Equals(default(AccountModule.OceanShineUserLoginRequest)))
        {
            deviceInfoDataForCloud = new AccountModule.OceanShineUserLoginRequest()
            {
                // ✅ Unity可直接获取
                Os_Apid = ChannelConfig.Instance.AppId, // 应用ID
                Os_Atd = Debug.isDebugBuild ? 1 : 0, // 调试模式：1=是 0=否
                Os_Cpu = SystemInfo.processorType, // CPU架构（Unity侧只能拿到处理器描述；更准确ABI需原生）
                Os_Ctr = DateTime.Now.Hour, // 当前小时值（0-23）
                Os_Dtd = Screen.dpi.ToString(), // 屏幕密度（dpi；部分机型可能返回0）
                Os_Dth = Screen.height, // 屏幕高
                Os_Dtw = Screen.width, // 屏幕宽
                Os_Lag = "",// Application.systemLanguage.ToString(), // 语言
                Os_Seid = PlayerPrefs.GetString(AccountModule.m_seidKey) , // 客户端UUID（建议：可改为自己持久化生成的UUID）
                Os_Ttz = TimeZoneInfo.Local.Id, // 时区
                Os_Ua = "Unity-Android", // 客户端标识（自定义）
                Os_Vc = ParseVersionCode(Application.version), // App版本号（自定义解析）
                Os_Vn = Application.version, // App版本名

                // ⚠️ 需要原生(Android/iOS)获取 / Unity拿不到或不可靠 -> 默认值
                Os_Anid = DeviceNativeBridge.GetAndroidId(), // 安卓ID：需原生 Android Settings.Secure.ANDROID_ID
                Os_Bbd = DeviceNativeBridge.GetBrand(), // 手机品牌：需原生 Android Build.BRAND / iOS需对应信息
                Os_Mbl = DeviceNativeBridge.GetModel(), // 设备型号：需原生 Android Build.MODEL
                Os_Obv = DeviceNativeBridge.GetOSVersion(), // 系统版本：需原生 Android Build.VERSION.RELEASE / iOS systemVersion
                
                // 报错
                Os_Gaid = PlayerPrefs.GetString(AccountModule.m_googleIdKey), // Google广告ID(GAID)：需原生 + Google Play服务（且需合规/用户同意）
                
                Os_Nbt = DeviceNativeBridge.GetNetworkType(), // 网络类型：需原生 ConnectivityManager / iOS Reachability
                Os_Rtt = DeviceNativeBridge.IsRooted(), // 是否root：需原生检测（1=是 0=否）
                Os_Sbi = DeviceNativeBridge.HasSimCard(), // SIM卡信息：需原生（可能还需要权限/系统限制）
                
                Os_Try = DeviceNativeBridge.GetCountryCode(), // 国家码：建议原生获取网络/Sim国家码（或Locale）更准
                Os_Cal = DeviceNativeBridge.GetChannel(), // 来源渠道：一般由你们渠道系统/打包宏定义/远端配置提供
                Os_Usid = DeviceNativeBridge.GetUserId() // 用户ID：你们登录系统拿到后再赋值
            };
        }

        return deviceInfoDataForCloud;
    }

    #endregion
    
    private static int ParseVersionCode(string version)
    {
        // 1.2.3 -> 10203（示例）
        var arr = version.Split('.');
        int major = arr.Length > 0 ? int.Parse(arr[0]) : 0;
        int minor = arr.Length > 1 ? int.Parse(arr[1]) : 0;
        int patch = arr.Length > 2 ? int.Parse(arr[2]) : 0;
        return major * 100 + minor * 10 + patch;
    }
}

[Serializable]
public class DeviceInfoData
{
    public int w; // 屏幕宽度
    public int h; // 屏幕高度
    public int dpi; // 屏幕 DPI
    public string bundle; // 应用包名
    public int vc; // 应用版本（整数形式）
    public string vn; // 应用版本（字符串形式）
    public string language; // 语言
    public string os; // 操作系统
    public string osv; // 操作系统版本
    public string contype; // 网络类型
    public long ts; // 时间戳
    public string brand; // 品牌
    public string model; // 设备型号
    public string adb; // 是否开启调试
    public string root; // 是否已 root
    public string ally; // 是否为 ally
    public string mcc; // 国家代码
    public string vs; // 版本
    public string referrer; // 引荐来源
    public string ifa; // IDFA
    public long st; // 状态
    public string tz; // 时区
    public string idfv; // IDFV
    public string ouid; // OpenUDID
    public int vpn; // 是否连接 VPN
    public int ue; // 其他标识
    public int svm; // 系统匹配标识
    public int jbp; // DJb路径标识
    public int jbu; // DJb USche标识
    public int jbl; // DJb Lib标识
    public int tam; // 篡改检测
    public int inj; // 注入库检测
    public string smdid; // 自定义 ID
}

#endif