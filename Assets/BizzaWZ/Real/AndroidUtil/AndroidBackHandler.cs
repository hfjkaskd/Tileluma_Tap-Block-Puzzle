#if BIZZA_REAL_WITHDRAW
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidBackHandler : MonoBehaviour
{
    // 数值越小，越优先关闭，
    // 提现信息确认界面 要小于 提现信息界面
    public Dictionary<int, List<PageId>> PriorityList = new ()
    {
        #if BIZZA_REAL_WITHDRAW
        {
            0, new List<PageId>()
            {
                UIPageIds.RealWithdrawPanel,
                UIPageIds.FakeWithdrawPanel,
                UIPageIds.WithdrawDanPanel
            }
        },
        // {
        //     1, new List<UIPageIds>()
        //     {
        //         UIPageIds.RealWithdrawPanel,
        //         UIPageIds.FakeWithdrawPanel,
        //         UIPageIds.WithdrawDanPanel
        //     }
        // },
        #endif
    };
    
    void Update()
    {
        // Android 返回键
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitCurrentView();
        }
    }

    void ExitCurrentView()
    {
        foreach (var pairs in PriorityList)
        {
            foreach (var page in pairs.Value)
            {
                bool isOpen = IsOpen(page);
                if (isOpen)
                {
                    UIModule.Instance.ClosePage(page);
                    return;
                }
            }
        }
    }

    private bool IsOpen(PageId pageType)
    {
        return UIModule.Instance.GetPage(pageType);
    }
}

#endif