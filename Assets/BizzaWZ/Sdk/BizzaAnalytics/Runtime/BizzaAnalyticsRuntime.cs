using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

public enum BizzaMessageFrequency
{
  Default = 0,
  High = 1,
}

internal sealed class BizzaAnalyticsConfig
{
  public string appId = "";
  public string ingestUrl = "";
  public string ingestToken = "";
  public BizzaMessageFrequency messageFrequency = BizzaMessageFrequency.Default;
  public bool encryptionEnabled = true;
  public string encryptionKeyBase64 = "";
  public string encryptionKid = "";
  public bool verboseLog;
}

internal sealed class BizzaAnalyticsRuntime
{
  private const string LogPrefix = "[BizzaAnalytics]";
  private const string LogColorInfo = "#66CCFF";
  private const string LogColorSuccess = "#58D68D";
  private const string LogColorWarn = "#F5B041";
  private const string LogColorError = "#EC7063";
  private const int MaxLogTextLength = 600;
  private const int Partitions = 200;
  private const int NormalRealtimeCap = 120;
  private const int OverloadRealtimeCap = 100;
  private const float HeartbeatIntervalSeconds = 30f;
  private const float FirstPhaseSeconds = 300f;
  private const float FirstPhaseFlushSeconds = 60f;
  private const float FallbackFlushSeconds = 240f;
  private const int MaxRetryCount = 8;
  private static readonly Regex IdPattern = new Regex("^[A-Za-z0-9._-]{1,128}$");

  private readonly BizzaAnalyticsAgent _host;
  private readonly BizzaAnalyticsConfig _config;
  private readonly BizzaStateStore _stateStore;
  private readonly BizzaState _state;
  private BizzaCrypto _crypto;

  private float _timeSinceHeartbeat;
  private bool _isSending;
  private Coroutine _sendCoroutine;

  public bool IsReady { get; private set; }
  public string LastError { get; private set; }

  public BizzaAnalyticsRuntime(BizzaAnalyticsAgent host, BizzaAnalyticsConfig config)
  {
    _host = host;
    _config = config;
    LastError = string.Empty;

    if (!ValidateConfig())
    {
      IsReady = false;
      return;
    }

    _stateStore = new BizzaStateStore(Application.persistentDataPath, _config.verboseLog);
    _state = _stateStore.Load();
    EnsureStableIds();
    BootstrapAutoSignals();
    SaveState();

    IsReady = true;
    LogStartupSnapshot();
    FlushNow("bootstrap");
  }

  public void Tick(float deltaTime)
  {
    if (!IsReady)
    {
      return;
    }

    if (deltaTime < 0f)
    {
      deltaTime = 0f;
    }

    var today = UtcDateToday();
    AddDailyElapsed(today, deltaTime);
    AddDailyFirstPhaseFlushElapsed(today, deltaTime);
    AddDailyFallbackFlushElapsed(today, deltaTime);
    _timeSinceHeartbeat += deltaTime;

    while (_timeSinceHeartbeat >= HeartbeatIntervalSeconds)
    {
      _timeSinceHeartbeat -= HeartbeatIntervalSeconds;
      OnHeartbeatTick();
    }

    if (GetDailyElapsed(today) <= FirstPhaseSeconds)
    {
      if (GetDailyRecordCount(today) >= 4 && GetDailyFirstPhaseFlushElapsed(today) >= FirstPhaseFlushSeconds)
      {
        _state.dailyFirstPhaseFlushElapsedByDt[today] = 0d;
        SaveState();
        FlushNow("first_five_minutes_of_day");
      }
    }
    else
    {
      if (GetDailyFallbackFlushElapsed(today) >= FallbackFlushSeconds)
      {
        _state.dailyFallbackFlushElapsedByDt[today] = 0d;
        SaveState();
        FlushNow("fallback_four_minutes_of_day");
      }
    }

    PromoteDelayedQueue(today);
    EnsureSenderRunning();
  }

