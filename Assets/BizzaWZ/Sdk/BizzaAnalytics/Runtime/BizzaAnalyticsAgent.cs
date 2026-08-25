using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class BizzaAnalyticsAgent : MonoBehaviour
{
  private const string DefaultEncryptionKeyBase64 = "xPfnb0Z1Dw6hn0HuFbGxY/RmA0MYNRupo8B/ZddLNiA=";
  private const string DefaultEncryptionKid = "k1-20260306";

  [Header("Required")]
  [SerializeField] private string appId = "";
  [SerializeField] private string ingestUrl = "https://ingest.popemusic.us";
  [SerializeField] private string ingestToken = "";

  [Header("Optional")]
  [SerializeField] private BizzaMessageFrequency messageFrequency = BizzaMessageFrequency.Default;
  [SerializeField] private bool encryptionEnabled = true;
  [SerializeField] private string encryptionKeyBase64 = DefaultEncryptionKeyBase64;
  [SerializeField] private string encryptionKid = DefaultEncryptionKid;
  [SerializeField] private bool dontDestroyOnLoad = true;
  [SerializeField] private bool autoInitialize = true;
  [SerializeField] private bool logMode = false;
  [SerializeField, HideInInspector] private bool verboseLog = false; // Legacy field, keep for old scenes.

  private static BizzaAnalyticsAgent _instance;
  private BizzaAnalyticsRuntime _runtime;

  public static BizzaAnalyticsAgent Instance => _instance;
  public bool IsReady => _runtime != null && _runtime.IsReady;
  public string LastError => _runtime?.LastError ?? string.Empty;

  private void Awake()
  {
#if BIZZA_REAL_WITHDRAW && !BIZZA_ENABLE_MAX
    return;
#endif
    if (_instance != null && _instance != this)
    {
      Debug.LogWarning("[BizzaAnalytics] Duplicate BizzaAnalyticsAgent detected and destroyed.");
      Destroy(gameObject);
      return;
    }

    _instance = this;
    if (dontDestroyOnLoad)
    {
      DontDestroyOnLoad(gameObject);
    }

    if (autoInitialize)
    {
      Initialize();
    }
  }

  private void Update()
  {
#if BIZZA_REAL_WITHDRAW && !BIZZA_ENABLE_MAX
    return;
#endif
    _runtime?.Tick(Time.unscaledDeltaTime);
  }

  private void OnApplicationPause(bool pauseStatus)
  {
#if BIZZA_REAL_WITHDRAW && !BIZZA_ENABLE_MAX
    return;
#endif
    if (pauseStatus)
    {
      _runtime?.FlushNow("application_pause");
    }
  }

  private void OnApplicationQuit()
  {
#if BIZZA_REAL_WITHDRAW && !BIZZA_ENABLE_MAX
    return;
#endif
    _runtime?.FlushNow("application_quit");
  }

  private void OnDestroy()
  {
#if BIZZA_REAL_WITHDRAW && !BIZZA_ENABLE_MAX
    return;
#endif
    if (_instance == this)
    {
      _runtime?.FlushNow("agent_destroy");
      _instance = null;
    }
  }

  public bool Initialize()
  {
#if BIZZA_REAL_WITHDRAW && !BIZZA_ENABLE_MAX
    return false;
#endif
    if (_runtime != null)
    {
      return _runtime.IsReady;
    }

    var config = new BizzaAnalyticsConfig
    {
      appId = appId,
      ingestUrl = ingestUrl,
      ingestToken = ingestToken,
      messageFrequency = messageFrequency,
      encryptionEnabled = encryptionEnabled,
      encryptionKeyBase64 = encryptionKeyBase64,
      encryptionKid = encryptionKid,
      verboseLog = logMode || verboseLog,
    };

    _runtime = new BizzaAnalyticsRuntime(this, config);
    return _runtime.IsReady;
  }

  public void BTrack(string eventName, Dictionary<string, object> data)
  {
#if BIZZA_REAL_WITHDRAW && !BIZZA_ENABLE_MAX
    return;
#endif
    EnsureRuntime();
    _runtime?.Track(eventName, data);
  }

  public void BUserProp(Dictionary<string, object> data)
  {
#if BIZZA_REAL_WITHDRAW && !BIZZA_ENABLE_MAX
    return;
#endif
    EnsureRuntime();
    _runtime?.UpdateUserProperties(data);
  }

  public static void Track(string eventName, Dictionary<string, object> data = null)
  {
#if BIZZA_REAL_WITHDRAW && !BIZZA_ENABLE_MAX
    return;
#endif
    if (_instance == null)
    {
      Debug.LogWarning("[BizzaAnalytics] BizzaAnalyticsAgent not found in scene.");
      return;
    }

    _instance.BTrack(eventName, data);
  }

  public static void UserProp(Dictionary<string, object> data)
  {
#if BIZZA_REAL_WITHDRAW && !BIZZA_ENABLE_MAX
    return;
#endif
    if (_instance == null)
    {
      Debug.LogWarning("[BizzaAnalytics] BizzaAnalyticsAgent not found in scene.");
      return;
    }
    _instance.BUserProp(data);
  }

  internal Coroutine RunCoroutine(System.Collections.IEnumerator routine)
  {
    return StartCoroutine(routine);
  }

  private void EnsureRuntime()
  {
#if BIZZA_REAL_WITHDRAW && !BIZZA_ENABLE_MAX
    return;
#endif
    if (_runtime == null)
    {
      Initialize();
    }
  }
}
