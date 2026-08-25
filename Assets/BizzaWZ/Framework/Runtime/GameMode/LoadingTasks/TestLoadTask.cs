using System.Collections;
using System.Collections.Generic;
using Bizza.Loading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TestLoadTask : LoadingTaskBase
{
    public override LoadingTaskName TaskName => LoadingTaskName.TestWait3s;
    public override float Weight => 3f;

    public override LoadingTaskName[] Dependencies => new LoadingTaskName[]
    {
        LoadingTaskName.LoadTables,
    };

    public override async UniTask Execute()
    {
        SetProgress(1);
        await UniTask.DelayFrame(60 * 2);
    }
}
