using System;
using System.Collections.Generic;
using System.Text;
using Bizza.Loading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;
using Bizza.Channel;
using Bizza.Sdk;
using Bizza.Unity.Android;
using System.Linq;

[Obfuz.ObfuzIgnore]
public class AccountModule : BaseGameModule<AccountModule>
{
    public bool Enabled => true;

    public static string adJustAdidKey = "adJustAdidKey";
    public static string adJustGooglIdKey = "adJustGoogleIdKey";
    public static string networkKey = "network";
    public static string campaignKey = "campaign";
    public static string adgroupkKey = "adgroup";

    public override void InitGameModule()
    {
        base.InitGameModule();
    }

    public void Login()
    {
        var cfg = ChannelConfig.Instance;
        LogLogger.LOGGameStart($"静态配置 :: " +
                               $"是否为发布阶段 {ChannelConfig.IsRelese_Mode} " +
                               $"GM开启 {cfg.GM}; " +
                               $"ECPM1000 {cfg.real_CustomConfig.testECPM1000}; " +
                               $"测试设备 {cfg.real_CustomConfig.openTestDevice}; " +
                               $"国家类型 {cfg.real_CustomConfig.CountryName} 读取国家开启 {string.Equals(cfg.real_CustomConfig.CountryName, AccountModule.E_CountryType.None)}; "
                               );

        OnLoginData().Forget();
    }

    #region 登录时加载玩家数据

    public static string m_userIdKey = "userId"; // 用来获取 userId 的 key
    public static string m_googleIdKey = "googleId"; // 用来获取 userId 的 key
    public static string m_seidKey = "seidId";
    public static string m_Os_RtiKey = "Os_Rti";
    public static string m_Os_AruKey = "Os_Aru";
    public static string m_Os_AcmKey = "Os_Acm";
    public static string m_Os_AiuKey = "Os_Aiu";

    public static double m_Temp_ACM
    {
        get
        {
            return PlayerPrefs.GetFloat("m_Temp_ACM", 0);
        }
        set
        {
            PlayerPrefs.SetFloat("m_Temp_ACM", (float)value);
        }
    }
    public static double m_Temp_AIU
    {
        get
        {
            return PlayerPrefs.GetFloat("m_Temp_AIU", 0);
        }
        set
        {
            PlayerPrefs.SetFloat("m_Temp_AIU", (float)value);
        }
    }

    public static double m_Temp_ShowECPM
    {
        get
        {
            return PlayerPrefs.GetFloat("m_Temp_ShowECPM", 0);
        }
        set
        {
            PlayerPrefs.SetFloat("m_Temp_ShowECPM", (float)value);
        }
    }

    public static string PIX = "pix";
    public static string PAGBANK = "pagbank";
    public static string PAYPAL = "paypal";
    public static string OVO = "ovo";
    public static string DANA = "dana";
    public static string SHOPEE = "Shopeepay";
    public static string GOPAY = "gopay";

    public string ADShow = "ad_show";
    public string ADClick = "ad_click";
    public string ADLoad = "ad_load";
    public string ADRewardType = "Rewarded";
    public string ADInterstitialType = "Interstitial";
    private const string HttpCategoryEvent = "event_update";
    private const string HttpCategoryProperty = "property_update";
    private const string HttpCategoryQuery = "query";
    private const string HttpRequestEventName = "ta_http_request";

    private const float UsMinWithdrawalValue = 0.2f;
    private const float IDMinWithdrawalValue = 50;
    private const float BRMinWithdrawalValue = 0.3f;

    private void SidecarTrackHttpRequest(string functionName, string endpoint, string requestCategory,
        string requestParams = null)
    {
        SidecarTrackHttpEvent(functionName, endpoint, requestCategory, requestParams);
    }

    private void SidecarTrackHttpEvent(string functionName, string endpoint, string requestCategory,
        string requestParams = null)
    {
        try
        {
            var data = new Dictionary<string, object>
            {
                { "request_function", functionName },
                { "http_endpoint", endpoint },
                { "request_category", requestCategory },
                { "request_bytes", requestParams },
                { "request_time_ms", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }
            };

#if UNITY_EDITOR
            // var info = JsonConvert.SerializeObject(data);
            // Debug.LogError($"[BizzaSidecar] {HttpRequestEventName} Track event: {info}");
#endif

            BizzaAnalyticsAgent.Track(HttpRequestEventName, data);
        }
        catch (Exception ex)
        {
            LogLogger.LogVerbose(LogTag.LOG_Account, $"[BizzaSidecar] Track event failed: {ex.Message}");
        }
    }

    #region 国家类型

    public static string CURRENCY_Usa = "US"; //美元
    public static string CURRENCY_Indonesia = "ID"; //印尼盾
    public static string CURRENCY_Brasil = "BR"; // 巴西雷亚尔

    private static E_CountryType countryType = E_CountryType.None;

    public static E_CountryType CountryType
    {
        set { countryType = value; }
        get
        {
            if (countryType == E_CountryType.None)
            {
                LogLogger.LogVerbose(LogTag.LOG_Account, "没有选择到服务器下发的国家");
                countryType = E_CountryType.None;
            }

            return countryType;
        }
    }

    public static E_CountryType SetCountryTypeByServer(string CountryCode)
    {
        LogLogger.LogVerbose(LogTag.LOG_Account, "CountryCode " + CountryCode + "--");

        if (string.Equals(CountryCode, CURRENCY_Usa, StringComparison.OrdinalIgnoreCase))
        {
            CountryType = E_CountryType.US;
        }
        else if (string.Equals(CountryCode, CURRENCY_Indonesia, StringComparison.OrdinalIgnoreCase))
        {
            CountryType = E_CountryType.ID;
        }
        else if (string.Equals(CountryCode, CURRENCY_Brasil, StringComparison.OrdinalIgnoreCase))
        {
            CountryType = E_CountryType.BR;
        }

        LogLogger.LogVerbose(LogTag.LOG_Account, "国家 " + CountryType);
        return CountryType;
    }

    public static void SetLanguageByCountryType()
    {
        switch (CountryType)
        {
            case E_CountryType.US:
                LanguageUtils.SelectedLanguage = "en-US";
                break;
            case E_CountryType.BR:
                LanguageUtils.SelectedLanguage = "pt-BR";
                break;
            case E_CountryType.ID:
                LanguageUtils.SelectedLanguage = "id-ID";
                break;
        }

        // LanguageUtils.SelectedLanguage = "zh-CN";
        LogLogger.LogVerbose(LogTag.LOG_Account, "LanguageUtils.SelectedLanguage " + LanguageUtils.SelectedLanguage);
    }

    public static float PointsToUs = 10000;
    public static float UsToIndonesia = 15151;
    public static float UsToBrazil = 5;
    [Obfuz.ObfuzIgnore]

    public enum E_CountryType
    {
        [LabelText("无指定国家")]
        None,
        [LabelText("美国")]
        US,
        [LabelText("巴西")]
        BR,
        [LabelText("印尼")]
        ID
    }

    #endregion


    private int retryUserIdTimes = 3;
    private int retryUserInfoTimes = 3;

    public bool LoadingFinish
    {
        get
        {
            return loginUserIdFinish && loginUserInfoFinish;
        }
    }



    //public bool deviceInfoFinish => ;

    private bool loginUserIdFinish = false;
    public bool LoginUserIdFinish => loginUserIdFinish;
    private bool loginUserInfoFinish = false;
    public bool LoginUserInfoFinish => loginUserInfoFinish;
    private bool loginAttributeFinish = false;

    //private bool root

    private bool isHaveGoogleId = false;

    private async UniTask OnLoginData()
    {
        loginUserIdFinish = false;
        loginUserInfoFinish = false;
        loginAttributeFinish = false;

        const string ClassName = "com.bangsawan.oceanshine.HttpUtil";
        const string SendHttpMethodName = "initialize";
        JavaBridgeUtils.CallStaticMethodCacheClass(ClassName, SendHttpMethodName, Application.identifier, Application.version, DeviceInfoUtil.GetDeviceInfoDataForCloud().Os_Vc.ToString());

        // 先查看是否有 用户id
#if UNITY_EDITOR
        if (ChannelConfig.Instance.real_CustomConfig.useEditorUserId)
        {
            PlayerPrefs.SetString(m_userIdKey, ChannelConfig.Instance.real_CustomConfig.editorUserId);
        }
#endif
        var id = PlayerPrefs.GetString(m_userIdKey);
        // 如果 没有用户id
        if (string.IsNullOrEmpty(id))
        {
            LogLogger.LOGUserInstall($" 没有用户id  进入到请求流程");
            // 获取谷歌 id
            var googleid = PlayerPrefs.GetString(m_googleIdKey);

            // 如果 谷歌id 没有则
            if (string.IsNullOrEmpty(googleid))
            {
                LogLogger.LOGUserInstall($" 没有用户id  进入到 没有谷歌id请求");
                BizzaEventSystem.On(FrameEvent.Android.UnityJavaObjectGoogleMessage, SetGoogleId);

                void SetGoogleId(string googleId)
                {
                    LogLogger.LOGUserInstall($"谷歌ID {googleId}");
                    BizzaEventSystem.Off(FrameEvent.Android.UnityJavaObjectGoogleMessage, SetGoogleId);
                    if (string.IsNullOrEmpty(googleId))
                    {
                        googleId = PlayerPrefs.GetString(adJustGooglIdKey);
                    }

                    PlayerPrefs.SetString(m_googleIdKey, googleId);
                    PlayerPrefs.Save();
                    isHaveGoogleId = true;
                }

                DeviceNativeBridge.GetGoogleId();
                await UniTask.WaitUntil(() => isHaveGoogleId);

                string _id = PlayerPrefs.GetString(m_googleIdKey);
                login(_id);
            }
            else
            {
                login(googleid);
            }
        }
        else
        {
            LogLogger.LOGUserInstall($" 有用户id了 {id}");
            InitUserInfo();
        }
    }

    // 只有第一次登录时才会进来， 如果有 UserId 那么就不会走到这里
    private void login(string id)
    {
        string Os_Seid = PlayerPrefs.GetString(m_seidKey, Guid.NewGuid().ToString());

        var deviceInfo = DeviceInfoUtil.GetDeviceInfoDataForCloud();
        deviceInfo.Os_Seid = Os_Seid;
        if (string.IsNullOrEmpty(deviceInfo.Os_Gaid))
        {
            deviceInfo.Os_Gaid = id;
        }

        LogLogger.LOGUserInstall($" 设备信息提交 {id}");
        deviceInfo.Print();
        Request_LoginRequest(deviceInfo, Response_LoginResponse);
    }

    private void InitUserInfo()
    {
        loginUserIdFinish = true; // 走到用户信息这一步 肯定就是有用户id了
        BizzaSdk.Ad.SetUserId(PlayerPrefs.GetString(m_userIdKey));

        Request_UserInfoRequest(true, false, Response_UserInfoResponse);
        Request_AppOtherConfigRequest(null, false);
        Request_RoutineTaskLookAdMoneyRequest(true, null, false);
        Request_FeedbackListV2Request(CreateOceanShineFeedbackListV2Request(), (FailHttpResponse<OceanShineFeedbackListV2Response> response) =>
        {
            if (response.success && response.data != null && response.data.Os_Msl != null)
            {
                if (response.data.Os_Msl.Count != SaveDataUtils.GameData.serviceInfoCount)
                {
                    SaveDataUtils.GameData.serviceAlter = true;
                }
                else
                {
                    SaveDataUtils.GameData.serviceAlter = false;
                }
                SaveDataUtils.Save();

            }


        });
    }

