using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


public class TestItemUtils : MonoBehaviour
{
    [Button]
    public void Test(int group, int times = 1)
    {
        for (int i = 0; i < times; i++)
        {
            var ret = ItemUtils.GetReward(group);
            Debug.LogError(ret.Type + "  " + ret.Count);
        }
    }
}
