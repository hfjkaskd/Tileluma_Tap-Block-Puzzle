using Cysharp.Threading.Tasks;
using UnityEngine;
using cfg;

public class GameMode_GamePlay : GameModeBase<GameMode_GamePlay>
{
    public override E_GameModeType GameModeType => E_GameModeType.GamePlay;

    /// <summary>
    /// 关卡开始，关卡结束，关卡结算。
    /// </summary>
    protected override void OnInit()
    {

#if BIZZA_REAL_WITHDRAW
        BridgingUtil.NewPlayerPropCountInit();
        AssetUtils.LoadSceneWithBlock(GameModeType.ToString(), () =>
        {
            UIModule.Instance.OpenPage(UIPageIds.RealGamePanel);
        });
#else
        AssetUtils.LoadSceneWithBlock(GameModeType.ToString(), () =>
        {
            //打开白包的游戏界面
        });
#endif
    }
 
    protected override void LateInit()
    {
       
    }

    protected override void OnPreRelease()
    {
        Resources.UnloadUnusedAssets();
        int layer = UIModule.Instance.GetLayerByName(nameof(E_UILayer.BaseLayer));
        UIModule.Instance.CloseAndDestroyLayerAllPage(layer);
    }

    /// <summary>
    /// 是否开始自定义的教程
    /// </summary>
    /// <param name="startState"></param>
    public void OnSetCustomTutorialState(bool startState)
    {
#if BIZZA_REAL_WITHDRAW
        if (SaveDataUtils.GameData.customTutorialEnd)
        {
            return;
        }
        SaveDataUtils.GameData.customTutorialEnd = startState;
        SaveDataUtils.Save();
#endif
    }


}
