using System;
using System.Collections;
using System.Collections.Generic;
using Bizza.Loading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class CheckNetworkTask : LoadingTaskBase
{
    public override LoadingTaskName TaskName => LoadingTaskName.CheckNetwork;
    public override float Weight => 1f;
    public override LoadingTaskName[] Dependencies => Array.Empty<LoadingTaskName>();
    public override async UniTask Execute()
    {
        SetProgress(0.9f);
        LogLogger.LogInfo("CheckNetworkTask-start");
        int maxTime = 3;
        while (true)
        {
            var hasNetwork = await NetworkUtils.HasNetworkConnectionAsync();
            LogLogger.LogInfo($"CheckNetworkTask:{hasNetwork}");
            if (!hasNetwork)
            {
                var args = new CommonConfirmTipsPanel.Args()
                {
                    isLanguage = true,
                    des = "Tips_NetworkError",
                };
                await UIModule.Instance.OpenPage(UIPageIds.CommonConfirmTipsPanel, args);
                //等待UI关闭
                await UniTask.WaitUntil(()=>UIModule.Instance.GetPage<CommonConfirmTipsPanel>() == null);
            }
            else
            {
                break;
            }

            maxTime--;
            if (maxTime <= 0)
            {
                Application.Quit();
            }
        }
        LogLogger.LogInfo("CheckNetworkTask-end");
    }
}