  public void Track(string eventName, Dictionary<string, object> data)
  {
    if (!IsReady)
    {
      return;
    }

    if (string.IsNullOrWhiteSpace(eventName))
    {
      LogWarn("BTrack eventName is empty, ignored.");
      return;
    }

    var payload = BizzaValueUtil.NormalizeDictionary(data ?? new Dictionary<string, object>());
    LogVerbose("track(user) event=" + eventName.Trim() + " payload=" + SerializeForLog(payload));
    var record = BizzaRecord.NewEvent(eventName.Trim(), payload);
    EnqueueRealtimeRecord(record);
    IncrementDailyRecordCount(UtcDateToday());
    OnRecordEnqueued();
  }

  public void UpdateUserProperties(Dictionary<string, object> data)
  {
    if (!IsReady || data == null || data.Count == 0)
    {
      return;
    }

    var changed = 0;
    foreach (var kv in data)
    {
      if (string.IsNullOrWhiteSpace(kv.Key))
      {
        continue;
      }
      if (ApplyUserProperty(kv.Key.Trim(), kv.Value, true, "user"))
      {
        changed += 1;
      }
    }

    if (changed > 0)
    {
      OnRecordEnqueued();
    }
  }

  public void FlushNow(string reason)
  {
    if (!IsReady)
    {
      return;
    }

    if (_config.verboseLog)
    {
      var today = UtcDateToday();
      LogInfo(
        "FlushNow reason=" + reason +
        " uid=" + _state.uid +
        " player_id=" + _state.playerId +
        " today_records=" + GetDailyRecordCount(today) +
        " today_active_seconds=" + Math.Floor(GetDailyElapsed(today)) +
        " realtime_records=" + _state.realtimeBuffer.Count +
        " pending_batches=" + _state.pendingBatches.Count +
        " pending_records=" + CountQueuedRecords(_state.pendingBatches) +
        " delayed_batches=" + _state.delayedQueue.Count +
        " delayed_records=" + CountQueuedRecords(_state.delayedQueue));
    }

    MoveRealtimeToPendingBatches(true);
    PromoteDelayedQueue(UtcDateToday());
    EnsureSenderRunning();
  }

