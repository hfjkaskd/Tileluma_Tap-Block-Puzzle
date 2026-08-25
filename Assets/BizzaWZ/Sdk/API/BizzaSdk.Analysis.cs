using System.Collections.Generic;
using Bizza.Sdk;

namespace BizzaSdk
{
    /// <summary>
    /// 打点/分析 SDK 统一入口，封装 Platform.Instance.AnalysisSdk 的调用。
    /// 用法示例：
    /// Analysis.AddCommonParams();
    /// Analysis.AddParam("level_id", levelId);
    /// Analysis.SendCustomEvent("c_begin_game");
    /// Analysis.SetUserProperty("site_id", value);
    /// Analysis.UploadUserProperty();
    /// </summary>
    public static class Analysis
    {
        private static AnalysisSdkBase Sdk => Platform.Instance?.AnalysisSdk;

        private static readonly Dictionary<string, object> EventParams = new Dictionary<string, object>();
        private static readonly Dictionary<string, object> UserProperties = new Dictionary<string, object>();

        /// <summary> 打点 SDK 是否已初始化 </summary>
        public static bool Inited => Sdk != null;

        #region 事件打点

        /// <summary> 添加本次事件的参数（与 AddCommonParams 一起使用，在 SendCustomEvent 后会被清空） </summary>
        public static void AddParam(string key, object value)
        {
            EventParams[key] = value;
        }

        /// <summary> 添加通用参数（如 appcode 等） </summary>
        private static void AddCommonParams()
        {
            if (ChannelConfig.Instance?.tkConfig == null) return;
            EventParams["appcode"] = ChannelConfig.Instance.tkConfig.appCode;
        }

        /// <summary> 发送自定义事件。eventParams 为 null 时使用 AddParam/AddCommonParams 累积的参数字典并发送后清空 </summary>
        public static void SendCustomEvent(string eventName, Dictionary<string, object> eventParams = null)
        {
            if (Sdk == null) return;
            if (eventParams != null)
            {
                Sdk.SendCustomEvent(eventName, eventParams);
                return;
            }
            var @params = new Dictionary<string, object>(EventParams);
            if (ChannelConfig.Instance?.tkConfig != null)
                @params["appcode"] = ChannelConfig.Instance.tkConfig.appCode;
            EventParams.Clear();
            Sdk.SendCustomEvent(eventName, @params);
        }

        #endregion

        #region 用户属性

        /// <summary> 设置用户属性（会累积，在 UploadUserProperty 时一并上报） </summary>
        public static void SetUserProperty(string key, object value)
        {
            UserProperties[key] = value;
            Sdk?.SetUserProperty(key, value);
        }

        /// <summary> 上报已设置的用户属性并清空本地缓存 </summary>
        public static void UploadUserProperty()
        {
            if (Sdk == null) return;
            var copy = new Dictionary<string, object>(UserProperties);
            UserProperties.Clear();
            Sdk.UploadUserProperty(copy);
        }

        #endregion
    }
}
