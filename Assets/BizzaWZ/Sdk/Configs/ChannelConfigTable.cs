using System;
using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Bizza.Sdk
{
    [Serializable]
    [Obfuz.ObfuzIgnore]
    public class ThinkingDataConfig // 数数打点
    {
        public string appId = "test";
        public string serverUrl = "https://global-receiver-ta.thinkingdata.cn";
        public string appCode = "";
        public bool debug = false;
    }

    [CreateAssetMenu(fileName = "ChannelConfigTable", menuName = "BizzaGame/ChannelConfigTable")]
    public class ChannelConfigTable : ScriptableObject
    {
        public const string folderPath = "Bizza/ChannelConfig/";
        public const string resourcesPath = folderPath + nameof(ChannelConfigTable);
        
#if UNITY_EDITOR
        [LabelText("渠道号")]
        [ValueDropdown(nameof(GetLayerSelection))]
#endif
        public string editorChannel = "editor";
        
#if UNITY_EDITOR
        private IEnumerable GetLayerSelection()
        {
            var array = Resources.LoadAll<ChannelConfig>(ChannelConfig.ResourcesPath);
            var valueDropList = new ValueDropdownList<string>();
            foreach (var t in array)
            {
                valueDropList.Add(t.channel, t.channel);
            }
            return valueDropList;
        }
#endif
    }
    
    [Serializable]
    [Obfuz.ObfuzIgnore]
    public class HttpDataConfig
    {
        // 指定服务器的基础 URL（即域名）。这个参数通常用于构建完整的请求 URL，比如在 RequestToServer 中会根据这个 domain 参数与请求路径拼接成完整的 API URL
        [LabelText("domain")] public string domain = ""; // 然后在请求时会将这个 URL 用作目标地址

        // 记录文档的版本号。通常用于标识客户端与服务器之间的兼容性，服务器可以根据版本号做一些特定处理
        [LabelText("doc_version")] public string doc_version; // 客户端向服务器传递当前文档（应用）版本信息，服务器可能会基于此决定是否返回某些特定格式的数据

        // 存储应用的包名（Bundle Name）。这个参数通常用于标识当前应用的身份，服务器可以根据包名来区分不同的应用，或根据不同的包名返回不同的内容
        [LabelText("bundle_name")] public string bundle_name; // 应用的唯一标识符

        // 用于 AES 加密和解密的密钥。aes_key 是一个用于对请求数据进行加密、对响应数据进行解密的关键参数。此密钥会与请求的参数一起使用，确保数据在传输过程中不被泄露
        [LabelText("aes_key")] public string aes_key = ""; // 是一个 32 字节的密钥（通常是一个十六进制字符串），用于 AES 加密算法中的对称加密。注意：密钥应该保密

        // 用于生成签名（GetVerifySign 方法中使用）。salt 是一个常常与其他信息组合（如应用版本、时间戳等）一起使用，用于生成安全的请求签名。这个签名可以确保请求数据的完整性，防止被篡改
        [LabelText("salt")] public string salt = ""; // salt 作为固定的加密或签名参数，与其他动态数据（如时间戳）一起使用生成唯一的签名

        // 存储服务器返回的时间戳。服务器的时间戳通常用于同步客户端和服务器之间的时差，或者用于对请求的时效性进行验证。比如，当请求返回时，服务器会返回当前时间，客户端可以根据这个时间做一些同步处理
        [LabelText("serverTime")] private long serverTime = 0; // serverTime 在请求响应中会被赋值为服务器返回的时间，便于客户端进行时间校对

        // 存储设备的 IDFA（广告标识符）。IDFA 是 iOS 设备中的唯一标识符，通常用于广告跟踪和分析。这个值会作为请求的一部分，确保服务器能够唯一地识别当前设备
        [LabelText("idfa")] private string idfa = ""; // idfa 会在设备上获取（如 GetIDFAStringAsync 中所示），并将其与其他请求参数一起发送给服务器
    }

    [Serializable]
    [Obfuz.ObfuzIgnore]
    public class AttributeConfig
    {

    }
}