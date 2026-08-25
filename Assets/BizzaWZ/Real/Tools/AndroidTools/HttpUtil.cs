#if BIZZA_REAL_WITHDRAW
using System;
using System.Collections.Generic;
using System.Text;
using Bizza.Sdk;
using Bizza.Unity.Android;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

[Obfuz.ObfuzIgnore]
public static class HttpUtil
{
    public static long serverTime = 0;
    public static string accountToken;

    private static readonly List<float> HTTP_TIMEOUT_SECONDS = new List<float> { 3f, 5f, 8f };
    private const int MAX_RETRY_COUNT = 3;
    private const int RETRY_DELAY_MS = 300;

    const string ClassName = "com.bangsawan.oceanshine.HttpUtil";
    const string SendHttpMethodName = "SendHTTPInfo";

    private static Dictionary<int, string> HTTPReply = new();
    private static int HttpId;
    private static DateTime CurrentTime => DateTime.Now;

    private static readonly JsonSerializerSettings JsonSetting = new()
    {
        NullValueHandling = NullValueHandling.Ignore,
    };

    public static HttpDataConfig HttpDataConfig
    {
        get
        {
            if (!ChannelConfig.Instance)
            {
                LogLogger.LogVerbose(LogTag.LOG_HTTP, "ChannelConfig.Instance is null");
                return null;
            }
            return ChannelConfig.Instance.httpConfig;
        }
    }

    #region Http Request Params

    private static readonly Dictionary<string, object> requestParams = new();

    public static void AddParam(string key, object data) => requestParams[key] = data;

    private static string SerializeRequestParams() => JsonConvert.SerializeObject(requestParams);

    #endregion

    #region Public APIs

    public static void RequestToServer<T>(
        string path,
        string requestJson,
        Action<FailHttpResponse<T>> response,
        bool block = false,
        float delay = 0,
        string method = "POST")
    {
#if BIZZA_REAL_WITHDRAW && !BIZZA_ENABLE_MAX
        return;
#endif
        LogLogger.LogHttpInfo("发送", path, requestJson);
        LogLogger.LogInfo(LogTag.LOG_HTTP, $"向 {path}； 数据为：{requestJson}； 时间为 {CurrentTime}");
        byte[] bodyRaw = BuildEncryptedBody(requestJson);
        RequestToServerCoroutine(path, bodyRaw, response, delay, block, method).Forget(ex => LogLogger.LogVerbose(LogTag.LOG_HTTP, $"Forget exception: {ex}"));
    }

    public static void RequestToServer(
        string path,
        string requestJson,
        Action<bool, string> response,
        bool block = false,
        float delay = 0,
        string method = "POST")
    {
#if BIZZA_REAL_WITHDRAW && !BIZZA_ENABLE_MAX
        return;
#endif
        LogLogger.LogHttpInfo("发送", path, requestJson);
        LogLogger.LogInfo(LogTag.LOG_HTTP, $"向 {path}； 数据为：{requestJson}； 时间为 {CurrentTime}");
        byte[] bodyRaw = BuildEncryptedBody(requestJson);
        RequestToServerCoroutine(path, bodyRaw, response, delay, block, method).Forget(ex => LogLogger.LogVerbose(LogTag.LOG_HTTP, $"Forget exception: {ex}"));
    }

    #endregion

    #region Coroutine - Generic (T)