    #endregion

    private UserInfo Os_Uso;
    private bool isLoadingAlter = false;

    public UserInfo Os_Current_Uso
    {
        get => Os_Uso;
        set
        {
            if (value != null)
            {
                LogLogger.LOGUserInfoAlter($"用户信息更新 + {value.GetInfo()} + {isLoadingAlter}");
            }
            if (Os_Uso != null)
            {
                LogLogger.LOGUserInfoAlter($"之前用户信息 + {Os_Uso.GetInfo()} + {isLoadingAlter}");
            }

            // #if UNITY_EDITOR
            //             if (ChannelConfig.Instance.real_PageTestConfig.testExPanel)
            //             {
            // #endif
            // #if !UNITY_EDITOR
            if (Os_Uso != null && isLoadingAlter && Os_Uso.Os_Tra < value.Os_Tra)
            {
                // #endif
                double beforeBlance = 0;
                beforeBlance = value.GetBalance();
                double beforeClash = 0;
                beforeClash = Os_Uso.Os_Ewl;
                double nowBlance = 0;
                nowBlance = value.GetBalance();
                double nowClash = 0;
                nowClash = value.Os_Ewl;
                LogLogger.LOGUserInfoAlter($"打开体现比例上升 + {beforeBlance} + {beforeClash} + {nowBlance} + {nowClash}");
                ExchangeRateInfo exchangeRateInfo = new ExchangeRateInfo()
                {
                    beforeBlance = beforeBlance,
                    beforeClash = beforeClash,
                    nowBlance = nowBlance,
                    nowClash = nowClash,
                    beforeRate = Os_Uso.Os_Tra,
                    nowRate = value.Os_Tra,
                };
                UIModule.Instance.OpenPage(UIPageIds.ExchangeRatePanel, exchangeRateInfo);
            }

            SaveDataUtils.GameData.todayAdTimes = value.Os_Lgd;
            SaveDataUtils.GameData.fakeWithdrawPanelFirstWithdraw = value.Os_Mny == 0;
            
            // FakeWithdrawPanel.isReward = value.Os_Mny == 0;
            // LogLogger.LogVerbose(LogTag.Frame, "用户是否是新手玩家" + FakeWithdrawPanel.isReward + " --- Value.Os_Mny : " + value.Os_Mny);
            isLoadingAlter = true;
            Os_Uso = value;
            BizzaEventSystem.Emit(EventDefine.Item.ItemChanged);
            SaveDataUtils.Save();
        }
    }

    public static string version => Application.version;

    public string Get_S_Ewl()
    {
        string _ewl = WithdrawalUtil.GetCustomizedValueByCountryType((float)Os_Uso.Os_Ewl);
        return _ewl;
    }

    [Serializable]
    public class UserInfo
    {
        [JsonProperty("PbSbac")]
        public double Os_Bac; // 当前金额 balance - 巴西/印尼

        [JsonProperty("PbSbaci")]
        public int Os_Baci; // 当前积分数 balance_int - 美国

        [JsonProperty("PbScon")]
        public int Os_Con; // 当前金币数 coin

        [JsonProperty("PbScrc")]
        public string Os_Crc; // 国家货币 currency

        [JsonProperty("PbScrcs")]
        public string Os_Crcs; // 国家货币符号 currencySymbols

        [JsonProperty("PbScty")]
        public string Os_Cty; // 国家 country

        [JsonProperty("PbSewl")]
        public double Os_Ewl; // 可提现金额 enableWithdrawal

        [JsonProperty("PbSimg")]
        public int Os_Img; // 是否有消息未读，1是，0否

        [JsonProperty("PbSlvl")]
        public int Os_Lev; // 当前关卡 level

        [JsonProperty("PbSlgd")]
        public int Os_Lgd; // 用户连续登录天数 loginDay

        [JsonProperty("PbSmy")]
        public double Os_Mny; // 用户真实的钱（货币）

        [JsonProperty("PbSisn")]
        public bool Os_Ncm; // 是否是新手 newComer

        [JsonProperty("PbSnnm")]
        public string Os_Nnm; // 用户昵称 nickName

        [JsonProperty("PbSrl")]
        public double Os_Rol; // 用户的卷（货币）

        [JsonProperty("PbSrti")]
        public long Os_Rti; // 注册时间 registerTime

        [JsonProperty("PbSrts")]
        public int Os_Rts; // 当前关卡 reachTimes

        [JsonProperty("PbStrt")]
        public double Os_Tra; // 可提现比例 taxRate

        public double GetBalance()
        {
            double balance = CountryType switch
            {
                E_CountryType.BR => Os_Bac,
                E_CountryType.ID => Os_Bac,
                E_CountryType.US => Os_Baci,
                _ => Os_Bac
            };

            return balance;
        }

        public string GetInfo()
        {
            return
                $"Os_Bac (当前金额-巴西/印尼): {Os_Bac}\n" +
                $"Os_Baci (当前积分-美国): {Os_Baci}\n" +
                $"Os_Con (当前金币数): {Os_Con}\n" +
                $"Os_Crc (货币类型): {Os_Crc}\n" +
                $"Os_Crcs (货币符号): {Os_Crcs}\n" +
                $"Os_Cty (国家): {Os_Cty}\n" +
                $"Os_Ewl (可提现金额): {Os_Ewl}\n" +
                $"Os_Img (是否有未读消息): {Os_Img}\n" +
                $"Os_Lev (当前关卡): {Os_Lev}\n" +
                $"Os_Lgd (连续登录天数): {Os_Lgd}\n" +
                $"Os_Mny (真实货币金额): {Os_Mny}\n" +
                $"Os_Ncm (是否新手): {Os_Ncm}\n" +
                $"Os_Nnm (用户昵称): {Os_Nnm}\n" +
                $"Os_Rol (用户卷/货币): {Os_Rol}\n" +
                $"Os_Rti (注册时间戳): {Os_Rti}\n" +
                $"Os_Rts (当前关卡): {Os_Rts}\n" +
                $"Os_Tra (可提现比例/税率): {Os_Tra}\n" +
                $"Balance (根据国家计算的余额): {GetBalance()}";
        }
    }


    #region 发行需要的API

    #region 增加收益 --- 看完广告后获取的内容

    // 请求体数据模型类
    [Serializable]
    public class OceanShineAdRevenueRequest
    {
        [JsonProperty("PbSapid")]
        public string Os_Apid; // 应用ID

        [JsonProperty("PbStcid")]
        public string Os_Btid; // 获取收益ID

        [JsonProperty("PbSecm")]
        public double Os_Ecm; // 广告ECPM

        [JsonProperty("PbSspl")]
        public string Os_Sal; // 特殊标识：ad_show / task_10000 / small

        [JsonProperty("PbSusid")]
        public string Os_Usid; // 用户ID

        [JsonProperty("vn")]
        public string Os_Vn; // 应用版本


        public string Print()
        {
            return
                $"Os_Apid : {Os_Apid} Os_Btid : {Os_Btid} Os_Ecm : {Os_Ecm} Os_Sal : {Os_Sal} Os_Usid : {Os_Usid} Os_Vn : {Os_Vn} ";
        }
    }


    // 响应体数据模型类
    [Serializable]
    public class OceanShineAdRevenueResponse
    {
        [JsonProperty("PbSbac")]
        public int Os_Bac; // 获取的卷 --- 美国使用

        [JsonProperty("PbSuso")]
        public UserInfo Os_Uso; // 用户信息

        [JsonProperty("PbSprc")]
        public double Os_Prc; // 获取的金额（当前国家）--- 巴西和印尼

        [JsonProperty("PbSmul")]
        public double Os_Mul; // 原基础上增加倍数，1为1倍，2为2倍，0.1为10%，0.05为5%
        [JsonProperty("PbScon")]
        public double Os_Con; // 获取的金币-有可能为0->coin

        public double GetBalance()
        {
            LogLogger.LOGUserInfoAlter($"看完广告后的内容 ---- Os_Bac : {Os_Bac} Os_Prc : {Os_Prc}");

            double balance = CountryType switch
            {
                E_CountryType.BR => Os_Prc,
                E_CountryType.ID => Os_Prc,
                E_CountryType.US => Os_Bac,
                _ => Os_Bac
            };

            return balance;
        }

        public string Print()
        {
            return $"Os_Bac : {Os_Bac} Os_Prc : {Os_Prc}";
        }
    }

    public OceanShineAdRevenueResponse AdRevenueResponse = new();

    /// <summary>
    /// 获取用户请求体
    /// </summary>
    /// <param name="_Os_Btid">获取收益 ID</param>
    /// <param name="_Os_Ecm">广告 ECPM</param>Http
    /// <param name="_Os_Sal">特殊标识</param>
    /// <returns></returns>
    public OceanShineAdRevenueRequest GetAdRevenueRequest(double _Os_Ecm, string _Os_Sal, string id)
    {
        OceanShineAdRevenueRequest request = new OceanShineAdRevenueRequest()
        {
            Os_Apid = DeviceInfoUtil.GetDeviceInfoDataForCloud().Os_Apid,
            Os_Usid = PlayerPrefs.GetString(AccountModule.m_userIdKey),
            Os_Vn = DeviceInfoUtil.GetDeviceInfoDataForCloud().Os_Vn,
            Os_Btid = id,
            Os_Ecm = _Os_Ecm,
            Os_Sal = _Os_Sal,
        };

        LogLogger.LOGUserInfoAlter(" 增加收益结果 " + request.Print());
        return request;
    }

