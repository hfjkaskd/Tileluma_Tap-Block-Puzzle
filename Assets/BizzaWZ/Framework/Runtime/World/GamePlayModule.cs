using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GamePlayModule : BaseGameModule, IUpdate //, ILateUpdate
{
    public bool Enabled => enabled;
    public World World { get; private set; }

    #region Game Module Event
    public override void InitGameModule()
    {
        if (m_gameModeConfig)
        {
            m_gameModeConfig.Init();
        }
        // GameObject worldGo = new GameObject("World", typeof(World), typeof(RectTransform), typeof(Canvas));
        GameObject worldGo = new GameObject("World", typeof(World));
        World = worldGo.GetComponent<World>();
        DontDestroyOnLoad(worldGo);
    }

    public override void ReleaseGameModule()
    {
        World.ClearActor();
        World = null;
    }

    public void OnUpdate(float delta)
    {
        World.OnUpdate(delta);
    }

    // public void OnLateUpdate(float delta)
    // {
    //     World.OnLateUpdate(delta);
    // }

    public override void PreReleaseGameModule()
    {
        base.PreReleaseGameModule();
        //TODO 关闭游戏前处理存档
    }

    #endregion

    #region Scene Manage

    public void AddSubScene(string sceneAsset)
    {
        StartCoroutine(AddSubSceneCoroutine(sceneAsset));
    }

    public void RemoveSubScene(string sceneAsset)
    {
        StartCoroutine(RemoveSubSceneCoroutine(sceneAsset));
    }

    private IEnumerator AddSubSceneCoroutine(string sceneAsset)
    {
        var operation = SceneManager.LoadSceneAsync(sceneAsset, LoadSceneMode.Additive);
        yield return operation;
        World.AddSubSceneFinish();
    }

    private IEnumerator RemoveSubSceneCoroutine(string sceneAsset)
    {
        var operation = SceneManager.UnloadSceneAsync(sceneAsset);
        yield return operation;
        World.RemoveSubSceneFinish();
    }

    #endregion

    #region GameMode

    [SerializeField]
    private E_GameModeType m_defaultGameMode;
    [SerializeField]
    private GameModeConfig1 m_gameModeConfig;
    public GameModeConfig1 GameModeConfig => m_gameModeConfig;
    #endregion

    public void GameStart()
    {
        World.ChangeGameMode(m_defaultGameMode);
    }
}