    private static async UniTask RequestToServerCoroutine<T>(
        string path,
        byte[] bodyRaw,
        Action<FailHttpResponse<T>> response,
        float delay,
        bool block,
        string method)
    {
#if BIZZA_REAL_WITHDRAW && !BIZZA_ENABLE_MAX
        return;
#endif
        string url = BuildUrl(path);
        LogLogger.LogVerbose(LogTag.LOG_HTTP, $"URL ::: {url}");

        void Reply(bool success, T data, string msg, string code)
        {
            response?.Invoke(new FailHttpResponse<T>
            {
                success = success,
                data = data,
                message = msg,
                errorCode = code
            });
        }

        void ReplyDefault(string msg = "Network Error", string code = "-1")
            => Reply(false, default, msg, code);

        if (block) LoadingBlock.AddBlock(nameof(HttpUtil));

        try
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            int id = HttpId++;
            HTTPReply[id] = string.Empty;
            LogLogger.LogVerbose(LogTag.LOG_HTTP, $"URL ::: {url} + ; id :{id} ; ");
            JavaBridgeUtils.CallStaticMethodCacheClass(ClassName, SendHttpMethodName, url, ToSByteArray(bodyRaw), id, delay);
            await UniTask.WaitUntil(() =>
            {
                return HTTPReply.TryGetValue(id, out var value)
                       && value is string s
                       && !string.IsNullOrEmpty(s);
            });

            string responseJson = HTTPReply[id];
            HTTPReply.Remove(id);
            if (string.IsNullOrEmpty(responseJson))
            {
                response?.Invoke(default);
                ReplyDefault("Network Error", "NETWORK_ERROR");
                return;
            }
            string respOuterJson = responseJson; // request.downloadHandler.text;
#endif
#if !UNITY_ANDROID || UNITY_EDITOR
            var request = await SendWithRetry(() => CreateRequest(url, method, bodyRaw), url, MAX_RETRY_COUNT);
            if (request == null)
            {
                response?.Invoke(default);
                ReplyDefault("Network Error", "NETWORK_ERROR");
                return;
            }

            using (request)
            {
                if (!IsNetworkSuccess(request))
                {
                    LogLogger.LogVerbose(LogTag.LOG_HTTP, $"HTTP Network Error: {request.error}");
                    ReplyDefault(request.error, "NETWORK_ERROR");
                    return;
                }
                string respOuterJson = request.downloadHandler.text;
#endif
                // ===== 解析 outer =====

                LogLogger.LogVerbose(LogTag.LOG_HTTP, $"HTTP 返回 raw: url={respOuterJson}");

                if (!TryParseOuter(respOuterJson, out HttpResponse<string> outer))
                {
                    ReplyDefault("Invalid Response", "PARSE_ERROR");
                    return;
                }

                serverTime = outer.st;

                // ✅ 保持你原语义：业务失败也当 success=true，但 data=default
                if (outer.code != "200")
                {
                    LogLogger.LOGHTTPInfo($"HTTP!=200: {outer.Message} - {outer.code}");
                    Reply(true, default, outer.Message, outer.code);
                    return;
                }

                // ===== 解密 =====
                if (!TryDecryptOuterData(outer.data, out string plainJson))
                {
                    ReplyDefault("Decrypt Error", "DECRYPT_ERROR");
                    return;
                }
                LogLogger.LogHttpInfo("接收", path, plainJson);
                LogLogger.LogInfo(LogTag.LOG_HTTP, "服务器最后结果 " + url + " --- " + plainJson);


                try
                {
                    // ===== 反序列化 =====
                    T dataObj = JsonConvert.DeserializeObject<T>(plainJson, JsonSetting);
                    LogLogger.LogInfo(LogTag.LOG_HTTP, $"反序列化成功: {JsonConvert.SerializeObject(dataObj)}");
                    Reply(true, dataObj, outer.Message, outer.code);
                }
                catch (Exception e)
                {
                    // 打印泛型类型信息
                    LogLogger.LOGHTTPInfo($"HTTP回调异常: {typeof(T).FullName}; 异常类型: {e.GetType().FullName}; 异常信息: {e.Message}; JSON 内容: {plainJson}");
                    // 打印堆栈，帮助定位
                    LogLogger.LOGHTTPInfo($"堆栈: {e.StackTrace}");
                    ReplyDefault("Deserialize Error", "DESERIALIZE_ERROR");
                }
#if !UNITY_ANDROID || UNITY_EDITOR
            }
#endif
        }
        finally
        {
            if (block) LoadingBlock.RemoveBlock(nameof(HttpUtil));
        }
    }

    #endregion

    #region Coroutine - Raw JSON

    private static async UniTask RequestToServerCoroutine(
        string path,
        byte[] bodyRaw,
        Action<bool, string> response,
        float delay,
        bool block,
        string method)
    {
#if BIZZA_REAL_WITHDRAW && !BIZZA_ENABLE_MAX
        return;
#endif
        string url = BuildUrl(path);
        LogLogger.LogVerbose(LogTag.LOG_HTTP, $"URL ::: {url}");

        if (block) LoadingBlock.AddBlock(nameof(HttpUtil));

        try
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            int id = HttpId++;
            HTTPReply[id] = string.Empty;
            LogLogger.LogVerbose(LogTag.LOG_HTTP, $"URL ::: {url} + ; id :{id} ; ");
            JavaBridgeUtils.CallStaticMethodCacheClass(ClassName, SendHttpMethodName, url, ToSByteArray(bodyRaw), id, delay);
            await UniTask.WaitUntil(() =>
            {
                return HTTPReply.TryGetValue(id, out var value)
                       && value is string s
                       && !string.IsNullOrEmpty(s);
            });

            string responseJson = HTTPReply[id];
            HTTPReply.Remove(id);
            if (string.IsNullOrEmpty(responseJson))
            {
                response?.Invoke(false, null);
                // ReplyDefault("Network Error", "NETWORK_ERROR");
                return;
            }
            string respOuterJson = responseJson;
#endif

#if !UNITY_ANDROID || UNITY_EDITOR
            // // ✅ 超时 + 最大重试 3 次
            var request = await SendWithRetry(() => CreateRequest(url, method, bodyRaw), url, MAX_RETRY_COUNT);
            if (request == null)
            {
                response?.Invoke(false, null);
                return;
            }

            using (request)
            {
                if (!IsNetworkSuccess(request))
                {
                    LogLogger.LogVerbose(LogTag.LOG_HTTP, $"HTTP Network Error: {request.error}");
                    response?.Invoke(false, null);
                    return;
                }
                string respOuterJson = request.downloadHandler.text;
#endif

                LogLogger.LogVerbose(LogTag.LOG_HTTP, $"HTTP raw: {respOuterJson}");

                if (!TryParseOuter(respOuterJson, out HttpResponse<string> outer))
                {
                    response?.Invoke(false, null);
                    return;
                }

                serverTime = outer.st;

                // ✅ 保持你原语义：业务失败 => false,null
                if (outer.code != "200")
                {
                    response?.Invoke(false, null);
                    return;
                }

                if (!TryDecryptOuterData(outer.data, out string plainJson))
                {
                    response?.Invoke(false, null);
                    return;
                }

                response?.Invoke(true, plainJson);
#if !UNITY_ANDROID || UNITY_EDITOR
            }
#endif
        }
        finally
        {
            if (block) LoadingBlock.RemoveBlock(nameof(HttpUtil));
        }
    }

    #endregion

    #region Retry + Timeout Core

    /// <summary>
    /// 发送请求：每次尝试都带超时控制，最多重试 maxRetry 次。
    /// 成功则返回那一次成功的 request（由调用方 using dispose），失败返回 null。
    /// </summary>
    private static async UniTask<UnityWebRequest> SendWithRetry(
        Func<UnityWebRequest> requestFactory,
        string url,
        int maxRetry)
    {
#if BIZZA_REAL_WITHDRAW && !BIZZA_ENABLE_MAX
        return null;    
#endif
        for (int attempt = 1; attempt <= maxRetry; attempt++)
        {
            var request = requestFactory();
            var time = HTTP_TIMEOUT_SECONDS[attempt - 1];
            bool sentOk = await SendOnceWithTimeout(request, url, time);

            // sentOk=false 表示：超时/异常（可重试）
            // sentOk=true 但 result!=Success 表示：网络层失败（也可重试）
            if (sentOk && request.result == UnityWebRequest.Result.Success)
                return request;

            LogLogger.LogVerbose(LogTag.LOG_HTTP,
                $"HTTP attempt {attempt}/{maxRetry} failed: url={url}, result={request.result}, error={request.error}");

            request.Dispose();

            if (attempt < maxRetry)
            {
                await UniTask.Delay(RETRY_DELAY_MS);
            }
            else
            {
                UIUtils.ShowTips(LanguageUtils.GetText("Tips_NetworkError"));
                LogLogger.LogVerbose(LogTag.LOG_HTTP,
                    $"重试超出次数  - 失败 failed: url={url}, result=  error=");
            }
        }

        return null;
    }

    /// <summary>
    /// 只发送一次请求，并带超时控制（不负责重试）。
    /// </summary>
    private static async UniTask<bool> SendOnceWithTimeout(UnityWebRequest request, string url, float time)
    {
        using var cts = new System.Threading.CancellationTokenSource(
            TimeSpan.FromSeconds(time)
        );

        try
        {

            await request.SendWebRequest().ToUniTask(cancellationToken: cts.Token);
            return true;
        }
        catch (OperationCanceledException)
        {
            LogLogger.LogVerbose(LogTag.LOG_HTTP, $"HTTP Timeout ({HTTP_TIMEOUT_SECONDS}s): {url}");
            request.Abort(); // 可保留
            return false;
        }
        catch (Cysharp.Threading.Tasks.UnityWebRequestException e)
        {
            LogLogger.LogVerbose(LogTag.LOG_HTTP, $"UnityWebRequestException: {e.Message} url={url}");
            return false;
        }
        catch (Exception e)
        {
            LogLogger.LogVerbose(LogTag.LOG_HTTP, $"Request Exception: {e} url={url}");
            return false;
        }
    }

    #endregion

    #region Helpers

    private static string BuildUrl(string path)
    {
        var domain = ChannelConfig.Instance.httpConfig.domain.TrimEnd('/');
        var p = path?.TrimStart('/') ?? string.Empty;
        return $"{domain}/{p}";
    }

    private static UnityWebRequest CreateRequest(string url, string method, byte[] bodyRaw)
    {
        var request = new UnityWebRequest(url, method)
        {
            downloadHandler = new DownloadHandlerBuffer(),
            uploadHandler = new UploadHandlerRaw(bodyRaw ?? Array.Empty<byte>())
        };

        request.SetRequestHeader("X-Bundle", Application.identifier);
        if (!string.IsNullOrEmpty(accountToken))
            request.SetRequestHeader("access_token", accountToken);

        return request;
    }

    private static bool IsNetworkSuccess(UnityWebRequest request)
        => request.result == UnityWebRequest.Result.Success;

    private static bool TryParseOuter(string json, out HttpResponse<string> outer)
    {
        try
        {
            outer = JsonConvert.DeserializeObject<HttpResponse<string>>(json, JsonSetting);
            return true;
        }
        catch (Exception e)
        {
            LogLogger.LogVerbose(LogTag.LOG_HTTP, $"Outer JSON 解析失败: {e}" + json);
            outer = default;
            return false;
        }
    }

    private static byte[] BuildEncryptedBody(string plainJson)
    {
        byte[] cipherBytes = EncryptionUtil.EncryptByXXTeA(plainJson, ChannelConfig.Instance.httpConfig.aes_key);
        string signBase64 = Convert.ToBase64String(cipherBytes);
        var wrapper = new BaseRequestDto { sign = signBase64 };
        string wrapperJson = JsonConvert.SerializeObject(wrapper);
        return Encoding.UTF8.GetBytes(wrapperJson);
    }

    private static bool TryDecryptOuterData(string base64Cipher, out string plainJson)
    {
        try
        {
            byte[] cipherBytes = Convert.FromBase64String(base64Cipher);
            plainJson = EncryptionUtil.DecryptByXXTeA(cipherBytes, ChannelConfig.Instance.httpConfig.aes_key);
            return true;
        }
        catch (Exception e)
        {
            LogLogger.LogVerbose(LogTag.LOG_HTTP, $"解密失败: {e}");
            plainJson = null;
            return false;
        }
    }



    #endregion

    #region DTOs

    [Serializable]
    public struct HttpResponse<T>
    {
        public string code;
        public string msg;
        public string message;
        public string uid;
        public long st;
        public T data;

        public string Message => !string.IsNullOrEmpty(message) ? message : msg;
    }

    [Serializable]
    public class BaseRequestDto
    {
        public string sign;
    }

    #endregion

    #region 图片下载

    /// <summary>
    /// 下载图片
    /// </summary>
    /// <param name="imgPath"></param>
    /// <param name="onDownloadFinish"></param>
    public static void RequestDownloadRawImage(string imgPath, Action<Texture> onDownloadFinish)
    {
        DownloadingRawImage(imgPath, onDownloadFinish).Forget();
    }

    public static async UniTask DownloadingRawImage(string imgPath, Action<Texture> onDownloadFinish)
    {
        using (UnityWebRequest request = new UnityWebRequest(imgPath))
        {
            // 创建 DownloadHandlerTexture 实例
            DownloadHandlerTexture textDownload = new DownloadHandlerTexture(true);
            request.downloadHandler = textDownload;

            try
            {
                // 发送请求并等待其完成
                await request.SendWebRequest().ToUniTask();

                // 检查请求是否成功
                if (request.result == UnityWebRequest.Result.Success)
                {
                    // 下载完成后调用回调函数
                    onDownloadFinish?.Invoke(textDownload.texture);
                }
                else
                {
                    LogLogger.LogVerbose(LogTag.LOG_HTTP, $"下载图片失败: {request.error}");
                    onDownloadFinish?.Invoke(null); // 如果下载失败，可以传递 `null`
                }
            }
            catch (Exception ex)
            {
                LogLogger.LogVerbose(LogTag.LOG_HTTP, $"下载图片时发生异常: {ex.Message}");
                onDownloadFinish?.Invoke(null); // 如果发生异常，也传递 `null`
            }
        }
    }

    #endregion


    public static void LoginHTTPDict(int id, string json)
    {
        if (HTTPReply.ContainsKey(id))
        {
            HTTPReply[id] = json;
        }
        else
        {
            LogLogger.LogError("出现问题，没有提前注册到字典中的值");
        }
    }

    static sbyte[] ToSByteArray(byte[] bytes)
    {
        var sbytes = new sbyte[bytes.Length];
        Buffer.BlockCopy(bytes, 0, sbytes, 0, bytes.Length);
        return sbytes;
    }
}


[Serializable]
public struct FailHttpResponse<T>
{
    public bool success;
    public T data;
    public string message;
    public string errorCode;
}
#endif
