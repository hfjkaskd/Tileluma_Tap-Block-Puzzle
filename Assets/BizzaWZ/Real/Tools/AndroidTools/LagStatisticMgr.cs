#if BIZZA_REAL_WITHDRAW
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

public struct LagSnapshot
{
    public string type;
    public int count;
    public float totalTime;
    public float maxTime;
}

internal struct LagPersistentStats
{
    public int reportCount;
    public int totalSingleLagCount;
    public float totalSingleLagTime;
    public int totalContinuousLagCount;
    public float totalContinuousLagTime;
    public int totalAverageLagCount;
    public float totalAverageLagTime;
}

internal static class LagSnapshotTypes
{
    public const string Single = "Single";
    public const string Continuous = "Continuous";
    public const string Average = "Average";
}

public class LagStatisticMgr : InstanceMono<LagStatisticMgr>
{
    private const string PlayerPrefsKeyPrefix = "LagStatistic.";
    private const string ReportCountKey = PlayerPrefsKeyPrefix + "ReportCount";
    private const string TotalSingleLagCountKey = PlayerPrefsKeyPrefix + "TotalSingleLagCount";
    private const string TotalSingleLagTimeKey = PlayerPrefsKeyPrefix + "TotalSingleLagTime";
    private const string TotalContinuousLagCountKey = PlayerPrefsKeyPrefix + "TotalContinuousLagCount";
    private const string TotalContinuousLagTimeKey = PlayerPrefsKeyPrefix + "TotalContinuousLagTime";
    private const string TotalAverageLagCountKey = PlayerPrefsKeyPrefix + "TotalAverageLagCount";
    private const string TotalAverageLagTimeKey = PlayerPrefsKeyPrefix + "TotalAverageLagTime";

    private readonly List<LagRecorder> _recorders = new();

    [SerializeField] private string eventName = "performanceLog";

    [Header("Intervals (seconds)")]
    [SerializeField] private float reportInterval = 120f;
    [SerializeField] private float fpsSampleInterval = 0.5f;

    [Header("Lag Thresholds (seconds)")]
    [SerializeField] private float singleLagThreshold = 0.1f;
    [SerializeField] private float continuousLagThreshold = 0.05f;
    [SerializeField] private float continuousLagMinDuration = 0.2f;
    [SerializeField] private float averageLagThreshold = 0.04f;
    [SerializeField] private float averageLagMinDuration = 0.3f;

    private float _currentFps;
    private float _fpsElapsed;
    private int _fpsFrames;
    private float _reportElapsed;
    private bool _ignoreNextFrameSample;

    protected override void Awake()
    {
#if BIZZA_REAL_WITHDRAW && !BIZZA_ENABLE_MAX
        return;
#endif
        base.Awake();
        DontDestroyOnLoad(gameObject);
        InitializeRecorders();
        ResetFpsWindow();
    }

    private void Update()
    {
#if BIZZA_REAL_WITHDRAW && !BIZZA_ENABLE_MAX
        return;
#endif
        if (!ShouldCollect())
        {
            return;
        }

        EnsureRecordersCreated();

        if (_ignoreNextFrameSample)
        {
            _ignoreNextFrameSample = false;
            return;
        }

        float dt = Time.unscaledDeltaTime;
        if (dt <= 0f)
        {
            return;
        }

        UpdateFps(dt);
        UpdateRecorders(dt);
        UpdateReportTimer(dt);
    }

    public void SendEvent()
    {
#if BIZZA_REAL_WITHDRAW && !BIZZA_ENABLE_MAX
        return;
#endif
        if (!ShouldCollect())
        {
            return;
        }

        TrySendEvent();
    }

    private bool TrySendEvent()
    {
        EnsureRecordersCreated();
        if (AccountModule.Instance == null)
        {
            return false;
        }

        AccountModule accountModule = AccountModule.Instance;
        List<LagSnapshot> snapshots = CollectAllSnapshots();
        LagPersistentStats persistentStats = UpdatePersistentStats(snapshots, incrementReportCount: true);
        UpdateUserProperties(persistentStats);

        string report = BuildReportMessage(snapshots);
        accountModule.Request_AppEventReportRequest(
            accountModule.GetLogRequest(string.Empty, eventName, report));
        return true;
    }

    private void OnApplicationPause(bool pauseStatus)
    {
#if BIZZA_REAL_WITHDRAW && !BIZZA_ENABLE_MAX
        return;
#endif
        if (pauseStatus)
        {
            PersistCurrentWindowWithoutReporting();
            PlayerPrefs.Save();
            return;
        }

        ResetFpsWindow();
        _ignoreNextFrameSample = true;
    }

