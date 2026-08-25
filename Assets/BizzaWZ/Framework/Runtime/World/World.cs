#define BonusGame

using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

public class World : MonoBehaviour
{
    public static World Current;

    private bool isPause;
    private int pauseCount;

    private readonly List<object> _pauseReasons = new();

    public bool IsPause
    {
        get => _pauseReasons.Count > 0;
        set
        {
            if (value)
            {
                Pause(this);
            }
            else
            {
                Resume(this);
            }
        }
    }

    public void Pause<TReason>(TReason reason) where TReason : class
    {
        _pauseReasons.Add(reason);
    }

    public void Resume<TReason>(TReason reason) where TReason : class
    {
        _pauseReasons.Remove(reason);
    }

    public void ResumeAll()
    {
        _pauseReasons.Clear();
    }

    public float UpdateTime { get; private set; }

    private void Awake()
    {
        Current = this;
        TimeScale = Time.timeScale;
    }

    #region Events

    public void AddSubSceneFinish()
    {
    }

    public void RemoveSubSceneFinish()
    {
    }

    #endregion

    #region Actor

    private readonly Dictionary<ulong, Actor> _mapActor = new();

    public bool IsValid(ulong guid)
    {
        return _mapActor.ContainsKey(guid);
    }

    public Actor GetActor(ulong guid)
    {
        _mapActor.TryGetValue(guid, out Actor actor);
        return actor;
    }

    public TActor GetActor<TActor>(ulong guid) where TActor : Actor
    {
        _mapActor.TryGetValue(guid, out var actor);
        return actor as TActor;
    }

    public T SpawnActor<T>(GameObject actorToSpawn) where T : Actor
    {
        return SpawnActor<T>(actorToSpawn, Vector3.zero);
    }

    public T SpawnActor<T>(GameObject actorToSpawn, Vector3 Position) where T : Actor
    {
        T actor = actorToSpawn.GetComponent<T>();
        return SpawnActor(actor, Position);
    }

    public T SpawnActor<T>(T actorToSpawn) where T : Actor
    {
        return SpawnActor(actorToSpawn, Vector3.zero);
    }

    public T SpawnActor<T>(T actorToSpawn, Vector3 Position) where T : Actor
    {
        if (!actorToSpawn)
        {
            return null;
        }

        T actorSpawned = Object.Instantiate(actorToSpawn, Position, Quaternion.identity);
        AddActor(actorSpawned);
        return actorSpawned;
    }

    public T SpawnActor<T>(T actorToSpawn, Transform parent, Vector3 Position) where T : Actor
    {
        if (!actorToSpawn)
        {
            return null;
        }

        T actorSpawned = Object.Instantiate(actorToSpawn, Position, Quaternion.identity, parent);
        AddActor(actorSpawned);
        return actorSpawned;
    }

    #endregion

    #region World Time

    private float _gameTime;

    public float RealTime
    {
        get { return Time.time; }
    }

    [ShowInInspector]
    public float GameTime
    {
        get { return _gameTime; }
    }

    public float TimeScale { get; set; }

    #endregion

    #region GameMode

    private GameModeBase _gameMode;

    public GameModeBase CurGameMode => _gameMode;

    public T GetGameMode<T>() where T : GameModeBase
    {
        return _gameMode as T;
    }

    public async void ChangeGameModeWithSameBattle(E_GameModeType targetGameMode, bool closeAllPage = true)
    {
        if (closeAllPage)
        {
            // UIModule.Instance.CloseAndDestroyLayerAllPage();
        }
        //
        // LoadingBlock.AddBlock(nameof(ChangeGameModeWithSameBattle));
        AssetUtils.LoadSceneWithBlock("Empty", () => {
            ChangeGameMode(targetGameMode, true, complete: () =>
        {
            LoadingBlock.RemoveBlock(nameof(ChangeGameModeWithSameBattle));
        }); });
    }

    public async void ChangeGameMode(E_GameModeType targetGameMode, bool allowSame = false, bool block = true, Action complete = null)
    {
        var prevGameMode = E_GameModeType.None;
        if (_gameMode != null)
        {
            prevGameMode = _gameMode.GameModeType;
            if (prevGameMode == targetGameMode && !allowSame)
            {
                complete?.Invoke();
                return;
            }
        }

        //if (block) LoadingBlock.AddBlock(nameof(ChangeGameMode));
        Pause(nameof(ChangeGameMode));

        UniTask<GameModeBase> task = GetGameModeByType(targetGameMode);
        await task;

        ClearActor();

        await UniTask.DelayFrame(3);

        _gameMode = SpawnActor(task.AsTask().Result);
        BizzaEventSystem.Emit(EventDefine.Frame.ChangeGameMode, targetGameMode, prevGameMode);

        Resume(nameof(ChangeGameMode));
        //if (block) LoadingBlock.RemoveBlock(nameof(ChangeGameMode));
        complete?.Invoke();
    }

    private async UniTask<GameModeBase> GetGameModeByType(E_GameModeType gameModeType)
    {
        if (GameModeConfig1.Instance == null)
        {
            LogLogger.LogVerbose(LogTag.LOG_Game,"GameModeConfig is Null");
            return null;
        }

        if (!GameModeConfig1.Instance.ConfigDic.ContainsKey(gameModeType))
        {
            LogLogger.LogVerbose(LogTag.LOG_Game,"Game Mode Config doest have this gameMode:" + gameModeType);
            return null;
        }

        var gameModeRef = GameModeConfig1.Instance.ConfigDic[gameModeType].gameModePrefab;
        return gameModeRef;
        // if (gameModeRef.IsValid())
        // {
        //     if (!gameModeRef.IsDone)
        //     {
        //         await gameModeRef.OperationHandle.Task;
        //     }
        //
        //     GameModeBase gameModeBase = (gameModeRef.Asset as GameObject).GetComponent<GameModeBase>();
        //     return gameModeBase;
        // }
        // else
        // {
        //     var handle = gameModeRef.LoadAssetAsync();
        //     await handle.Task;
        //     if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        //     {
        //         return handle.Result.GetComponent<GameModeBase>();
        //     }
        // }

        return null;
    }