    public void Request_AdRevenueRequest(OceanShineAdRevenueRequest request, E_AdType adType,
        Action<FailHttpResponse<OceanShineAdRevenueResponse>> callback = null, float delay = 0)
    {
        Action<FailHttpResponse<OceanShineAdRevenueResponse>> _callback = null;
        _callback += OnRequest;
        _callback += callback;
        string requestParams = JsonConvert.SerializeObject(request);
        HttpUtil.RequestToServer<OceanShineAdRevenueResponse>(AccountModuleCfg.add_ecpm, requestParams, _callback,
            false, delay);
        SidecarTrackHttpRequest(nameof(Request_AdRevenueRequest), AccountModuleCfg.add_ecpm, HttpCategoryEvent,
            requestParams);

        void OnRequest(FailHttpResponse<OceanShineAdRevenueResponse> response)
        {
            LogLogger.LogVerbose(LogTag.ADReportFlow, "请求用户id 来自于 增加收益");
            // Request_GetAdRevenueReportIdRequest(adType, null, false);
            if (!response.success || response.data == null)
            {
                LogLogger.LogVerbose(LogTag.ADReportFlow, "增加收益回调失败，设置为无id");
                return;
            }


            AdRevenueResponse = response.data;
            Os_Current_Uso = response.data.Os_Uso;
            LogLogger.LogVerbose(LogTag.ADReportFlow, "当前是否是新的一天 " + SaveDataUtils.GameData.lastGetDailyRewardTime.Date + " " + DateTime.Now.Date);
            var last = SaveDataUtils.GameData.lastGetDailyRewardTime;
#if UNITY_EDITOR
            if (ChannelConfig.Instance.real_PageTestConfig.testDialyWithdrawalPanel)
            {
                UIModule.Instance.OpenPage(UIPageIds.DailyWithdrawPanel);
            }
#endif
            if (last == DateTime.MinValue || last.Date < DateTime.Now.Date)
            {
                // 可以领取每日奖励
                double cur = Os_Current_Uso.Os_Ewl;
                LogLogger.LogVerbose(LogTag.ADReportFlow, $"进入到当天的第一次提现 cur :: {cur} --- {CountryType} ");
                switch (CountryType)
                {
                    case E_CountryType.BR:
                        if (cur < BRMinWithdrawalValue)
                        {
                            return;
                        }

                        break;
                    case E_CountryType.ID:
                        if (cur < IDMinWithdrawalValue)
                        {
                            return;
                        }

                        break;
                    case E_CountryType.US:
                        if (cur < UsMinWithdrawalValue)
                        {
                            return;
                        }

                        break;
                }

                LogLogger.LogVerbose(LogTag.LOG_Account, $"可提现{Os_Current_Uso.Os_Ewl} ");
                SaveDataUtils.GameData.lastGetDailyRewardTime = DateTime.Now;
                SaveDataUtils.gameStrategy.SaveData();
                UIModule.Instance.OpenPage(UIPageIds.DailyWithdrawPanel);
            }
        }
    }

    #endregion

    #region 去提现 - 全部提现 --- 点击提现后

    // 请求体数据模型类，包含了去提现请求所需的字段
    [Serializable]
    public class OceanShineApplyWithdrawalRequestReal
    {
        [JsonProperty("PbSapid")]
        public string Os_Apid; // 应用ID

        [JsonProperty("PbScpf")]
        public string Os_Cp; // CPF

        [JsonProperty("PbSgdid")]
        public int Os_Gdid; // 商品ID

        [JsonProperty("PbSmtid")]
        public int Os_Mid; // 商品中的支付配置ID

        // [JsonProperty("Mn")]
        // public string Os_Mn; // money现金提现节点 或 task_任务ID

        [JsonProperty("PbSpbt")]
        public string Os_Pb; // Pix账号绑定类型 (P E C B)

        [JsonProperty("PbSrat")]
        public string Os_Ra; // ReceiverAccount

        [JsonProperty("PbSrel")]
        public string Os_Re; // ReceiverEmail

        [JsonProperty("PbSrme")]
        public string Os_Rm; // ReceiverMobile

        [JsonProperty("PbSrae")]
        public string Os_Rn; // ReceiverName

        [JsonProperty("PbSseid")]
        public string Os_Seid; // 客户端UUID

        [JsonProperty("PbSusid")]
        public string Os_Usid; // 用户ID

        [JsonProperty("vn")]
        public string Os_Vn; // 应用版本

        // [JsonProperty("At")]
        // public string Os_At; // 现金或卷提现 applyType：money 或 rolll

        [JsonProperty("PbStrt")]  // -------------------------------------------多出来的API
        public string Os_Trt; // 选择提现的比例
    }

    // 响应体数据模型类，包含了去提现请求的响应字段
    [Serializable]
    public class OceanShineApplyWithdrawalResponse
    {
        [JsonProperty("PbSorn")]
        public string Os_Odr; // 订单号 orderNo
    }

    public OceanShineApplyWithdrawalRequestReal GetApplyWithdrawalRequestReal(string _Os_Cp, int _Os_Mid, string _Os_Pb,
        string _Os_Ra, string _Os_Re, string _Os_Rm, string _Os_Rn, string Os_Mn, string _Os_At)
    {
        OceanShineApplyWithdrawalRequestReal request = new OceanShineApplyWithdrawalRequestReal()
        {
            Os_Apid = DeviceInfoUtil.GetDeviceInfoDataForCloud().Os_Apid,
            Os_Vn = DeviceInfoUtil.GetDeviceInfoDataForCloud().Os_Vn,
            Os_Usid = PlayerPrefs.GetString(AccountModule.m_userIdKey),
            Os_Seid = DeviceInfoUtil.GetDeviceInfoDataForCloud().Os_Seid,

            // Os_Mn = Os_Mn,
            Os_Cp = _Os_Cp,
            Os_Gdid = 0,
            Os_Mid = _Os_Mid,
            Os_Pb = _Os_Pb,
            Os_Ra = _Os_Ra,
            Os_Re = _Os_Re,
            Os_Rm = _Os_Rm,
            Os_Rn = _Os_Rn,
            //Os_At = _Os_At,
            Os_Trt = "0"
        };
        return request;
    }

    public void Request_ApplyWithdrawalRequest(OceanShineApplyWithdrawalRequestReal request,
        Action<FailHttpResponse<OceanShineApplyWithdrawalResponse>> callback = null)
    {
        Action<FailHttpResponse<OceanShineApplyWithdrawalResponse>> replace = null;
        replace += OnResponse;
        replace += callback;
        string requestParams = JsonConvert.SerializeObject(request);
        LogLogger.LogVerbose(LogTag.LOG_Account, "提现信息： " + requestParams + "");
        HttpUtil.RequestToServer(AccountModuleCfg.add_order, requestParams, replace, true);
        SidecarTrackHttpRequest(nameof(Request_ApplyWithdrawalRequest), AccountModuleCfg.add_order,
            HttpCategoryEvent, requestParams);

        void OnResponse(FailHttpResponse<OceanShineApplyWithdrawalResponse> response)
        {
            Request_UserInfoRequest(true, true, (r) => { });
        }
    }

    #endregion

    #region 新接口-去提现-现金或奖卷 --- 请求 返回参数 和 "去提现 - 全部提现" 一致
    [Serializable]
    public class OceanShineApplyWithdrawalRequestFake
    {
        [JsonProperty("PbSapid")]
        public string Os_Apid; // 应用ID

        [JsonProperty("PbScpf")]
        public string Os_Cp; // CPF

        [JsonProperty("PbSgdid")]
        public int Os_Gdid; // 商品ID

        [JsonProperty("PbSmtid")]
        public int Os_Mid; // 商品中的支付配置ID

        [JsonProperty("Mn")]
        public string Os_Mn; // money现金提现节点 或 task_任务ID

        [JsonProperty("PbSpbt")]
        public string Os_Pb; // Pix账号绑定类型 (P E C B)

        [JsonProperty("PbSrat")]
        public string Os_Ra; // ReceiverAccount

        [JsonProperty("PbSrel")]
        public string Os_Re; // ReceiverEmail

        [JsonProperty("PbSrme")]
        public string Os_Rm; // ReceiverMobile

        [JsonProperty("PbSrae")]
        public string Os_Rn; // ReceiverName

        [JsonProperty("PbSseid")]
        public string Os_Seid; // 客户端UUID

        [JsonProperty("PbSusid")]
        public string Os_Usid; // 用户ID

        [JsonProperty("vn")]
        public string Os_Vn; // 应用版本

        [JsonProperty("At")]
        public string Os_At; // 现金或卷提现 applyType：money 或 rolll
    }

    public OceanShineApplyWithdrawalRequestFake GetApplyWithdrawalRequestFake(string _Os_Cp, int _Os_Mid, string _Os_Pb,
        string _Os_Ra, string _Os_Re, string _Os_Rm, string _Os_Rn, string _Os_Mn, string _Os_At)
    {
        OceanShineApplyWithdrawalRequestFake request = new OceanShineApplyWithdrawalRequestFake()
        {
            Os_Apid = DeviceInfoUtil.GetDeviceInfoDataForCloud().Os_Apid,
            Os_Vn = DeviceInfoUtil.GetDeviceInfoDataForCloud().Os_Vn,
            Os_Usid = PlayerPrefs.GetString(AccountModule.m_userIdKey),
            Os_Seid = DeviceInfoUtil.GetDeviceInfoDataForCloud().Os_Seid,

            Os_Mn = _Os_Mn,
            Os_Cp = _Os_Cp,
            Os_Gdid = 0,
            Os_Mid = _Os_Mid,
            Os_Pb = _Os_Pb,
            Os_Ra = _Os_Ra,
            Os_Re = _Os_Re,
            Os_Rm = _Os_Rm,
            Os_Rn = _Os_Rn,
            Os_At = _Os_At
        };
        return request;
    }

    public void Request_ApplyWithdrawalMoneyRollReqRequest(OceanShineApplyWithdrawalRequestFake request,
        Action<FailHttpResponse<OceanShineApplyWithdrawalResponse>> callback = null)
    {
        Action<FailHttpResponse<OceanShineApplyWithdrawalResponse>> replace = null;
        replace += OnResponse;
        replace += callback;
        string requestParams = JsonConvert.SerializeObject(request);
        HttpUtil.RequestToServer(AccountModuleCfg.add_order_task, requestParams, replace, true);
        SidecarTrackHttpRequest(nameof(Request_ApplyWithdrawalMoneyRollReqRequest), AccountModuleCfg.add_order_task,
            HttpCategoryEvent, requestParams);

        void OnResponse(FailHttpResponse<OceanShineApplyWithdrawalResponse> response)
        {
            Request_UserInfoRequest(true, true, (r) => { });
        }

    }

    #endregion

    #region 其它配置信息

    // 请求体数据模型类，包含了其它配置信息请求所需的字段
    [Serializable]
    public class OceanShineAppOtherConfigRequest
    {
        [JsonProperty("PbSapid")]
        public string Os_Apid; // 应用ID

        [JsonProperty("vn")]
        public string Os_Vn; // 应用版本

        [JsonProperty("PbScfn")]
        public string Os_cfn; // 配置Key
    }

    public string OceanShineAppOtherConfigResponse;

    public int GetMaxDrawithRatio(bool isCash)
    {
        if (OceanShineAppOtherConfigResponse == null) return 0;

        var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(OceanShineAppOtherConfigResponse);
        if (dict == null) return 0;

        int lookedTimes = Mathf.Max(SaveDataUtils.GameData.todayAdTimes,
            AccountModule.Instance.Os_Current_Uso.Os_Lgd); // 已看广告次数

        int _ratioTemp = 0;

        int oneLook = Convert.ToInt32(dict[AccountModuleCfg.one_Count]);
        int oneRatio = Convert.ToInt32(dict[AccountModuleCfg.one_Ratio]);
        if (lookedTimes >= oneLook)
        {
            _ratioTemp = oneRatio;
        }

        int twoLook = Convert.ToInt32(dict[AccountModuleCfg.two_Count]);
        int twoRatio = Convert.ToInt32(dict[AccountModuleCfg.two_Ratio]);
        if (lookedTimes >= twoLook)
        {
            _ratioTemp = twoRatio;
        }

        int threeLook = Convert.ToInt32(dict[AccountModuleCfg.three_Count]);
        int threeRatio = Convert.ToInt32(dict[AccountModuleCfg.three_Ratio]);
        if (lookedTimes >= threeLook)
        {
            _ratioTemp = threeRatio;
        }

        return _ratioTemp;
    }