  private bool ValidateConfig()
  {
    if (_config == null)
    {
      LastError = "config_is_null";
      LogError(LastError);
      return false;
    }

    _config.appId = (_config.appId ?? string.Empty).Trim();
    if (!IdPattern.IsMatch(_config.appId))
    {
      LastError = "app_id_must_match_^[A-Za-z0-9._-]{1,128}$";
      LogError(LastError);
      return false;
    }

    _config.ingestUrl = (_config.ingestUrl ?? string.Empty).Trim().TrimEnd('/');
    if (string.IsNullOrEmpty(_config.ingestUrl) || !_config.ingestUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
    {
      LastError = "ingest_url_is_invalid";
      LogError(LastError);
      return false;
    }

    _config.ingestToken = (_config.ingestToken ?? string.Empty).Trim();
    if (string.IsNullOrEmpty(_config.ingestToken))
    {
      LastError = "ingest_token_is_empty";
      LogError(LastError);
      return false;
    }

    _config.encryptionKeyBase64 = (_config.encryptionKeyBase64 ?? string.Empty).Trim();
    _config.encryptionKid = (_config.encryptionKid ?? string.Empty).Trim();

    if (_config.encryptionEnabled)
    {
      string cryptoError;
      if (!BizzaCrypto.TryCreate(_config.encryptionKeyBase64, _config.encryptionKid, out _crypto, out cryptoError))
      {
        LastError = "encryption_config_invalid:" + cryptoError;
        LogError(LastError);
        return false;
      }
    }
    else
    {
      _crypto = null;
    }

    return true;
  }

  private void EnsureStableIds()
  {
    if (!IdPattern.IsMatch(_state.uid))
    {
      _state.uid = GenerateId("u");
    }

    if (string.IsNullOrEmpty(_state.playerId))
    {
      _state.playerId = GenerateId("player");
    }
  }

  private void BootstrapAutoSignals()
  {
    if (!_state.registerSent)
    {
      EnqueueRealtimeRecord(BizzaRecord.NewEvent("register", new Dictionary<string, object>()));
      LogVerbose("track(auto_event) event=register payload={}");
      _state.registerSent = true;
      IncrementDailyRecordCount(UtcDateToday());
    }

    EnqueueRealtimeRecord(BizzaRecord.NewEvent("login", new Dictionary<string, object>()));
    LogVerbose("track(auto_event) event=login payload={}");
    IncrementDailyRecordCount(UtcDateToday());

    ApplyUserProperty("playerID", _state.playerId, true, "auto_bootstrap");
    ApplyUserProperty("device_model", string.IsNullOrEmpty(SystemInfo.deviceModel) ? "unknown" : SystemInfo.deviceModel, true, "auto_bootstrap");

    var duration = (long)Math.Floor(_state.gameDurationSeconds);
    ApplyUserProperty("game_duration", duration, true, "auto_bootstrap");
  }

  private bool ApplyUserProperty(string key, object value, bool countAsDailyRecord, string source)
  {
    var normalizedValue = BizzaValueUtil.Normalize(value);
    object oldValue = null;
    var exists = _state.userProps.TryGetValue(key, out oldValue);
    if (exists && BizzaValueUtil.AreEqual(oldValue, normalizedValue))
    {
      return false;
    }

    _state.userProps[key] = normalizedValue;
    var record = BizzaRecord.NewUserProfileChange(
      key,
      exists ? oldValue : null,
      normalizedValue,
      DateTime.UtcNow.ToString("o")
    );
    EnqueueRealtimeRecord(record);
    LogVerbose(
      "user_prop(" + source + ") key=" + key +
      " old=" + SerializeForLog(exists ? oldValue : null) +
      " new=" + SerializeForLog(normalizedValue));

    if (countAsDailyRecord)
    {
      IncrementDailyRecordCount(UtcDateToday());
    }

    return true;
  }

  private void OnHeartbeatTick()
  {
    var oldDuration = (long)Math.Floor(_state.gameDurationSeconds);
    _state.gameDurationSeconds += HeartbeatIntervalSeconds;
    var newDuration = (long)Math.Floor(_state.gameDurationSeconds);

    if (newDuration != oldDuration)
    {
      ApplyUserProperty("game_duration", newDuration, true, "auto_heartbeat");
      OnRecordEnqueued();
    }
  }

  private void OnRecordEnqueued()
  {
    SaveState();

    var today = UtcDateToday();
    var dailyRecordCount = GetDailyRecordCount(today);
    if (dailyRecordCount <= 4)
    {
      if ((dailyRecordCount % 2) == 0)
      {
        FlushNow("first_four_records_of_day");
      }
      return;
    }

    if (GetDailyElapsed(today) > FirstPhaseSeconds)
    {
      var threshold = ResolveEffectiveBatchSize();
      if (_state.realtimeBuffer.Count >= threshold)
      {
        FlushNow("count_threshold");
      }
    }
  }

  private void EnqueueRealtimeRecord(BizzaRecord record)
  {
    _state.realtimeBuffer.Add(record);
  }

  private void MoveRealtimeToPendingBatches(bool flushAll)
  {
    if (_state.realtimeBuffer.Count == 0)
    {
      SaveState();
      return;
    }

    var batchSize = ResolveEffectiveBatchSize();
    if (batchSize < 2)
    {
      batchSize = 2;
    }

    while (_state.realtimeBuffer.Count >= batchSize)
    {
      CreatePendingRealtimeBatch(batchSize);
    }

    if (flushAll && _state.realtimeBuffer.Count > 0)
    {
      CreatePendingRealtimeBatch(_state.realtimeBuffer.Count);
    }

    SaveState();
  }

  private void CreatePendingRealtimeBatch(int takeCount)
  {
    var records = new List<BizzaRecord>();
    for (var i = 0; i < takeCount; i++)
    {
      records.Add(_state.realtimeBuffer[i]);
    }
    _state.realtimeBuffer.RemoveRange(0, takeCount);

    _state.pendingBatches.Add(new BizzaBatch
    {
      batchId = GenerateId("b"),
      channel = "realtime",
      dt = UtcDateToday(),
      originDt = "",
      earliestSendDt = UtcDateToday(),
      part = -1,
      retryCount = 0,
      nextRetryAt = "",
      records = records,
    });
  }

  private int ResolveEffectiveBatchSize()
  {
    var today = UtcDateToday();
    var baseSize = 20;
    if (GetDailyRecordCount(today) < 4)
    {
      baseSize = 2;
    }
    else if (GetDailyElapsed(today) > FirstPhaseSeconds)
    {
      baseSize = _config.messageFrequency == BizzaMessageFrequency.High ? 100 : 20;
    }

    if (IsOverloadMode(today))
    {
      baseSize = Math.Max(baseSize, 100);
    }

    var usedPartitions = GetUsedPartitions(today);
    if (usedPartitions >= 175)
    {
      baseSize *= 4;
    }
    else if (usedPartitions >= 160)
    {
      baseSize *= 2;
    }

    if (baseSize > 500) baseSize = 500;
    if (baseSize < 2) baseSize = 2;
    return baseSize;
  }

  private int GetUsedPartitions(string dt)
  {
    return GetCounter(_state.realtimeSeqByDt, dt) + GetCounter(_state.delayedSeqByDt, dt);
  }

  private void PromoteDelayedQueue(string today)
  {
#if BIZZA_REAL_WITHDRAW && !BIZZA_ENABLE_MAX
    return;
#endif
    if (_state.delayedQueue.Count == 0)
    {
      _state.delayedQueueNonEmptySince = "";
      return;
    }

    _state.delayedQueue.Sort((a, b) =>
    {
      var cmp = string.CompareOrdinal(a.originDt, b.originDt);
      if (cmp != 0) return cmp;
      return string.CompareOrdinal(a.earliestSendDt, b.earliestSendDt);
    });

    var moved = 0;
    while (_state.delayedQueue.Count > 0)
    {
      var next = _state.delayedQueue[0];
      var earliest = string.IsNullOrEmpty(next.earliestSendDt) ? today : next.earliestSendDt;
      if (string.CompareOrdinal(earliest, today) > 0)
      {
        break;
      }

      _state.delayedQueue.RemoveAt(0);
      _state.pendingBatches.Add(next);
      moved += 1;
    }

    if (_state.delayedQueue.Count == 0)
    {
      _state.delayedQueueNonEmptySince = "";
    }
    else if (string.IsNullOrEmpty(_state.delayedQueueNonEmptySince))
    {
      _state.delayedQueueNonEmptySince = today;
    }

    if (moved > 0)
    {
      SaveState();
    }
  }

  private void EnsureSenderRunning()
  {
#if BIZZA_REAL_WITHDRAW && !BIZZA_ENABLE_MAX
return;
#endif
    if (_isSending || !IsReady)
    {
      return;
    }

    if (!HasReadyBatch())
    {
      return;
    }

    _sendCoroutine = _host.RunCoroutine(SendLoop());
  }

  private bool HasReadyBatch()
  {
    var now = DateTime.UtcNow;
    for (var i = 0; i < _state.pendingBatches.Count; i++)
    {
      var batch = _state.pendingBatches[i];
      if (batch.part < 0)
      {
        return true;
      }
      if (string.IsNullOrEmpty(batch.nextRetryAt))
      {
        return true;
      }

      DateTime retryAt;
      if (!DateTime.TryParse(batch.nextRetryAt, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out retryAt))
      {
        return true;
      }
      if (now >= retryAt)
      {
        return true;
      }
    }
    return false;
  }

  private IEnumerator SendLoop()
  {
    _isSending = true;

    while (true)
    {
      var today = UtcDateToday();
      PromoteDelayedQueue(today);

      var index = FindNextReadyBatchIndex(DateTime.UtcNow);
      if (index < 0)
      {
        break;
      }

      var batch = _state.pendingBatches[index];
      if (batch.part < 0)
      {
        int part;
        if (!TryAllocatePart(batch, today, out part))
        {
          _state.pendingBatches.RemoveAt(index);
          MoveBatchToDelayedQueue(batch, today);
          SaveState();
          continue;
        }

        batch.part = part;
        batch.dt = today;
        _state.pendingBatches[index] = batch;
        SaveState();
      }

      var sent = false;
      var statusCode = 0L;
      var errorText = "";

      yield return SendBatchRequest(
        batch,
        (ok, code, err) =>
        {
          sent = ok;
          statusCode = code;
          errorText = err;
        }
      );

      if (sent)
      {
        _state.pendingBatches.RemoveAt(index);
        SaveState();
        continue;
      }

      HandleSendFailure(index, batch, today, statusCode, errorText);
      SaveState();
    }

    _isSending = false;
    _sendCoroutine = null;
  }

  private IEnumerator SendBatchRequest(BizzaBatch batch, Action<bool, long, string> onDone)
  {
    Dictionary<string, object> payload;
    byte[] bodyRaw;
    try
    {
      payload = BuildRequestPayload(batch);
      var json = BizzaJson.Serialize(payload);
      bodyRaw = Encoding.UTF8.GetBytes(json);
    }
    catch (Exception ex)
    {
      LogError(
        "send(build_failed) batch=" + batch.batchId +
        " uid=" + _state.uid +
        " part=" + batch.part +
        " channel=" + batch.channel +
        " error=" + ex.Message);
      onDone(false, 0L, "build_payload_failed:" + ex.Message);
      yield break;
    }

    using (var request = new UnityWebRequest(_config.ingestUrl + "/v1/ingest", "POST"))
    {
      if (_config.verboseLog)
      {
        LogInfo(
          "send(start) url=" + _config.ingestUrl + "/v1/ingest" +
          " batch=" + batch.batchId +
          " uid=" + _state.uid +
          " part=" + batch.part +
          " channel=" + batch.channel +
          " records=" + (batch.records == null ? 0 : batch.records.Count) +
          " bytes=" + (bodyRaw == null ? 0 : bodyRaw.Length));
      }

      request.uploadHandler = new UploadHandlerRaw(bodyRaw);
      request.downloadHandler = new DownloadHandlerBuffer();
      request.SetRequestHeader("Authorization", "Bearer " + _config.ingestToken);
      request.SetRequestHeader("Content-Type", "application/json; charset=utf-8");

      yield return request.SendWebRequest();

      var ok = request.result == UnityWebRequest.Result.Success && request.responseCode >= 200 && request.responseCode < 300;
      var responseText = request.downloadHandler != null ? request.downloadHandler.text : "";
      var error = ok ? "" : (!string.IsNullOrEmpty(request.error) ? request.error : responseText);

      if (_config.verboseLog)
      {
        var resultText =
          "send(result) batch=" + batch.batchId +
          " uid=" + _state.uid +
          " part=" + batch.part +
          " channel=" + batch.channel +
          " ok=" + ok +
          " status=" + request.responseCode +
          " err=" + LimitForLog(error) +
          " resp=" + LimitForLog(responseText);
        if (ok)
        {
          LogSuccess(resultText);
        }
        else
        {
          LogWarn(resultText);
        }
      }

      onDone(ok, request.responseCode, error);
    }
  }

  private Dictionary<string, object> BuildRequestPayload(BizzaBatch batch)
  {
    if (_config.encryptionEnabled && _crypto != null)
    {
      try
      {
        return BuildRequestPayload(batch, true);
      }
      catch (PlatformNotSupportedException ex)
      {
        DisableEncryptionFallback(ex);
      }
      catch (NotSupportedException ex)
      {
        DisableEncryptionFallback(ex);
      }
    }

    return BuildRequestPayload(batch, false);
  }

  private Dictionary<string, object> BuildRequestPayload(BizzaBatch batch, bool encryptRecords)
  {
    var records = new List<object>();
    var originDt = batch.channel == "delayed" ? batch.originDt : "";

    for (var i = 0; i < batch.records.Count; i++)
    {
      var wireRecord = batch.records[i].ToWireRecord(originDt);
      if (encryptRecords && _crypto != null)
      {
        EncryptWireRecordInPlace(wireRecord);
      }
      records.Add(wireRecord);
    }

    return new Dictionary<string, object>
    {
      { "app_id", _config.appId },
      { "uid", _state.uid },
      { "batch_id", batch.batchId },
      { "dt", string.IsNullOrEmpty(batch.dt) ? UtcDateToday() : batch.dt },
      { "part", batch.part },
      { "records", records },
    };
  }

  private void DisableEncryptionFallback(Exception ex)
  {
    _config.encryptionEnabled = false;
    _crypto = null;
    LogWarn(
      "encryption(disabled) reason=" + ex.Message +
      " fallback=plaintext platform=" + Application.platform);
  }

  private void EncryptWireRecordInPlace(Dictionary<string, object> wireRecord)
  {
    var type = BizzaValueUtil.AsString(wireRecord.ContainsKey("type") ? wireRecord["type"] : null, "");
      if (type == "event")
    {
      if (wireRecord.ContainsKey("payload"))
      {
        wireRecord["payload"] = _crypto.EncryptValue(wireRecord["payload"]);
        LogVerbose(
          "encrypt(event) event_name=" + BizzaValueUtil.AsString(wireRecord.ContainsKey("event_name") ? wireRecord["event_name"] : null, "") +
          " encrypted_payload=" + SerializeForLog(wireRecord["payload"]));
      }
      return;
    }

    if (type == "user_profile_change")
    {
      if (wireRecord.ContainsKey("old") && wireRecord["old"] != null)
      {
        wireRecord["old"] = _crypto.EncryptValue(wireRecord["old"]);
      }
      if (wireRecord.ContainsKey("new") && wireRecord["new"] != null)
      {
        wireRecord["new"] = _crypto.EncryptValue(wireRecord["new"]);
      }
      LogVerbose(
        "encrypt(user_profile_change) key=" + BizzaValueUtil.AsString(wireRecord.ContainsKey("key") ? wireRecord["key"] : null, "") +
        " encrypted_old=" + SerializeForLog(wireRecord.ContainsKey("old") ? wireRecord["old"] : null) +
        " encrypted_new=" + SerializeForLog(wireRecord.ContainsKey("new") ? wireRecord["new"] : null));
    }
  }

  private void HandleSendFailure(int pendingIndex, BizzaBatch batch, string today, long statusCode, string errorText)
  {
    if (statusCode == 409)
    {
      _state.pendingBatches.RemoveAt(pendingIndex);
      MoveBatchToDelayedQueue(batch, today);
      return;
    }

    batch.retryCount += 1;

    if (batch.retryCount > MaxRetryCount)
    {
      _state.pendingBatches.RemoveAt(pendingIndex);
      MoveBatchToDelayedQueue(batch, today);
      return;
    }

    var backoff = Math.Min(300d, Math.Pow(2d, Math.Min(8, batch.retryCount)) * 2d);
    batch.nextRetryAt = DateTime.UtcNow.AddSeconds(backoff).ToString("o");
    _state.pendingBatches[pendingIndex] = batch;

    if (_config.verboseLog)
    {
      LogWarn(
        "send(retry) batch=" + batch.batchId +
        " status=" + statusCode + " retry=" + batch.retryCount +
        " next_retry_at=" + batch.nextRetryAt + " error=" + LimitForLog(errorText)
      );
    }
  }

  private void MoveBatchToDelayedQueue(BizzaBatch batch, string today)
  {
    batch.channel = "delayed";
    if (string.IsNullOrEmpty(batch.originDt))
    {
      batch.originDt = string.IsNullOrEmpty(batch.dt) ? today : batch.dt;
    }
    batch.earliestSendDt = NextUtcDate(today);
    batch.dt = batch.earliestSendDt;
    batch.part = -1;
    batch.retryCount = 0;
    batch.nextRetryAt = "";

    _state.delayedQueue.Add(batch);
    if (string.IsNullOrEmpty(_state.delayedQueueNonEmptySince))
    {
      _state.delayedQueueNonEmptySince = today;
    }
  }

  private bool TryAllocatePart(BizzaBatch batch, string dt, out int part)
  {
    var overloadMode = IsOverloadMode(dt);
    var realtimeCap = overloadMode ? OverloadRealtimeCap : NormalRealtimeCap;
    var delayedStart = realtimeCap;
    var delayedCap = Partitions - delayedStart;

    if (batch.channel == "realtime")
    {
      var seq = GetCounter(_state.realtimeSeqByDt, dt);
      if (seq >= realtimeCap)
      {
        part = -1;
        return false;
      }

      part = seq;
      _state.realtimeSeqByDt[dt] = seq + 1;
      return true;
    }

    var delayedSeq = GetCounter(_state.delayedSeqByDt, dt);
    if (delayedSeq >= delayedCap)
    {
      part = -1;
      return false;
    }

    part = delayedStart + delayedSeq;
    _state.delayedSeqByDt[dt] = delayedSeq + 1;
    return true;
  }

  private bool IsOverloadMode(string today)
  {
    if (_state.delayedQueue.Count == 0)
    {
      return false;
    }

    var oldestOrigin = _state.delayedQueue[0].originDt;
    DateTime oldestOriginDate;
    DateTime todayDate;
    if (TryParseDate(oldestOrigin, out oldestOriginDate) && TryParseDate(today, out todayDate))
    {
      if ((todayDate - oldestOriginDate).TotalDays > 2d)
      {
        return true;
      }
    }

    if (!string.IsNullOrEmpty(_state.delayedQueueNonEmptySince))
    {
      DateTime sinceDate;
      if (TryParseDate(_state.delayedQueueNonEmptySince, out sinceDate) && TryParseDate(today, out todayDate))
      {
        if ((todayDate - sinceDate).TotalDays >= 2d)
        {
          return true;
        }
      }
    }

    return false;
  }

  private int FindNextReadyBatchIndex(DateTime nowUtc)
  {
    for (var i = 0; i < _state.pendingBatches.Count; i++)
    {
      var batch = _state.pendingBatches[i];
      if (batch.part < 0)
      {
        return i;
      }

      if (string.IsNullOrEmpty(batch.nextRetryAt))
      {
        return i;
      }

      DateTime retryAt;
      if (!DateTime.TryParse(batch.nextRetryAt, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out retryAt))
      {
        return i;
      }

      if (nowUtc >= retryAt)
      {
        return i;
      }
    }
    return -1;
  }

  private void IncrementDailyRecordCount(string dt)
  {
    _state.dailyRecordCountByDt[dt] = GetCounter(_state.dailyRecordCountByDt, dt) + 1;
  }

  private void AddDailyElapsed(string dt, float deltaTime)
  {
    if (deltaTime <= 0f)
    {
      return;
    }

    _state.dailyActiveSecondsByDt[dt] = GetDoubleCounter(_state.dailyActiveSecondsByDt, dt) + deltaTime;
  }

  private void AddDailyFirstPhaseFlushElapsed(string dt, float deltaTime)
  {
    if (deltaTime <= 0f)
    {
      return;
    }

    _state.dailyFirstPhaseFlushElapsedByDt[dt] = GetDoubleCounter(_state.dailyFirstPhaseFlushElapsedByDt, dt) + deltaTime;
  }

  private void AddDailyFallbackFlushElapsed(string dt, float deltaTime)
  {
    if (deltaTime <= 0f)
    {
      return;
    }

    _state.dailyFallbackFlushElapsedByDt[dt] = GetDoubleCounter(_state.dailyFallbackFlushElapsedByDt, dt) + deltaTime;
  }

  private int GetDailyRecordCount(string dt)
  {
    return GetCounter(_state.dailyRecordCountByDt, dt);
  }

  private double GetDailyElapsed(string dt)
  {
    return GetDoubleCounter(_state.dailyActiveSecondsByDt, dt);
  }

  private double GetDailyFirstPhaseFlushElapsed(string dt)
  {
    return GetDoubleCounter(_state.dailyFirstPhaseFlushElapsedByDt, dt);
  }

  private double GetDailyFallbackFlushElapsed(string dt)
  {
    return GetDoubleCounter(_state.dailyFallbackFlushElapsedByDt, dt);
  }

  private int GetCounter(Dictionary<string, int> map, string key)
  {
    int value;
    if (!map.TryGetValue(key, out value))
    {
      return 0;
    }
    return value;
  }

  private double GetDoubleCounter(Dictionary<string, double> map, string key)
  {
    double value;
    if (!map.TryGetValue(key, out value))
    {
      return 0d;
    }
    return value;
  }

  private void LogVerbose(string message)
  {
    if (!_config.verboseLog)
    {
      return;
    }

    LogInfo(message);
  }

  private string SerializeForLog(object value)
  {
    try
    {
      return BizzaJson.Serialize(BizzaValueUtil.Normalize(value));
    }
    catch
    {
      return value == null ? "null" : value.ToString();
    }
  }

  private static string Colorize(string color, string message)
  {
    return "<color=" + color + ">" + LogPrefix + " " + message + "</color>";
  }

  private void LogInfo(string message)
  {
    if (!_config.verboseLog)
    {
      return;
    }

    Debug.Log(Colorize(LogColorInfo, message));
  }

  private void LogSuccess(string message)
  {
    if (!_config.verboseLog)
    {
      return;
    }

    Debug.Log(Colorize(LogColorSuccess, message));
  }

  private void LogWarn(string message)
  {
    if (!_config.verboseLog)
    {
      return;
    }

    Debug.LogWarning(Colorize(LogColorWarn, message));
  }

  private void LogError(string message)
  {
    Debug.LogError(Colorize(LogColorError, message));
  }

  private static string LimitForLog(string text)
  {
    if (string.IsNullOrEmpty(text))
    {
      return "";
    }

    if (text.Length <= MaxLogTextLength)
    {
      return text;
    }

    return text.Substring(0, MaxLogTextLength) + "...(truncated)";
  }

  private void SaveState()
  {
    _stateStore.Save(_state);
  }

  private void LogStartupSnapshot()
  {
    if (!_config.verboseLog)
    {
      return;
    }

    LogSuccess("startup(app_id) " + _config.appId);
    LogSuccess("startup(current_uid) " + _state.uid);
    LogSuccess("startup(current_player_id) " + _state.playerId);
    var today = UtcDateToday();
    LogSuccess(
      "startup(buffer_state) register_sent=" + _state.registerSent +
      " today_records=" + GetDailyRecordCount(today) +
      " today_active_seconds=" + Math.Floor(GetDailyElapsed(today)) +
      " realtime_records=" + _state.realtimeBuffer.Count +
      " pending_batches=" + _state.pendingBatches.Count +
      " pending_records=" + CountQueuedRecords(_state.pendingBatches) +
      " delayed_batches=" + _state.delayedQueue.Count +
      " delayed_records=" + CountQueuedRecords(_state.delayedQueue));
    LogSuccess("startup(user_props) " + SerializeForLog(_state.userProps));
  }

  private static int CountQueuedRecords(List<BizzaBatch> batches)
  {
    if (batches == null || batches.Count == 0)
    {
      return 0;
    }

    var total = 0;
    for (var i = 0; i < batches.Count; i++)
    {
      total += batches[i].records == null ? 0 : batches[i].records.Count;
    }
    return total;
  }

  private static string GenerateId(string prefix)
  {
    var core = Guid.NewGuid().ToString("N");
    var id = prefix + "_" + core.Substring(0, 20);
    return id;
  }

  private static string UtcDateToday()
  {
    return DateTime.UtcNow.ToString("yyyy-MM-dd");
  }

  private static string NextUtcDate(string utcDate)
  {
    DateTime date;
    if (!TryParseDate(utcDate, out date))
    {
      date = DateTime.UtcNow.Date;
    }
    return date.AddDays(1).ToString("yyyy-MM-dd");
  }

  private static bool TryParseDate(string dateText, out DateTime result)
  {
    return DateTime.TryParseExact(
      dateText,
      "yyyy-MM-dd",
      CultureInfo.InvariantCulture,
      DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
      out result
    );
  }
}