    private void OnApplicationQuit()
    {
#if BIZZA_REAL_WITHDRAW && !BIZZA_ENABLE_MAX
        return;
#endif
        PersistCurrentWindowWithoutReporting();
        PlayerPrefs.Save();
    }

    private void InitializeRecorders()
    {
        _recorders.Clear();
        _recorders.Add(new ContinuousLagRecorder(continuousLagThreshold, continuousLagMinDuration));
        _recorders.Add(new AverageLagRecorder(averageLagThreshold, averageLagMinDuration));
        _recorders.Add(new SingleLagRecorder(singleLagThreshold));
    }

    private void EnsureRecordersCreated()
    {
        if (_recorders.Count > 0)
        {
            return;
        }

        InitializeRecorders();
    }

    private bool ShouldCollect()
    {
        if (SaveDataUtils.GameData == null)
        {
            return false;
        }

        // Only collect device performance during the early onboarding levels.
        return SaveDataUtils.GameData.playerSelectedLv < 5;
    }

    private void UpdateFps(float dt)
    {
        _fpsElapsed += dt;
        _fpsFrames++;

        float sampleInterval = GetFpsSampleInterval();
        if (_fpsElapsed < sampleInterval)
        {
            return;
        }

        _currentFps = _fpsFrames / _fpsElapsed;
        ResetFpsWindow();
    }

    private void UpdateRecorders(float dt)
    {
        foreach (var recorder in _recorders)
        {
            recorder.Update(dt);
        }
    }

    private void UpdateReportTimer(float dt)
    {
        _reportElapsed += dt;
        if (_reportElapsed < GetReportInterval())
        {
            return;
        }

        if (TrySendEvent())
        {
            _reportElapsed = 0f;
        }
    }

    private void FlushAllRecorders()
    {
        foreach (var recorder in _recorders)
        {
            recorder.Flush();
        }
    }

    private List<LagSnapshot> CollectAllSnapshots()
    {
        FlushAllRecorders();
        var snapshots = new List<LagSnapshot>(_recorders.Count);
        foreach (var recorder in _recorders)
        {
            snapshots.Add(recorder.CollectAndReset());
        }

        return snapshots;
    }

    private void PersistCurrentWindowWithoutReporting()
    {
        if (_recorders.Count == 0)
        {
            return;
        }

        List<LagSnapshot> snapshots = CollectAllSnapshots();
        if (!HasSnapshotData(snapshots))
        {
            return;
        }

        UpdatePersistentStats(snapshots, incrementReportCount: false);
    }

    private static bool HasSnapshotData(IReadOnlyList<LagSnapshot> snapshots)
    {
        foreach (var snapshot in snapshots)
        {
            if (snapshot.count > 0 || snapshot.totalTime > 0f || snapshot.maxTime > 0f)
            {
                return true;
            }
        }

        return false;
    }

    private LagPersistentStats UpdatePersistentStats(IReadOnlyList<LagSnapshot> snapshots, bool incrementReportCount)
    {
        LagPersistentStats stats = LoadPersistentStats();
        if (incrementReportCount)
        {
            stats.reportCount++;
        }

        foreach (var snapshot in snapshots)
        {
            switch (snapshot.type)
            {
                case LagSnapshotTypes.Single:
                    stats.totalSingleLagCount += snapshot.count;
                    stats.totalSingleLagTime += snapshot.totalTime;
                    break;
                case LagSnapshotTypes.Continuous:
                    stats.totalContinuousLagCount += snapshot.count;
                    stats.totalContinuousLagTime += snapshot.totalTime;
                    break;
                case LagSnapshotTypes.Average:
                    stats.totalAverageLagCount += snapshot.count;
                    stats.totalAverageLagTime += snapshot.totalTime;
                    break;
            }
        }

        SavePersistentStats(stats);
        return stats;
    }

    private LagPersistentStats LoadPersistentStats()
    {
        return new LagPersistentStats
        {
            reportCount = PlayerPrefs.GetInt(ReportCountKey, 0),
            totalSingleLagCount = PlayerPrefs.GetInt(TotalSingleLagCountKey, 0),
            totalSingleLagTime = PlayerPrefs.GetFloat(TotalSingleLagTimeKey, 0f),
            totalContinuousLagCount = PlayerPrefs.GetInt(TotalContinuousLagCountKey, 0),
            totalContinuousLagTime = PlayerPrefs.GetFloat(TotalContinuousLagTimeKey, 0f),
            totalAverageLagCount = PlayerPrefs.GetInt(TotalAverageLagCountKey, 0),
            totalAverageLagTime = PlayerPrefs.GetFloat(TotalAverageLagTimeKey, 0f),
        };
    }