    public void Request_AppOtherConfigRequest(Action<bool, string> callback = null, bool black = true)
    {
        if (!string.IsNullOrEmpty(OceanShineAppOtherConfigResponse))
        {
            callback?.Invoke(true, OceanShineAppOtherConfigResponse);
            return;
        }

        callback += OnRequest;
        OceanShineAppOtherConfigRequest request = new OceanShineAppOtherConfigRequest()
        {
            Os_Apid = DeviceInfoUtil.GetDeviceInfoDataForCloud().Os_Apid,
            Os_Vn = DeviceInfoUtil.GetDeviceInfoDataForCloud().Os_Vn,
            Os_cfn = "look_ad_reward_mul"
        };
        string requestParams = JsonConvert.SerializeObject(request);
        HttpUtil.RequestToServer(AccountModuleCfg.base_list, requestParams, callback, black);
        SidecarTrackHttpRequest(nameof(Request_AppOtherConfigRequest), AccountModuleCfg.base_list, HttpCategoryQuery,
            requestParams);

        void OnRequest(bool success, string response)
        {
            OceanShineAppOtherConfigResponse = response;
        }
    }

    #endregion

    #region 用户归因 ---- 用户安装的第一次

    // 请求体数据模型类，包含了用户归因请求所需的字段  -- 实际没有用到不用替换
    [Serializable]
    public class OceanShineUserAttrsRequest
    {
        [JsonProperty("PbSadgp")]
        public string Os_Agp; // 设备当前归因广告组的名称

        [JsonProperty("PbSaid")]
        public string Os_Aid; // 设备的唯一 Adjust ID

        [JsonProperty("PbSapid")]
        public string Os_Apid; // 应用ID

        [JsonProperty("PbScat")]
        public double Os_Cat; // 安装成本 costAmount

        [JsonProperty("PbSccy")]
        public string Os_Ccy; // 成本相关的货币代码 costCurrency

        [JsonProperty("PbSckl")]
        public string Os_Ckl; // 安装被标记的点击标签 clickLabel

        [JsonProperty("PbScam")]
        public string Os_Cmp; // 设备当前归因 campaign

        [JsonProperty("PbSctv")]
        public string Os_Cti; // 设备当前归因素材名称 creative

        [JsonProperty("PbSctm")]
        public long Os_Ctm; // 客户端收到回调的时间 clientTime

        [JsonProperty("PbScty")]
        public string Os_Cty; // 推广活动定价模型 costType

        [JsonProperty("PbSdbu")]
        public string Os_Dlu; // 投放参数 deepLinkUrl

        [JsonProperty("PbSevt")]
        public string Os_Evt; // 触发归因的事件 event

        [JsonProperty("PbSfbr")]
        public string Os_Fbr; // 回调原始值 fbInstallReferrer

        [JsonProperty("PbSgbt")]
        public string Os_Gct; // Google 点击时间 ggClickTime

        [JsonProperty("PbSnbt")]
        public string Os_Net; // 归因渠道名称 network

        [JsonProperty("PbStra")]
        public string Os_Trn; // 归因跟踪码 trackerName

        [JsonProperty("PbStrt")]
        public string Os_Trt; // 归因跟踪名称 trackerToken

        [JsonProperty("PbSusid")]
        public string Os_Usid; // 用户ID

        [JsonProperty("vn")]
        public string Vn; // 应用版本
    }

    /// <summary>
    /// 用户归因 上报请求（无业务返回值）
    /// </summary>
    public void Request_UserAttrsRequest(Dictionary<string, object> request,
        Action<FailHttpResponse<OceanShineUserAttrsRequest>> callback = null)
    {
        LogLogger.LogAttribute($"Adjust归因上报服务器");
        string requestParams = JsonConvert.SerializeObject(request);
        // 无返回体，使用 object
        HttpUtil.RequestToServer<OceanShineUserAttrsRequest>(AccountModuleCfg.from, requestParams, callback, false);
        SidecarTrackHttpRequest(nameof(Request_UserAttrsRequest), AccountModuleCfg.from, HttpCategoryProperty,
            requestParams);
    }

    #endregion

    #region 获取收益ID-定制版参数 ---- 多个广告平台的 获取ID 当前应该不用

    // 请求体数据模型类，包含了获取收益ID请求所需的字段
    [Serializable]
    public class OceanShineGetAdRevenueReportIdRequest
    {
        [JsonProperty("PbSapid")]
        public string Os_Apid; // 应用ID

        [JsonProperty("PbSusid")]
        public string Os_Usid; // 用户ID

        [JsonProperty("vn")]
        public string Os_Vn; // 应用版本
    }

    // 响应体数据模型类，包含了获取收益ID请求的响应字段
    [Serializable]
    public class OceanShineGetAdRevenueReportIdResponse
    {
        [JsonProperty("PbStcid")]
        public string Os_Btid; // 收益ID batch_id

        [JsonProperty("PbSpfrm")]
        public string Os_Pfm; // 广告平台名称
    }

    public void Request_BatchId(Action<FailHttpResponse<OceanShineGetAdRevenueReportIdResponse>> callback = null)
    {
        OceanShineGetAdRevenueReportIdRequest request = new()
        {
            Os_Apid = DeviceInfoUtil.GetDeviceInfoDataForCloud().Os_Apid,
            Os_Usid = PlayerPrefs.GetString(AccountModule.m_userIdKey),
            Os_Vn = DeviceInfoUtil.GetDeviceInfoDataForCloud().Os_Vn,
        };
        string requestParams = JsonConvert.SerializeObject(request);
        HttpUtil.RequestToServer<OceanShineGetAdRevenueReportIdResponse>(AccountModuleCfg.get_ecpm_id, requestParams,
            callback);
        SidecarTrackHttpRequest(nameof(Request_BatchId), AccountModuleCfg.get_ecpm_id, HttpCategoryQuery, requestParams);
    }

    #endregion

    #region 用户登陆-定制版参数 --- 用户如果没有 Uid时才会调用 --- 暂时没有问题

    // 请求体数据模型类，包含了用户登录请求所需的字段
    [Serializable]
    public class OceanShineUserLoginRequest
    {
        [JsonProperty("PbSanid")]
        public string Os_Anid; // 安卓ID

        [JsonProperty("PbSapid")]
        public string Os_Apid; // 应用ID

        [JsonProperty("PbSatd")]
        public int Os_Atd; // 调试模式

        [JsonProperty("PbSbbd")]
        public string Os_Bbd; // 手机品牌

        [JsonProperty("PbScal")]
        public string Os_Cal; // 来源

        [JsonProperty("PbSctr")]
        public int Os_Ctr; // 当前小时值

        [JsonProperty("PbSdtd")]
        public string Os_Dtd; // 屏幕密度

        [JsonProperty("PbSdth")]
        public int Os_Dth; // 屏幕高

        [JsonProperty("PbSdtw")]
        public int Os_Dtw; // 屏幕宽

        [JsonProperty("PbSgaid")]
        public string Os_Gaid; // GoogleId

        [JsonProperty("PbSlag")]
        public string Os_Lag; // 语言

        [JsonProperty("PbSmbl")]
        public string Os_Mbl; // 设备型号

        [JsonProperty("PbSnbt")]
        public string Os_Nbt; // 网络

        [JsonProperty("PbSobv")]
        public string Os_Obv; // 系统版本

        [JsonProperty("PbSrtt")]
        public int Os_Rtt; // 是否root

        [JsonProperty("PbSsbi")]
        public int Os_Sbi; // SIM卡

        [JsonProperty("PbSseid")]
        public string Os_Seid; // 客户端UUID

        [JsonProperty("PbStry")]
        public string Os_Try; // 国家码

        [JsonProperty("PbSttz")]
        public string Os_Ttz; // 时区

        [JsonProperty("PbSusid")]
        public string Os_Usid; // 用户ID

        [JsonProperty("cpu")]
        public string Os_Cpu; // CPU架构

        [JsonProperty("ua")]
        public string Os_Ua; // 客户端

        [JsonProperty("vc")]
        public int Os_Vc; // App版本号

        [JsonProperty("vn")]
        public string Os_Vn; // App版本名

        /// <summary>
        /// 打印当前对象所有字段
        /// </summary>
        public void Print()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("===== OceanShineUserLoginRequest =====");
            sb.AppendLine($"Os_Anid (安卓ID): {Os_Anid}");
            sb.AppendLine($"Os_Apid (应用ID): {Os_Apid}");
            sb.AppendLine($"Os_Atd (调试模式): {Os_Atd}");
            sb.AppendLine($"Os_Bbd (手机品牌): {Os_Bbd}");
            sb.AppendLine($"Os_Cal (来源): {Os_Cal}");
            sb.AppendLine($"Os_Cpu (CPU架构): {Os_Cpu}");
            sb.AppendLine($"Os_Ctr (当前小时): {Os_Ctr}");
            sb.AppendLine($"Os_Dtd (屏幕密度): {Os_Dtd}");
            sb.AppendLine($"Os_Dth (屏幕高): {Os_Dth}");
            sb.AppendLine($"Os_Dtw (屏幕宽): {Os_Dtw}");
            sb.AppendLine($"Os_Gaid (GoogleId): {Os_Gaid}");
            sb.AppendLine($"Os_Lag (语言): {Os_Lag}");
            sb.AppendLine($"Os_Mbl (设备型号): {Os_Mbl}");
            sb.AppendLine($"Os_Nbt (网络): {Os_Nbt}");
            sb.AppendLine($"Os_Obv (系统版本): {Os_Obv}");
            sb.AppendLine($"Os_Rtt (是否Root): {Os_Rtt}");
            sb.AppendLine($"Os_Sbi (SIM卡): {Os_Sbi}");
            sb.AppendLine($"Os_Seid (UUID): {Os_Seid}");
            sb.AppendLine($"Os_Try (国家码): {Os_Try}");
            sb.AppendLine($"Os_Ttz (时区): {Os_Ttz}");
            sb.AppendLine($"Os_Ua (客户端): {Os_Ua}");
            sb.AppendLine($"Os_Usid (用户ID): {Os_Usid}");
            sb.AppendLine($"Os_Vc (版本号): {Os_Vc}");
            sb.AppendLine($"Os_Vn (版本名): {Os_Vn}");
            sb.AppendLine("====================================");

