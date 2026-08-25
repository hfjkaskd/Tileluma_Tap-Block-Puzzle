using System;
using System.Linq;
using Bizza.Sdk;
using cfg;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using SRDebugger.Services;
using SRF.Service;
using UnityEngine;

public class DebugModule : BaseGameModule<DebugModule>
{
    [ShowInInspector]
    public int curFrame => Time.frameCount;

    [Button]
    void TestTransition()
    {
        UIModule.Instance.OpenPage(UIPageIds.TransitonPanel);
        GameUtils.DelayDo(() =>
        {
            UIModule.Instance.ClosePage(UIPageIds.TransitonPanel);
        }, 2);
    }

    public override void InitGameModule()
    {
#if !DEBUG_MODE && !UNITY_EDITOR
        return;
#endif
        if (ChannelConfig.Instance.GM)
        {
            SRDebug.Init();
            DelayShowAndHideGM();
        }
    }

    private async void DelayShowAndHideGM()
    {
        await UniTask.Delay(2000);
        var mgr = SRServiceManager.GetService<IDebugService>();
        if (mgr != null)
        {
            SRServiceManager.GetService<IDebugService>().ShowDebugPanel();
            SRServiceManager.GetService<IDebugService>().HideDebugPanel();
            await UniTask.Delay(2000);
            SRServiceManager.GetService<IDebugService>().ShowDebugPanel();
            SRServiceManager.GetService<IDebugService>().HideDebugPanel();
        }


    }

    public override void ReleaseGameModule()
    {
    }

    public float maxDist;
    private void OnDrawGizmos()
    {
        // if (Input.GetMouseButton(2))
        // {
        // var worldPos = GameUtils.GetMouseGroundPos(Camera.main);
        // Gizmos.DrawSphere(worldPos, 0.5f);
        // Gizmos.DrawSphere(BattleSys_PathFinding.Instance.pathFinding.GetNearestWalkablePoint(worldPos, maxDist), 0.5f);
        // }
    }

    [Button("打开/关闭GM")]
    public void SwitchGM()
    {
        var mgr = SRServiceManager.GetService<IDebugService>();
        if (mgr != null)
        {
            if (!mgr.IsDebugPanelVisible)
            {
                SRServiceManager.GetService<IDebugService>().ShowDebugPanel();
            }
            else
            {
                SRServiceManager.GetService<IDebugService>().HideDebugPanel();
            }
        }
    }

    void Update()
    {
#if !DEBUG_MODE && !UNITY_EDITOR
        return;
#endif

        bool showGM = false;
#if UNITY_EDITOR
        showGM = true;
#else
        showGM = ChannelConfig.Instance.GM;
#endif
        if (showGM)
        {
            UpdateClickLeftBottomOpenGM();

            if ((Input.GetKeyDown(KeyCode.BackQuote) || Input.touchCount >= 5) && Time.realtimeSinceStartup - _lastSwitchTime > 0.1f)
            {
                _lastSwitchTime = Time.realtimeSinceStartup;
                SwitchGM();
            }
        }
    }

    private float _lastSwitchTime;


    private float _firstClickTime;

    private int _clickCount;

    private const float ClickThreshold = 2f;
    //点击左下角打开GM
    private void UpdateClickLeftBottomOpenGM()
    {
        if (_clickCount > 0 && Time.realtimeSinceStartup - _firstClickTime > ClickThreshold)
        {
            _clickCount = 0;
        }
        if (Input.GetMouseButtonDown(0) && InRightArea(Input.mousePosition))
        {
            if (_clickCount == 0)
            {
                _firstClickTime = Time.realtimeSinceStartup;
            }
            _clickCount++;

            if (_clickCount >= 5)
            {
                _clickCount = 0;
                SwitchGM();
            }
        }
    }

    private bool InRightArea(Vector2 pos)
    {
        var x = pos.x / Screen.width;
        var y = pos.y / Screen.height;
        return x < 0.1f || x > 0.9f && y < 0.1f;
    }
}