    private void SavePersistentStats(LagPersistentStats stats)
    {
        PlayerPrefs.SetInt(ReportCountKey, stats.reportCount);
        PlayerPrefs.SetInt(TotalSingleLagCountKey, stats.totalSingleLagCount);
        PlayerPrefs.SetFloat(TotalSingleLagTimeKey, stats.totalSingleLagTime);
        PlayerPrefs.SetInt(TotalContinuousLagCountKey, stats.totalContinuousLagCount);
        PlayerPrefs.SetFloat(TotalContinuousLagTimeKey, stats.totalContinuousLagTime);
        PlayerPrefs.SetInt(TotalAverageLagCountKey, stats.totalAverageLagCount);
        PlayerPrefs.SetFloat(TotalAverageLagTimeKey, stats.totalAverageLagTime);
    }

    private void UpdateUserProperties(LagPersistentStats stats)
    {
        if (stats.reportCount <= 0)
        {
            return;
        }

        float reportCount = stats.reportCount;
        BizzaSdk.Analysis.SetUserProperty("AvgSigLagCount", stats.totalSingleLagCount / reportCount);
        BizzaSdk.Analysis.SetUserProperty("AvgConLagCount", stats.totalContinuousLagCount / reportCount);
        BizzaSdk.Analysis.SetUserProperty("AvgConLagTime", stats.totalContinuousLagTime / reportCount);
        BizzaSdk.Analysis.SetUserProperty("AvgAvgLagCount", stats.totalAverageLagCount / reportCount);
        BizzaSdk.Analysis.SetUserProperty("AvgAvgLagTime", stats.totalAverageLagTime / reportCount);
    }

    private string BuildReportMessage(IReadOnlyList<LagSnapshot> snapshots)
    {
        var builder = new StringBuilder(192);
        AppendField(builder, "Device", GetDeviceInfo());
        AppendField(builder, "FPS", Mathf.RoundToInt(_currentFps).ToString(CultureInfo.InvariantCulture));

        // Single / Continuous / Average are independent lag dimensions and may overlap.
        foreach (var snapshot in snapshots)
        {
            AppendField(builder, snapshot.type + "Count", snapshot.count.ToString(CultureInfo.InvariantCulture));
            AppendField(builder, snapshot.type + "Time", FormatFloat(snapshot.totalTime));
            AppendField(builder, snapshot.type + "MaxTime", FormatFloat(snapshot.maxTime));
        }

        return builder.ToString();
    }

    private string GetDeviceInfo()
    {
        var deviceInfo = DeviceInfoUtil.GetDeviceInfoDataForCloud();
        string brand = string.IsNullOrEmpty(deviceInfo.Os_Bbd) ? SystemInfo.deviceModel : deviceInfo.Os_Bbd;
        string model = string.IsNullOrEmpty(deviceInfo.Os_Mbl) ? SystemInfo.deviceModel : deviceInfo.Os_Mbl;
        return brand + "--" + model;
    }

    private void ResetFpsWindow()
    {
        _fpsElapsed = 0f;
        _fpsFrames = 0;
    }

    private static void AppendField(StringBuilder builder, string key, string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return;
        }

        if (builder.Length > 0)
        {
            builder.Append("; ");
        }

        builder.Append(key).Append(':').Append(value);
    }

    private static string FormatFloat(float value)
    {
        return value.ToString("0.###", CultureInfo.InvariantCulture);
    }

    private float GetFpsSampleInterval()
    {
        return Mathf.Max(0.01f, fpsSampleInterval);
    }

    private float GetReportInterval()
    {
        return Mathf.Max(0.01f, reportInterval);
    }
}

public abstract class LagRecorder
{
    private readonly string _snapshotType;

    protected LagRecorder(string snapshotType)
    {
        _snapshotType = snapshotType;
    }

    public abstract void Update(float dt);
    public abstract void Flush();
    public abstract LagSnapshot CollectAndReset();

    protected LagSnapshot CreateSnapshot(int count, float totalTime, float maxTime)
    {
        return new LagSnapshot
        {
            type = _snapshotType,
            count = count,
            totalTime = totalTime,
            maxTime = maxTime,
        };
    }
}

