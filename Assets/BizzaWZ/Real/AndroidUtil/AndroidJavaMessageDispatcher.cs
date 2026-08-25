#if BIZZA_REAL_WITHDRAW
using System;
using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Obfuz;

public static partial class FrameEvent
{
    public static partial class Android
    {
        public static readonly GameEvent<JObject> UnityJavaStringMessage = new();
        
        public static readonly GameEvent<int> UnityJavaObjectRootMessage = new();
        
        public static readonly GameEvent<int> UnityJavaObjectSimCardMessage = new();
        
        public static readonly GameEvent<string> UnityJavaObjectGoogleMessage = new();
    }
}

namespace Bizza.Unity.Android
{
     
    public class AndroidJavaMessageDispatcher : MonoBehaviour
    {
        public const string MessageName = nameof(MessageName);
        
        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            gameObject.name = nameof(AndroidJavaMessageDispatcher);
        }

        [ObfuzIgnore(ObfuzScope.MethodName)]
        public void DispatchMessage(string args)
        {
            JObject json = JObject.Parse(args);
            LogLogger.LogVerbose(LogTag.LOG_Device, "OnUnityJavaStringMessage:::" + json);
            BizzaEventSystem.Emit(FrameEvent.Android.UnityJavaStringMessage, json);
        }

        [ObfuzIgnore(ObfuzScope.MethodName)]
        public void DispatchRootMessage(string args)
        {
            JObject json = JObject.Parse(args);
            string message = (string)json[AndroidJavaMessageDispatcher.MessageName];
            int.TryParse(message, out int rootId);
            LogLogger.LogVerbose(LogTag.LOG_Device, "Root权限 Java 回调:::" + json + " rootId:" + rootId);
            BizzaEventSystem.Emit(FrameEvent.Android.UnityJavaObjectRootMessage, rootId);
        }
        
        [ObfuzIgnore(ObfuzScope.MethodName)]
        public void DispatchsimCardMessage(string args)
        {
            JObject json = JObject.Parse(args);
            string message = (string)json[AndroidJavaMessageDispatcher.MessageName];
            int.TryParse(message, out int rootId);
            LogLogger.LogVerbose(LogTag.LOG_Device, "OnUnityJavaStringMessage:::" + json + " simCard:" + rootId);
            BizzaEventSystem.Emit(FrameEvent.Android.UnityJavaObjectSimCardMessage, rootId);
        }
        
        [ObfuzIgnore(ObfuzScope.MethodName)]
        public void DispatchGoogleIdMessage(string args)
        {
            JObject json = JObject.Parse(args);
            string message = (string)json[AndroidJavaMessageDispatcher.MessageName];
            
            LogLogger.LogVerbose(LogTag.LOG_Device, "谷歌ID Java 回调:::" + json + " GoogleId:" + message);
            BizzaEventSystem.Emit(FrameEvent.Android.UnityJavaObjectGoogleMessage, message);
        }

        [ObfuzIgnore(ObfuzScope.MethodName)]
        public void DispatchHTTPMessage(string args)
        {
            LogLogger.LogVerbose(LogTag.LOG_HTTP, "Java方法回调 -- " + DateTime.Now + "call" + args);
            JObject json = JObject.Parse(args);

            // id
            int id = json["id"]?.Value<int>() ?? -1;
            if (id < 0)
            {
                return;
            }

            // data（Java 侧 success / error 都放在 data）
            string data = json["data"]?.ToString();

            // 写回等待中的请求
            HttpUtil.LoginHTTPDict(id, data);
        }

        [Button("Test")]
        public void Test()
        {
            // LogLogger.LogVerbose(BaseConst.LOG_Android,"LanguageUtils.SelectedLanguage " + LanguageUtils.SelectedLanguage);
        
            // LogLogger.LogVerbose(BaseConst.LOG_Android,"Account " + AccountModule.CountryType);
        }
        
    }
}
#endif