    #endregion

    public List<Actor> AllActors => _mapActor.Values.ToList();
    [ShowInInspector, ReadOnly] private readonly List<IReleaseUpdate> _updaters = new();
    private readonly Dictionary<object, ListNode<object>> _mapTag = new();
    private readonly Dictionary<IReleaseUpdate, ListNode<object>> _mapUpdaterTag = new();
    private readonly SimpleList<object> _nodePool = new();
    private readonly List<int> _indexes = new();

    public void AddTag(IReleaseUpdate updater, object updaterTag)
    {
        if (updaterTag != null)
        {
            _mapTag[updaterTag] = _mapTag.GetValueOrDefault(updaterTag).AddFirst(updater, _nodePool);
            _mapUpdaterTag[updater] = _mapUpdaterTag.GetValueOrDefault(updater).AddFirst(updaterTag, _nodePool);
            // _mapTag.AddNode(updaterTag, updater, _nodePool);
            // _mapUpdaterTag.AddNode(updater, updaterTag, _nodePool);
        }
    }

    public void AddUpdater(IReleaseUpdate updater)
    {
        _updaters.Add(updater);
    }

    public void AddUpdater(IReleaseUpdate updater, object updaterTag)
    {
        AddUpdater(updater);
        AddTag(updater, updaterTag);
    }

    private void _RemoveUpdater()
    {
        for (int i = 0; i < _updaters.Count; i++)
        {
            if (_updaters[i].ReleaseTag)
            {
                _indexes.Add(i);
            }
        }

        for (int i = _indexes.Count - 1; i > -1; i--)
        {
            var updater = _updaters[_indexes[i]];
            if (_mapUpdaterTag.TryGetValue(updater, out var node))
            {
                foreach (var tagNode in node)
                {
                    _mapTag.Remove(tagNode);
                }

                _mapUpdaterTag.Remove(updater);
            }

            updater.OnReleaseUpdater();
        }

        _updaters.RemoveByIndexes(_indexes);
        _indexes.Clear();
    }

    public void RemoveUpdater(IReleaseUpdate updater)
    {
        updater.ReleaseTag = true;
    }

    public void RemoveUpdaterByTag(object updaterTag)
    {
        if (!_mapTag.TryGetValue(updaterTag, out var node) || node == null) return;
        foreach (var n in node)
        {
            if (n is IReleaseUpdate updater)
                RemoveUpdater(updater);
        }

        _nodePool.AddNodeWithClear(node);
        _mapTag.Remove(updaterTag);
    }

    public void OnUpdate(float delta)
    {
        if (!IsPause)
        {
            delta *= TimeScale;
            _gameTime += delta;

            // LitMotion.ManualMotionDispatcher.Default.Update(delta);

            UpdateTime = delta;

            for (int i = 0; i < _updaters.Count; i++)
            {
                var updater = _updaters[i];
                if (!updater.ReleaseTag)
                {
#if BIZZA_GAME_PROFILING
                    UnityEngine.Profiling.Profiler.BeginSample(updater.GetType().Name);
#endif
                    updater.OnUpdate(delta);

#if BIZZA_GAME_PROFILING
                    UnityEngine.Profiling.Profiler.EndSample();
#endif
                }
            }

            for (int i = 0; i < _updaters.Count; i++)
            {
                var updater = _updaters[i];
                if (!updater.ReleaseTag)
                {
#if BIZZA_GAME_PROFILING
                    UnityEngine.Profiling.Profiler.BeginSample(updater.GetType().Name);
#endif
                    updater.OnLateUpdate(delta);
#if BIZZA_GAME_PROFILING
                    UnityEngine.Profiling.Profiler.EndSample();
#endif
                }
            }
        }

        _RemoveUpdater();
    }

    /// <summary>
    /// 注册Actor
    /// </summary>
    /// <param name="actor"></param>
    /// <param name="worldParent"></param>
    public void AddActor(Actor actor, bool worldParent = true)
    {
#if STRICT_MODE
        if (actor == null) return;
#else
        if ((object)actor == null) return;
#endif
        if (actor.inited)
            return;

        AddUpdater(actor, actor);

        if (worldParent)
            actor.transform.SetParent(transform, false);
        actor.InitActor(this);
        _mapActor.Add(actor.GUID, actor);
    }

    public void RemoveActor(Actor actor)
    {
#if STRICT_MODE
        if (actor == null) return;
#else
        if ((object)actor == null) return;
#endif
        _mapActor.Remove(actor.GUID);
        RemoveUpdaterByTag(actor);
        actor.gameObject.SetActive(false);
    }

    public void OnGameQuit()
    {
        for (int i = _updaters.Count - 1; i > -1; i--)
        {
            _updaters[i].ReleaseTag = true;
        }
    }

    public void ClearActor()
    {
        _mapTag.Clear();

        for (int i = _updaters.Count - 1; i > -1; i--)
        {
            _updaters[i].ReleaseTag = true;
        }

        if (this != null)
        {
            for (int i = _updaters.Count - 1; i > -1; i--)
            {
                _updaters[i].OnReleaseUpdater();
            }
        }

        _updaters.Clear();
        _mapActor.Clear();
        _gameMode = null;
        Actor.ClearGuid();
    }
}