public class ContinuousLagRecorder : LagRecorder
{
    private readonly float _continuousThreshold;
    private readonly float _segmentMinDuration;

    private float _currentSegmentTime;
    private int _lagCount;
    private float _lagTime;
    private float _maxLagTime;

    public ContinuousLagRecorder(float continuousThreshold, float segmentMinDuration)
        : base(LagSnapshotTypes.Continuous)
    {
        _continuousThreshold = continuousThreshold;
        _segmentMinDuration = segmentMinDuration;
    }

    public override void Update(float dt)
    {
        if (dt >= _continuousThreshold)
        {
            _currentSegmentTime += dt;
            return;
        }

        CompleteCurrentSegment();
    }

    public override void Flush()
    {
        CompleteCurrentSegment();
    }

    public override LagSnapshot CollectAndReset()
    {
        LagSnapshot snapshot = CreateSnapshot(_lagCount, _lagTime, _maxLagTime);
        _lagCount = 0;
        _lagTime = 0f;
        _maxLagTime = 0f;
        return snapshot;
    }

    private void CompleteCurrentSegment()
    {
        if (_currentSegmentTime < _segmentMinDuration)
        {
            _currentSegmentTime = 0f;
            return;
        }

        _lagCount++;
        _lagTime += _currentSegmentTime;
        if (_currentSegmentTime > _maxLagTime)
        {
            _maxLagTime = _currentSegmentTime;
        }

        _currentSegmentTime = 0f;
    }
}

public class SingleLagRecorder : LagRecorder
{
    private readonly float _singleThreshold;

    private int _lagCount;
    private float _lagTime;
    private float _maxLagTime;

    public SingleLagRecorder(float singleThreshold)
        : base(LagSnapshotTypes.Single)
    {
        _singleThreshold = singleThreshold;
    }

    public override void Update(float dt)
    {
        if (dt < _singleThreshold)
        {
            return;
        }

        _lagCount++;
        _lagTime += dt;
        if (dt > _maxLagTime)
        {
            _maxLagTime = dt;
        }
    }

    public override void Flush()
    {
    }

    public override LagSnapshot CollectAndReset()
    {
        LagSnapshot snapshot = CreateSnapshot(_lagCount, _lagTime, _maxLagTime);
        _lagCount = 0;
        _lagTime = 0f;
        _maxLagTime = 0f;
        return snapshot;
    }
}

public class AverageLagRecorder : LagRecorder
{
    private readonly float _averageThreshold;
    private readonly float _segmentMinDuration;

    private float _currentSegmentTime;
    private int _currentSegmentFrames;
    private int _lagCount;
    private float _lagTime;
    private float _maxLagTime;

    public AverageLagRecorder(float averageThreshold, float segmentMinDuration)
        : base(LagSnapshotTypes.Average)
    {
        _averageThreshold = averageThreshold;
        _segmentMinDuration = segmentMinDuration;
    }

    public override void Update(float dt)
    {
        // This tracks a continuous accumulated segment whose overall average dt stays above the threshold.
        // It is intentionally not a fixed-size sliding average window.
        float nextSegmentTime = _currentSegmentTime + dt;
        int nextSegmentFrames = _currentSegmentFrames + 1;
        float nextAverage = nextSegmentTime / nextSegmentFrames;

        if (nextAverage >= _averageThreshold)
        {
            _currentSegmentTime = nextSegmentTime;
            _currentSegmentFrames = nextSegmentFrames;
            return;
        }

        CompleteCurrentSegment();

        if (dt >= _averageThreshold)
        {
            _currentSegmentTime = dt;
            _currentSegmentFrames = 1;
        }
    }

    public override void Flush()
    {
        CompleteCurrentSegment();
    }

    public override LagSnapshot CollectAndReset()
    {
        LagSnapshot snapshot = CreateSnapshot(_lagCount, _lagTime, _maxLagTime);
        _lagCount = 0;
        _lagTime = 0f;
        _maxLagTime = 0f;
        return snapshot;
    }

    private void CompleteCurrentSegment()
    {
        if (_currentSegmentTime < _segmentMinDuration)
        {
            _currentSegmentTime = 0f;
            _currentSegmentFrames = 0;
            return;
        }

        _lagCount++;
        _lagTime += _currentSegmentTime;
        if (_currentSegmentTime > _maxLagTime)
        {
            _maxLagTime = _currentSegmentTime;
        }

        _currentSegmentTime = 0f;
        _currentSegmentFrames = 0;
    }
}
#endif