            Debug.Log(sb.ToString());
        }
    }

    // 响应体数据模型类，包含了用户登录请求的响应字段
    [Serializable]
    public class OceanShineLoginResponse
    {
        [JsonProperty("PbSare")]
        public ActiveRule Os_Aru; // activeRule

        [JsonProperty("PbSrdt")]
        public string Os_Rdt; // 注册日期

        [JsonProperty("PbSrti")]
        public long Os_Rti; // 注册时间戳

        [JsonProperty("PbSim")]
        public int Os_Rw; // 当前模式，0无模式(开发)，1审核模式，2投放模式

        [JsonProperty("PbSusid")]
        public string Os_Usid; // 用户ID

        [Serializable]
        public class ActiveRule
        {
            [JsonProperty("PbSacm")]
            public double Os_Acm; // ad_cpm

            [JsonProperty("PbSaiu")]
            public double Os_Aiu; // ad_ipu
        }
    }

    public void Request_LoginRequest(OceanShineUserLoginRequest userLoginRequest,
        Action<FailHttpResponse<OceanShineLoginResponse>> callback = null)
    {
        string requestParams = JsonConvert.SerializeObject(userLoginRequest);
        HttpUtil.RequestToServer<OceanShineLoginResponse>(AccountModuleCfg.login, requestParams, callback, false);
        SidecarTrackHttpRequest(nameof(Request_LoginRequest), AccountModuleCfg.login, HttpCategoryProperty,
            requestParams);
    }

    public void Response_LoginResponse(FailHttpResponse<OceanShineLoginResponse> response)
    {
        LogLogger.LogVerbose(LogTag.LOG_Account, $"=== OceanShineLoginResponse ===");
        if (!response.success || response.data == null)
        {
            if (response.errorCode == "1001" || response.errorCode == "400")
            {
                var args = new CommonConfirmTipsPanel.Args
                {
                    isLanguage = true,
                    des = "HTTPNetworkProblem",
                    onFinish = OnFinish,
                };
                UIModule.Instance.OpenPage(UIPageIds.CommonConfirmTipsPanel, args);

                void OnFinish()
                {
                    Application.Quit();
                }
            }

            return;
        }

        if (response.success)
        {
            if (!string.IsNullOrEmpty(response.data.Os_Usid))
            {
                PlayerPrefs.SetString(m_userIdKey, response.data.Os_Usid);
            }

            m_Temp_ACM = response.data.Os_Aru.Os_Acm;
            m_Temp_AIU = response.data.Os_Aru.Os_Aiu;

            InitUserInfo();

            if (response.data.Os_Aru.Os_Aiu == 0)
            {
                // 在这里进行 归因初始化
                BizzaSdk.Attribution.AttributeStart("checkIPU");
            }

        }
        else
        {
            if (retryUserIdTimes > 0)
            {
                login("");
                retryUserIdTimes--;
            }
            else
            {
                // TODOItem 有一个 网络检查的标签
            }
        }

        LogLogger.LogVerbose(LogTag.LOG_Account, $"+++ OceanShineLoginResponse +++");
    }

    #endregion

    #region 增加反馈 -- 可能时用户的点击

    // 请求体数据模型类，包含了添加反馈请求所需的字段
    [Serializable]
    public class OceanShineFeedbackRequest
    {
        [JsonProperty("PbSapid")]
        public string Os_Apid; // 应用ID

        [JsonProperty("PbSdtn")]
        public string Os_Des; // 反馈描述

        [JsonProperty("PbSusid")]
        public string Os_Usid; // 用户ID

        [JsonProperty("vn")]
        public string Os_Vn; // 应用版本
    }

    public OceanShineFeedbackRequest CreateFeedbackRequest(string description)
    {
        return new OceanShineFeedbackRequest
        {
            Os_Apid = DeviceInfoUtil.GetDeviceInfoDataForCloud().Os_Apid,
            Os_Des = description,
            Os_Usid = PlayerPrefs.GetString(AccountModule.m_userIdKey),
            Os_Vn = version
        };
    }

    /// <summary>
    /// 增加反馈 请求（无返回数据）
    /// </summary>
    public void Request_FeedbackRequest(OceanShineFeedbackRequest request,
        Action<FailHttpResponse<OceanShineFeedbackRequest>> callback = null)
    {
        string requestParams = JsonConvert.SerializeObject(request);

        // 无返回值，使用 object / 或者你们内部的 EmptyResponse
        HttpUtil.RequestToServer(AccountModuleCfg.msg, requestParams, callback);
        SidecarTrackHttpRequest(nameof(Request_FeedbackRequest), AccountModuleCfg.msg, HttpCategoryEvent, requestParams);
    }

    #endregion

    #region 反馈列表 --- 也是用户点击把

    // 请求体数据模型类，包含了获取反馈列表请求所需的字段
    [Serializable]
    public class OceanShineFeedbackListV2Request
    {
        [JsonProperty("PbSapid")]
        public string Os_Apid; // 应用ID

        [JsonProperty("PbSusid")]
        public string Os_Usid; // 用户ID

        [JsonProperty("vn")]
        public string Os_Vn; // 应用版本
    }

    public OceanShineFeedbackListV2Request CreateOceanShineFeedbackListV2Request()
    {
        return new OceanShineFeedbackListV2Request
        {
            Os_Apid = DeviceInfoUtil.GetDeviceInfoDataForCloud().Os_Apid,
            Os_Usid = PlayerPrefs.GetString(AccountModule.m_userIdKey),
            Os_Vn = version
        };
    }

    // 响应体数据模型类，包含了反馈列表请求的响应字段
    [Serializable]
    public class OceanShineFeedbackListV2Response
    {
        [JsonProperty("PbSmsl")]
        public List<OceanShineFeedbackListV2ResponseData> Os_Msl; // 反馈信息列表

        [JsonProperty("PbSuso")]
        public UserInfo Os_Uso; // 用户信息部分

        [Serializable]
        public class OceanShineFeedbackListV2ResponseData
        {
            [JsonProperty("PbSapid")]
            public string Os_Apid; // 应用ID

            [JsonProperty("PbScda")]
            public string Os_Cda; // 提交时间 created_at

            [JsonProperty("PbScte")]
            public long Os_Cte; // 提交时间毫秒 created_time

            [JsonProperty("PbSctt")]
            public string Os_Ctt; // 内容 msg/text

            [JsonProperty("PbSid")]
            public long Os_Id; // 数据库ID

            [JsonProperty("PbStpe")]
            public long Os_Tpe; // 类型，1用户反馈，2后台回答

            [JsonProperty("PbSuda")]
            public string Os_Uda; // 更新时间 updated_at

            [JsonProperty("PbSusid")]
            public string Os_Usid; // 用户ID

            [JsonProperty("PbSute")]
            public long Os_Ute; // 更新时间毫秒 updated_time

            [JsonProperty("vn")]
            public string Os_Vn; // 版本号
        }

        public List<OceanShineFeedbackListV2ResponseData> GetSortedFeedbackByCreatedTime()
        {
            if (Os_Msl == null)
                return new List<OceanShineFeedbackListV2ResponseData>();

            // 使用 LINQ 创建新列表并排序
            return Os_Msl;
        }
    }

    // 实例
    public OceanShineFeedbackListV2Request feedbackListV2Request = new();
    public OceanShineFeedbackListV2Response feedbackListV2Response = new();

    /// <summary>
    /// 获取反馈列表 请求
    /// </summary>
    public void Request_FeedbackListV2Request(OceanShineFeedbackListV2Request request,
        Action<FailHttpResponse<OceanShineFeedbackListV2Response>> callback = null)
    {
        callback += OnRequest;
        string requestParams = JsonConvert.SerializeObject(request);
        HttpUtil.RequestToServer<OceanShineFeedbackListV2Response>(AccountModuleCfg.msg_list, requestParams, callback);
        SidecarTrackHttpRequest(nameof(Request_FeedbackListV2Request), AccountModuleCfg.msg_list, HttpCategoryQuery,
            requestParams);

        void OnRequest(FailHttpResponse<OceanShineFeedbackListV2Response> response)
        {
            if (!response.success || response.data == null)
                return;

            if (response.data.Os_Msl != null)
            {
                SaveDataUtils.GameData.serviceInfoCount = response.data.Os_Msl.Count;
            }
            Os_Current_Uso = response.data.Os_Uso;
        }
    }

    #endregion

    #region 新用户奖励-领取  ---- 第一次登录时的判定  --- 会修改 UserInfo

    // 请求体数据模型类，包含了新用户奖励领取请求所需的字段
    [Serializable]
    public class OceanShineUserReachReportRequest
    {
        [JsonProperty("PbSapid")]
        public string Os_Apid; // 应用ID

        [JsonProperty("PbSusid")]
        public string Os_Usid; // 用户ID
    }

    // 响应体数据模型类，包含了新用户奖励领取请求的响应字段
    [Serializable]
    public class OceanShineUserReachReportResponse
    {
        [JsonProperty("PbSacg")]
        public NewcomerReward Os_Acg; // 新手奖励信息

        [JsonProperty("PbSuso")]
        public UserInfo Os_Uso; // 用户信息部分

        [Serializable]
        public class NewcomerReward
        {
            [JsonProperty("PbScom")]
            public int Os_Com; // commonMerge

            [JsonProperty("PbSnba")]
            public int Os_Nba; // 新手奖励积分数 new_balance

            [JsonProperty("PbSncc")]
            public int Os_Ncc; // 新手奖励金币 new_comer_coin

            [JsonProperty("PbSrnw")]
            public double Os_Rnw; // 新手奖励金额 newComerReward

            [JsonProperty("PbSusd")]
            public double Os_Usd; // 转usd比例 toUsd
        }
    }

    /// <summary>
    /// 新用户奖励-领取 请求
    /// </summary>
    public void Request_UserReachReportRequest(
        Action<FailHttpResponse<OceanShineUserReachReportResponse>> callback = null)
    {
        callback += OnRequest;
        var request = new AccountModule.OceanShineUserReachReportRequest()
        {
            Os_Apid = DeviceInfoUtil.GetDeviceInfoDataForCloud().Os_Apid,
            Os_Usid = PlayerPrefs.GetString(AccountModule.m_userIdKey),
        };
        string requestParams = JsonConvert.SerializeObject(request);
        HttpUtil.RequestToServer<OceanShineUserReachReportResponse>(AccountModuleCfg.new_user, requestParams, callback,
            false);
        SidecarTrackHttpRequest(nameof(Request_UserReachReportRequest), AccountModuleCfg.new_user, HttpCategoryEvent,
            requestParams);

        void OnRequest(FailHttpResponse<OceanShineUserReachReportResponse> response)
        {
            if (!response.success || response.data == null)
                return;
            Os_Current_Uso = response.data.Os_Uso;
        }
    }

    #endregion

    #region 关卡，完成次数上报 --- 和 新用户奖励-领取 一致

    /// <summary>
    /// 关卡，完成次数上报 请求
    /// </summary>
    public void Request_UserReachLevelReportRequest(
        Action<FailHttpResponse<OceanShineUserReachReportResponse>> callback = null)
    {
        callback += OnRequest;
        var request = new AccountModule.OceanShineUserReachReportRequest()
        {
            Os_Apid = DeviceInfoUtil.GetDeviceInfoDataForCloud().Os_Apid,
            Os_Usid = PlayerPrefs.GetString(AccountModule.m_userIdKey),
        };
        string requestParams = JsonConvert.SerializeObject(request);
        HttpUtil.RequestToServer<OceanShineUserReachReportResponse>(AccountModuleCfg.number, requestParams, callback,
            false);
        SidecarTrackHttpRequest(nameof(Request_UserReachLevelReportRequest), AccountModuleCfg.number,
            HttpCategoryEvent, requestParams);

        void OnRequest(FailHttpResponse<OceanShineUserReachReportResponse> response)
        {
            if (!response.success || response.data == null)
                return;
            Os_Current_Uso = response.data.Os_Uso;
        }
    }

    #endregion

    #region 订单列表 -- 历史提现记录

    // 请求体数据模型类，包含了订单列表请求所需的字段
    [Serializable]
    public class OceanShineUserInfoRequest
    {
        [JsonProperty("PbSapid")]
        public string Os_Apid; // 应用ID

        [JsonProperty("PbSusid")]
        public string Os_Usid; // 用户ID
    }

    [Serializable]
    public class OceanShineWithdrawalRecord
    {
        [JsonProperty("PbSrvp")]
        public string Os_Cp; // CPF

        [JsonProperty("PbScrc")]
        public string Os_Crc; // 货币码 currency

        [JsonProperty("PbScrcs")]
        public string Os_Crcs; // 货币符号 currencySymbols

        [JsonProperty("PbSdat")]
        public string Os_Dat; // 提现时间 date

        [JsonProperty("PbSded")]
        public double Os_Ded; // 扣除金额或现金提现时转roll金额

        [JsonProperty("PbSid")]
        public long Os_Id; // 订单ID

        [JsonProperty("PbSpxe")]
        public string Os_Pb; // Pix账号绑定类型 P/E/C/B

        [JsonProperty("PbSprc")]
        public double Os_Prc; // 金额 amount

        [JsonProperty("PbSpym")]
        public string Os_Pym; // 支付名称 payName

        [JsonProperty("PbSrva")]
        public string Os_Ra; // 收款账户 ReceiverAccount

        [JsonProperty("PbSrve")]
        public string Os_Re; // 收款人电子邮件 ReceiverEmail

        [JsonProperty("PbSrvm")]
        public string Os_Rm; // 收款人手机号码 ReceiverMobile

        [JsonProperty("PbSrvn")]
        public string Os_Rn; // 收款人姓名 ReceiverName

        [JsonProperty("PbSrrm")]
        public string Os_Rrm; // 提示信息 remarks

        [JsonProperty("PbSsts")]
        public int Os_Sts; // 状态：1进行中，2违规被拒，3成功，4失败

        [JsonProperty("PbStry")]
        public int Os_Try; // 1现金提现，2roll提现

        [JsonProperty("PbStsm")]
        public string Os_Tsm; // 提示信息（现金提现失败后）
    }

    /// <summary>
    /// 订单列表 请求
    /// </summary>
    public void Request_WithdrawalRecordRequest(
        Action<FailHttpResponse<List<OceanShineWithdrawalRecord>>> callback = null)
    {
        var request = new AccountModule.OceanShineUserInfoRequest()
        {
            Os_Apid = DeviceInfoUtil.GetDeviceInfoDataForCloud().Os_Apid,
            Os_Usid = PlayerPrefs.GetString(AccountModule.m_userIdKey),
        };
        string requestParams = JsonConvert.SerializeObject(request);
        HttpUtil.RequestToServer(AccountModuleCfg.order_list, requestParams, callback, false);
        SidecarTrackHttpRequest(nameof(Request_WithdrawalRecordRequest), AccountModuleCfg.order_list, HttpCategoryQuery,
            requestParams);
    }

    #endregion

    #region 提现用户列表  

    // 请求体数据模型类，包含了提现用户列表请求所需的字段
    [Serializable]
    public class OceanShineWithdrawalListRequest
    {
        [JsonProperty("PbSapid")]
        public string Os_Apid; // 应用ID

        [JsonProperty("PbSusid")]
        public string Os_Usid; // 用户ID

        [JsonProperty("vn")]
        public string Os_Vn; // 应用版本
    }

    // 响应体数据模型类，包含了提现用户列表请求的响应字段
    [Serializable]
    public class OceanShineWithdrawalUserListResponse
    {
        [JsonProperty("PbSmsl")]
        public List<OceanShineWithdrawalUserListResponseData> Os_Msl; // 提现用户列表

        [Serializable]
        public class OceanShineWithdrawalUserListResponseData
        {
            [JsonProperty("PbScss")]
            public string Os_Css; // 国家货币符号

            [JsonProperty("PbSmny")]
            public double Os_Mny; // 提现金额

            [JsonProperty("PbSnnm")]
            public string Os_Nnm; // 用户昵称
        }
    }

    /// <summary>
    /// 提现用户列表 请求
    /// </summary>
    public void Request_WithdrawalUserListRequest(OceanShineWithdrawalListRequest request,
        Action<FailHttpResponse<OceanShineWithdrawalUserListResponse>> callback = null)
    {
        string requestParams = JsonConvert.SerializeObject(request);
        HttpUtil.RequestToServer<OceanShineWithdrawalUserListResponse>(AccountModuleCfg.order_name, requestParams,
            callback);
        SidecarTrackHttpRequest(nameof(Request_WithdrawalUserListRequest), AccountModuleCfg.order_name,
            HttpCategoryQuery, requestParams);
    }

    #endregion

    #region 提现平台 --- 可能时打开提现界面时需要的数据

    // 请求体数据模型类，包含了提现平台请求所需的字段
    [Serializable]
    public class OceanShineWithdrawalPageRequest
    {
        [JsonProperty("PbSapid")]
        public string Os_Apid; // 应用ID

        [JsonProperty("PbSusid")]
        public string Os_Usid; // 用户ID

        [JsonProperty("vn")]
        public string Os_Vn; // 应用版本
    }

    // 响应体数据模型类，包含了提现平台请求的响应字段 ---
    [Serializable]
    public class OceanShineWithdrawalPageResponse
    {
        // 用户信息部分
        [JsonProperty("PbSuso")]
        public UserInfo Os_Uso;

        // 提现规则
        [JsonProperty("PbSwr")]
        public List<WithdrawalRatio> Os_Wr;

        // 提现平台
        [JsonProperty("PbSwwf")]
        public List<WithdrawalPlatform> Os_Wwf;

        // 提现规则
        [Serializable]
        public class WithdrawalRatio
        {
            [JsonProperty("PbSad")]
            public int Os_Ady; // 提现档位 adFrequency

            [JsonProperty("PbSbe")]
            public int Os_Ben; // 开始 begin

            [JsonProperty("PbSen")]
            public int Os_End; // 结束 end

            [JsonProperty("PbSwr")]
            public double Os_Wro; // 提现比例 withdrawRatio
        }

        // 提现平台
        [Serializable]
        public class WithdrawalPlatform
        {
            [JsonProperty("PbScn")]
            public string Os_Cn; // 支付平台图标 icon

            [JsonProperty("PbSgdl")]
            public List<WithdrawalGoods> Os_Gdl;

            [JsonProperty("PbSme")]
            public string Os_Me; // 支付平台名称 name

            [JsonProperty("PbSmid")]
            public int Os_Mid; // 配置ID

            [JsonProperty("PbSmlt")]
            public double Os_Mlt; // 最低提现值 minimumLimit

            [JsonProperty("PbSxt")]
            public string Os_Et; // 支付平台描述 text

            [JsonProperty("id")]
            public int Os_Id; // 平台ID

            [Serializable]
            [Obfuz.ObfuzIgnore]
            public class WithdrawalGoods
            {
                [JsonProperty("PbSapid")]
                public string Os_Apid;

                [JsonProperty("PbScn")]
                public string Os_Cn;

                [JsonProperty("PbScrc")]
                public string Os_Crc;

                [JsonProperty("PbScrcs")]
                public string Os_Crcs;

                [JsonProperty("PbScty")]
                public string Os_Cty;

                [JsonProperty("PbSdlb")]
                public int Os_Dlb;

                [JsonProperty("PbSgdid")]
                public int Os_Gdid;

                [JsonProperty("PbSgdn")]
                public string Os_Gdn;

                [JsonProperty("PbSgds")]
                public string Os_Gds;

                [JsonProperty("PbSise")]
                public int Os_Ise;

                [JsonProperty("PbSmad")]
                public int Os_Mad;

                [JsonProperty("PbSmar")]
                public string Os_Mar;

                [JsonProperty("PbSmdid")]
                public int Os_Mdid;

                [JsonProperty("PbSme")]
                public string Os_Me;

                [JsonProperty("PbSmil")]
                public double Os_Mil;

                [JsonProperty("PbSmny")]
                public double Os_Mny;

                [JsonProperty("PbSmtid")]
                public int Os_Mtid;

                [JsonProperty("PbSnd_ba")]
                public int Os_NdBa;

                [JsonProperty("PbSnd_co")]
                public int Os_NdCo;

                [JsonProperty("PbSnts")]
                public string Os_Nts;

                [JsonProperty("PbSsrt")]
                public int Os_Srt;

                [JsonProperty("PbSsts")]
                public int Os_Sts;

                [JsonProperty("PbSulb")]
                public int Os_Ulb;

                [JsonProperty("PbSxt")]
                public string Os_Xt;
            }
        }
    }

    private OceanShineWithdrawalPageResponse withdrawalPageResponse;

    /// <summary>
    /// 提现平台/提现页面 请求
    /// </summary>
    public void Request_WithdrawalPageRequest(
        Action<FailHttpResponse<OceanShineWithdrawalPageResponse>> callback = null, bool block = true, bool useCach = true)
    {
        if (withdrawalPageResponse != null && withdrawalPageResponse.Os_Wwf.Count > 0 && useCach)
        {
            FailHttpResponse<OceanShineWithdrawalPageResponse> response = new()
            {
                success = true,
                data = withdrawalPageResponse,
            };
            callback?.Invoke(response);
            return;
        }

        callback += OnRequest;
        var request = new OceanShineWithdrawalPageRequest()
        {
            Os_Apid = DeviceInfoUtil.GetDeviceInfoDataForCloud().Os_Apid,
            Os_Usid = PlayerPrefs.GetString(m_userIdKey),
            Os_Vn = version
        };

        string requestParams = JsonConvert.SerializeObject(request);
        HttpUtil.RequestToServer<OceanShineWithdrawalPageResponse>(AccountModuleCfg.plat_from, requestParams, callback,
            block);
        SidecarTrackHttpRequest(nameof(Request_WithdrawalPageRequest), AccountModuleCfg.plat_from, HttpCategoryQuery,
            requestParams);

        void OnRequest(FailHttpResponse<OceanShineWithdrawalPageResponse> response)
        {
            if (!response.success || response.data == null)
                return;

            withdrawalPageResponse = response.data;
            if (response.data == null) return;
            // Os_Current_Uso = response.data.Os_Uso;
        }
    }

    #endregion

    #region 每日任务-现金-广告提现金

    // 请求体数据模型类，包含了每日任务-现金-广告提现金请求所需的字段
    [Serializable]
    public class Apid_Usid_Vn_Request
    {
        [JsonProperty("PbSapid")]
        public string Os_Apid; // 应用ID

        [JsonProperty("PbSusid")]
        public string Os_Usid; // 用户ID

        [JsonProperty("vn")]
        public string Os_Vn; // 应用版本
    }

    // 响应体数据模型类，包含了每日任务-现金-广告提现金请求的响应字段
    [Serializable]
    public class RoutineTaskLookAdMoneyResponse
    {
        [JsonProperty("PbSan")]
        public int Os_An; // 广告次数

        [JsonProperty("PbScss")]
        public string Os_Css; // 国家货币符号

        [JsonProperty("PbSln")]
        public int Os_Ln; // 已看次数

        [JsonProperty("PbSmy_rt")]
        public double Os_Mrt; // 卷转金额的比例

        [JsonProperty("PbSmy")]
        public double Os_My; // 可提现金额

        [JsonProperty("PbSsr")]
        public int Os_Sr; // 排序

        [JsonProperty("PbSss")]
        public int Os_Ss; // 当前状态：1未达到条件，2可提现，3已提现

        [JsonProperty("PbStid")]
        public int Os_Tid; // 任务ID

        public string GetInfo()
        {
            return $"广告次数:{Os_An}, 国家货币符号:{Os_Css}, 已看次数:{Os_Ln}, 卷转金额的比例:{Os_Mrt}, 可提现金额:{Os_My}, bsm_sr:{Os_Sr}, 当前状态：:{Os_Ss}, bsm_tid:{Os_Tid}";
        }
    }

    public RoutineTaskLookAdMoneyResponse routineTaskLookAdMoneyResponse = new();
    public List<RoutineTaskLookAdMoneyResponse> routineTaskLookAdMoneyResponses = new();

    /// <summary>
    /// 每日任务-现金-广告提现金 请求
    /// </summary>
    public void Request_RoutineTaskLookAdMoneyRequest(bool update,
        Action<FailHttpResponse<List<RoutineTaskLookAdMoneyResponse>>> resultCallback, bool block = true)
    {
        if (!update && routineTaskLookAdMoneyResponse != null && routineTaskLookAdMoneyResponse.Os_An != 0)
        {
            routineTaskLookAdMoneyResponses ??= new();
            routineTaskLookAdMoneyResponses.Clear();
            routineTaskLookAdMoneyResponses.Add(routineTaskLookAdMoneyResponse);
            FailHttpResponse<List<RoutineTaskLookAdMoneyResponse>> response = new()
            {
                success = true,
                data = routineTaskLookAdMoneyResponses
            };
            resultCallback?.Invoke(response);
            return;
        }

        resultCallback += OnRoutineTaskLookAdMoneyResponse;
        var request = new Apid_Usid_Vn_Request()
        {
            Os_Apid = DeviceInfoUtil.GetDeviceInfoDataForCloud().Os_Apid,
            Os_Usid = PlayerPrefs.GetString(m_userIdKey),
            Os_Vn = version
        };
        string requestParams = JsonConvert.SerializeObject(request);
        HttpUtil.RequestToServer<List<RoutineTaskLookAdMoneyResponse>>(AccountModuleCfg.task_list, requestParams,
            resultCallback, block);
        SidecarTrackHttpRequest(nameof(Request_RoutineTaskLookAdMoneyRequest), AccountModuleCfg.task_list,
            HttpCategoryQuery, requestParams);
    }

    private void OnRoutineTaskLookAdMoneyResponse(FailHttpResponse<List<RoutineTaskLookAdMoneyResponse>> response)
    {
        if (response.success && response.data != null)
        {
            if (response.data.Count == 0)
            {
                return;
            }

            routineTaskLookAdMoneyResponse = response.data[0];
            SaveDataUtils.GameData.userLookDailyAdCountMax = response.data[0].Os_An;
            SaveDataUtils.GameData.userLookDailyAdCount = response.data[0].Os_Ln;
            BizzaEventSystem.Emit(EventDefine.RealWithdraw.RefreshDailyMissionPageFalse);
        }
    }

    #endregion

    #region 用户信息-定制版参数 ---- 主要玩家登录时的 请求 --- 会保存用户信息 - UserInfo

    // 请求体数据模型类，包含了用户信息请求所需的字段 ---- 订单列表 一样的请求参数

    // 响应体数据模型类，包含了用户信息请求的响应字段
    [Serializable]
    public class OceanShineUserInfoResponse
    {
        // 新手奖励信息
        [JsonProperty("PbSacg")]
        public NewcomerReward Os_Acg;

        // 用户信息部分
        [JsonProperty("PbSuso")]
        public UserInfo Os_Uso;

        [Serializable]
        public class NewcomerReward
        {
            [JsonProperty("PbScom")]
            public int Os_Com; // commonMerge

            [JsonProperty("PbSnba")]
            public int Os_Nba; // 新手奖励积分数 new_balance

            [JsonProperty("PbSncc")]
            public int Os_Ncc; // 新手奖励金币 new_comer_coin

            [JsonProperty("PbSrnw")]
            public double Os_Rnw; // 新手奖励金额 newComerReward

            [JsonProperty("PbSusd")]
            public double Os_Usd; // 转 USD 比例 toUsd

            public double GetBalance()
            {
                double balance = CountryType switch
                {
                    E_CountryType.BR => Os_Rnw,
                    E_CountryType.ID => Os_Rnw,
                    E_CountryType.US => Os_Nba,
                    _ => Os_Rnw
                };

                return balance;
            }
        }
    }

    private OceanShineUserInfoResponse userInfoResponse;

    /// <summary>
    /// 用户信息 请求
    /// </summary>
    public void Request_UserInfoRequest(bool isUpdate, bool block = true,
        Action<FailHttpResponse<OceanShineUserInfoResponse>> callback = null)
    {
        if (!isUpdate && userInfoResponse != null && !string.IsNullOrEmpty(Os_Current_Uso.Os_Cty))
        {
            var response = new FailHttpResponse<OceanShineUserInfoResponse>()
            {
                success = true,
                data = userInfoResponse,
            };
            callback?.Invoke(response);
            return;
        }

        callback += OnRequest;
        OceanShineUserInfoRequest request = new OceanShineUserInfoRequest()
        {
            Os_Apid = DeviceInfoUtil.GetDeviceInfoDataForCloud().Os_Apid,
            Os_Usid = PlayerPrefs.GetString(m_userIdKey),
        };

        string requestParams = JsonConvert.SerializeObject(request);
        HttpUtil.RequestToServer<OceanShineUserInfoResponse>(AccountModuleCfg.user_info, requestParams, callback, block);
        SidecarTrackHttpRequest(nameof(Request_UserInfoRequest), AccountModuleCfg.user_info, HttpCategoryQuery,
            requestParams);

        void OnRequest(FailHttpResponse<OceanShineUserInfoResponse> response)
        {
            if (response.success && response.data != null && isUpdate)
            {
                userInfoResponse = response.data;
                Os_Current_Uso = response.data.Os_Uso;
            }

            if (response.errorCode == "1001" || response.errorCode == "400")
            {
                var args = new CommonConfirmTipsPanel.Args
                {
                    isLanguage = true,
                    des = "HTTPNetworkProblem",
                    onFinish = OnFinish,
                };
                UIModule.Instance.OpenPage(UIPageIds.CommonConfirmTipsPanel, args);

                void OnFinish()
                {
                    Application.Quit();
                }
            }
        }
    }

    /// <summary>
    /// 用户信息 响应
    /// </summary>
    public void Response_UserInfoResponse(FailHttpResponse<OceanShineUserInfoResponse> response)
    {
        LogLogger.LogVerbose(LogTag.LOG_Account, $"=== OceanShineUserInfoResponse === {response.success}");

        if (response.success)
        {
            if (!response.success || response.data == null)
                return;
            this.Os_Current_Uso = response.data.Os_Uso;
            SetCountryTypeByServer(Os_Uso.Os_Cty);
            SetLanguageByCountryType();
            BizzaEventSystem.Emit(EventDefine.Login.InitContentByCountry);
            loginUserInfoFinish = true;
        }
        else
        {
            if (retryUserInfoTimes > 0)
            {
                InitUserInfo();
                retryUserInfoTimes--;
            }
            else
            {
                UIUtils.ShowLanguageTips("AccountModule_NetWork");

                if (response.errorCode == "1001" || response.errorCode == "400")
                {
                    var args = new CommonConfirmTipsPanel.Args
                    {
                        isLanguage = true,
                        des = "HTTPNetworkProblem",
                        onFinish = OnFinish,
                    };
                    UIModule.Instance.OpenPage(UIPageIds.CommonConfirmTipsPanel, args);

                    void OnFinish()
                    {
                        Application.Quit();
                    }
                }
            }
        }

        LogLogger.LogVerbose(LogTag.LOG_Account, $"+++ OceanShineUserInfoResponse +++");
        Request_WithdrawalPageRequest(null, false);
    }

    #endregion

    #region 添加修改昵称

    // 请求体数据模型类，包含了修改昵称请求所需的字段
    [Serializable]
    public class OceanShineUserNickNameRequest
    {
        [JsonProperty("PbSapid")]
        public string Os_Apid; // 应用 ID
        [JsonProperty("PbSnnm")]
        public string Os_Nm; // 昵称
        [JsonProperty("PbSusid")]
        public string Os_Usid; // 用户 ID
    }

    // 实例
    public OceanShineUserNickNameRequest userNickNameRequest = new();

    /// <summary>
    /// 添加 / 修改昵称 请求
    /// </summary>
    public void Request_UserNickNameRequest(OceanShineUserNickNameRequest request,
        Action<FailHttpResponse<OceanShineUserNickNameRequest>> callback = null)
    {
        string requestParams = JsonConvert.SerializeObject(request);
        // 无返回体
        HttpUtil.RequestToServer(AccountModuleCfg.user_name, requestParams, callback);
        SidecarTrackHttpRequest(nameof(Request_UserNickNameRequest), AccountModuleCfg.user_name, HttpCategoryProperty,
            requestParams);
    }

    #endregion

    #region 广告上报

    // 请求体数据模型类，包含了广告日志报告请求所需的字段
    [Serializable]
    public class OceanShineAdLogReportRequest
    {
        // 公共信息部分
        [JsonProperty("PbScif")]
        public CommonInfo Os_Cnf;

        // 扩展参数部分
        [JsonProperty("PbSepm")]
        public ExtendParam Os_Epm;

        public OceanShineAdLogReportRequest()
        {
            Os_Cnf = new CommonInfo();
            Os_Epm = new ExtendParam();
        }

        public void Clear()
        {
            Os_Cnf?.Clear();
            Os_Epm?.Clear();
        }

        [Serializable]
        public class CommonInfo
        {
            [JsonProperty("PbSapid")]
            public string Os_Apid; // 应用ID

            [JsonProperty("PbSusid")]
            public string Os_Usid; // 用户ID

            [JsonProperty("vn")]
            public string Os_Vn; // 应用版本

            public void Clear()
            {
                Os_Apid = default;
                Os_Usid = default;
                Os_Vn = default;
            }
        }

        [Serializable]
        public class ExtendParam
        {
            [JsonProperty("PbSabd")]
            public string Os_Abd; // adType

            [JsonProperty("PbSadgp")]
            public string Os_Adgp; // adgroup

            [JsonProperty("PbStcid")]
            public string Os_Btid; // batchId

            [JsonProperty("PbScid")]
            public string Os_Cid; // codeId

            [JsonProperty("PbScam")]
            public string Os_Cmp; // campaign

            [JsonProperty("PbSctc")]
            public string Os_Ctc; // currencyCode

            [JsonProperty("PbSecm")]
            public string Os_Ecm; // ecpm

            [JsonProperty("PbSevt")]
            public string Os_Evt; // event

            [JsonProperty("PbSevm")]
            public string Os_EvtM; // eventMsg

            [JsonProperty("PbSmbc")]
            public string Os_Mbc; // mediaCodeId

            [JsonProperty("PbSmbp")]
            public string Os_Mbp; // mediaPlatform

            [JsonProperty("PbSnbt")]
            public string Os_Nbt; // network

            [JsonProperty("PbSptf")]
            public string Os_Ptf; // platform

            public void Clear()
            {
                Os_Abd = default;
                Os_Adgp = default;
                Os_Btid = default;
                Os_Cid = default;
                Os_Cmp = default;
                Os_Ctc = default;
                Os_Ecm = default;
                Os_Evt = default;
                Os_EvtM = default;
                Os_Mbc = default;
                Os_Mbp = default;
                Os_Nbt = default;
                Os_Ptf = default;
            }
        }

        public string ToLogString()
        {
            var sb = new StringBuilder();

            if (Os_Cnf != null)
            {
                AppendIfNotEmpty(sb, nameof(Os_Cnf.Os_Apid), Os_Cnf.Os_Apid);
                AppendIfNotEmpty(sb, nameof(Os_Cnf.Os_Usid), Os_Cnf.Os_Usid);
                AppendIfNotEmpty(sb, nameof(Os_Cnf.Os_Vn), Os_Cnf.Os_Vn);
            }

            if (Os_Epm != null)
            {
                AppendIfNotEmpty(sb, nameof(Os_Epm.Os_Abd), Os_Epm.Os_Abd);
                AppendIfNotEmpty(sb, nameof(Os_Epm.Os_Adgp), Os_Epm.Os_Adgp);
                AppendIfNotEmpty(sb, nameof(Os_Epm.Os_Btid), Os_Epm.Os_Btid);
                AppendIfNotEmpty(sb, nameof(Os_Epm.Os_Cid), Os_Epm.Os_Cid);
                AppendIfNotEmpty(sb, nameof(Os_Epm.Os_Cmp), Os_Epm.Os_Cmp);
                AppendIfNotEmpty(sb, nameof(Os_Epm.Os_Ctc), Os_Epm.Os_Ctc);
                AppendIfNotEmpty(sb, nameof(Os_Epm.Os_Ecm), Os_Epm.Os_Ecm);
                AppendIfNotEmpty(sb, nameof(Os_Epm.Os_Evt), Os_Epm.Os_Evt);
                AppendIfNotEmpty(sb, nameof(Os_Epm.Os_EvtM), Os_Epm.Os_EvtM);
                AppendIfNotEmpty(sb, nameof(Os_Epm.Os_Mbc), Os_Epm.Os_Mbc);
                AppendIfNotEmpty(sb, nameof(Os_Epm.Os_Mbp), Os_Epm.Os_Mbp);
                AppendIfNotEmpty(sb, nameof(Os_Epm.Os_Nbt), Os_Epm.Os_Nbt);
                AppendIfNotEmpty(sb, nameof(Os_Epm.Os_Ptf), Os_Epm.Os_Ptf);
            }

            return sb.ToString().TrimEnd(',', ' ');
        }

        private static void AppendIfNotEmpty(StringBuilder sb, string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                sb.Append(key).Append(": ").Append(value).Append(", ");
            }
        }
    }

    /// <summary>
    /// 广告上报 请求（无业务返回值）
    /// </summary>
    public void Request_AdLogReportRequest(E_AdType adType, string type, string evt, string MAXadId, string ecmp,
        string Mbc, string Mbp, string id,
        Action<FailHttpResponse<OceanShineAdLogReportRequest>> callback = null)
    {
        OceanShineAdLogReportRequest request = new()
        {
            Os_Cnf = new OceanShineAdLogReportRequest.CommonInfo()
            {
                Os_Apid = DeviceInfoUtil.GetDeviceInfoDataForCloud().Os_Apid,
                Os_Usid = PlayerPrefs.GetString(m_userIdKey),
                Os_Vn = version
            },
            Os_Epm = new()
            {
                Os_Abd = type,
                Os_Adgp = PlayerPrefs.GetString(adgroupkKey, ""),
                Os_Btid = id,
                Os_Cid = MAXadId,
                Os_Cmp = PlayerPrefs.GetString(campaignKey, ""),
                Os_Ctc = DeviceInfoUtil.deviceInfoDataForCloud.Os_Ctr.ToString(),
                Os_Ecm = ecmp, //(adInfo != null ? (float)(adInfo.Revenue *1000) : 0f).ToString(),
                Os_Evt = evt,
                Os_EvtM = "",
                Os_Mbc = Mbc, //adInfo != null ? adInfo.NetworkPlacement : "",
                Os_Mbp = Mbp, //adInfo != null ? adInfo.NetworkName : "",
                Os_Nbt = PlayerPrefs.GetString(networkKey, ""),
                Os_Ptf = "Max",
            }
        };

        if (string.Equals(evt, Instance.ADShow))
        {
            SaveDataUtils.GameData.totalFinishAdTimes++;
            SaveDataUtils.GameData.todayAdTimes++;
        }

        double.TryParse(ecmp, out double ecm);
        activeAdjust(ecm, type);
        string requestParams = JsonConvert.SerializeObject(request);
        LogLogger.LogVerbose(LogTag.ADReportFlow, type + "广告上报 ：" + id + " evt " + evt);
        LogLogger.LogVerbose(LogTag.ADReportFlow, ">>> OceanShineAdLogReportRequest: ");
        LogLogger.LogADPR($"{type}广告上报 ：{id} evt {evt} ecmp {ecmp} Mbc {Mbc} Mbp ;;; 具体信息 {request.ToLogString()}");

        // 无返回体
        HttpUtil.RequestToServer(AccountModuleCfg.ad_info, requestParams, callback, false);
        SidecarTrackHttpRequest(nameof(Request_AdLogReportRequest), AccountModuleCfg.ad_info, HttpCategoryEvent,
            requestParams);
    }

    private void activeAdjust(double ecpm, string eventName)
    {
        var userEcpm = m_Temp_ACM;
        var userIpu = m_Temp_AIU;
        var userShowEcpm = m_Temp_ShowECPM; // 最开始本地是一个 0
        var adShowTimes = SaveDataUtils.GameData.totalFinishAdTimes;
        LogLogger.LogAttribute($"激活归因 : adShowTimes:{adShowTimes}; userEcpm:{userEcpm}; userIpu:{userIpu}; userShowEcpm:{userShowEcpm}; ecpm:{ecpm}");
        if (ecpm >= userShowEcpm) // 当前广告的ECPM 》 本地的ECPM
        {
            LogLogger.LogAttribute($"激活归因 进入 ecpm >= userShowEcpm : {ecpm >= userShowEcpm}");
            userShowEcpm = ecpm;
            m_Temp_ShowECPM = ecpm;
        }

        if (0.0 != userEcpm && 0.0 == userIpu) // ECPM不为0 且 看广告次数为0
        {
            LogLogger.LogAttribute($"激活归因 进入 0.0 != userEcpm && 0.0 == userIpu : {0.0 != userEcpm && 0.0 == userIpu}");
            if (userShowEcpm >= userEcpm)// 本地 大于等于 服务器ECPm
            {
                LogLogger.LogAttribute($"激活归因 进入 0.0 != userEcpm && 0.0 == userIpu : {userShowEcpm >= userEcpm}");
                BizzaSdk.Attribution.AttributeStart(eventName);
            }
        }
        else if (0.0 == userEcpm && 0.0 != userIpu)
        {
            LogLogger.LogAttribute($"激活归因 进入 0.0 == userEcpm && 0.0 != userIpu : {0.0 == userEcpm && 0.0 != userIpu}");
            if (adShowTimes >= userIpu)
            {
                LogLogger.LogAttribute($"激活归因 进入 0.0 == userEcpm && 0.0 != userIpu : {adShowTimes >= userIpu}");
                BizzaSdk.Attribution.AttributeStart(eventName);
            }
        }
        else
        {
            LogLogger.LogAttribute($"激活归因 都没有进入:");
            if (userShowEcpm >= userEcpm && adShowTimes >= userIpu)
            {
                LogLogger.LogAttribute($"userShowEcpm >= userEcpm && adShowTimes >= userIpu");
                BizzaSdk.Attribution.AttributeStart(eventName);
            }
        }
    }

    #endregion

    #region 日志上报

    // 请求体数据模型类，包含了日志上报请求所需的字段
    [Serializable]
    public class OceanShineAppEventReportRequest
    {
        [JsonProperty("PbScif")]
        public CommonInfo Os_Cnf; // 公共信息部分

        [JsonProperty("PbSepm")]
        public ExtendParam Os_Epm; // 扩展参数部分

        [Serializable]
        public class CommonInfo
        {
            [JsonProperty("PbSapid")]
            public string Os_Apid; // 应用ID

            [JsonProperty("PbSusid")]
            public string Os_Usid; // 用户ID

            [JsonProperty("vn")]
            public string Os_Vn; // 应用版本
        }

        [Serializable]
        public class ExtendParam
        {
            [JsonProperty("PbSbgd")]
            public string Os_Bgd; // 页面ID

            [JsonProperty("PbSevt")]
            public string Os_Evt; // event

            [JsonProperty("PbSeve")]
            public string Os_EvtE; // eventExt
        }


    }

    public OceanShineAppEventReportRequest GetLogRequest(string bgd, string evt, string evtE)
    {
        OceanShineAppEventReportRequest request = new();
        request.Os_Cnf = new OceanShineAppEventReportRequest.CommonInfo()
        {
            Os_Apid = DeviceInfoUtil.GetDeviceInfoDataForCloud().Os_Apid,
            Os_Usid = PlayerPrefs.GetString(m_userIdKey, "NoUid"),
            Os_Vn = DeviceInfoUtil.GetDeviceInfoDataForCloud().Os_Vn,
        };
        request.Os_Epm = new OceanShineAppEventReportRequest.ExtendParam()
        {
            Os_Bgd = bgd,
            Os_Evt = evt,
            Os_EvtE = evtE,
        };
        return request;
    }

    /// <summary>
    /// 日志上报 请求（无业务返回值）
    /// </summary>
    public void Request_AppEventReportRequest(OceanShineAppEventReportRequest request,
        Action<FailHttpResponse<OceanShineAppEventReportRequest>> callback = null)
    {
        string requestParams = JsonConvert.SerializeObject(request);
        // 无返回体
        HttpUtil.RequestToServer(AccountModuleCfg.app_info, requestParams, callback);
        SidecarTrackHttpRequest(nameof(Request_AppEventReportRequest), AccountModuleCfg.app_info, HttpCategoryEvent,
            requestParams);
    }

    #endregion

    #endregion
}

// 看广告后给收益
// 提现
//    -- 真体现
//    -- 假提现

#region 加载任务


public class LoadUserDataTask : LoadingTaskBase
{
    public override LoadingTaskName TaskName => LoadingTaskName.LoadUserId;
    public override float Weight => 5f;

    public override LoadingTaskName[] Dependencies => new LoadingTaskName[]
    {
        LoadingTaskName.CheckNetwork,
    };

    public override async UniTask Execute()
    {
        LogLogger.LogInfo($"{TaskName} start");
        AccountModule.Instance.Login();

        SetProgress(0.94f);
        await UniTask.WaitUntil(() => AccountModule.Instance.LoadingFinish);

        SetProgress(1f);

        LogLogger.LogInfo($"{TaskName} end");
    }
}

#endregion


