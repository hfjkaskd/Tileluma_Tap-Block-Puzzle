#if BIZZA_REAL_WITHDRAW
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class SimpleNightTicker : MonoBehaviour
{
    private CancellationTokenSource _cts;

    // 你要每秒执行的多个方法，直接塞这里
    private Action[] _jobs;

    private void Start()
    {
    }

    private void OnDestroy()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }



    private void JobA()
    {

    }

    private void JobB()
    {

    }
    
    private void JobC()
    {

    }
    
    private void JobD()
    {

    }
    
    private void JobE()
    {

    }
}
#endif