#if BIZZA_REAL_WITHDRAW
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class NumbericalStatistics
{
    public static Dictionary<E_ItemType, int> _propUseTimes = new();
    #region 道具

    public static void InitProp()
    {
        _propUseTimes[E_ItemType.GameProp_Back] = 0;
        _propUseTimes[E_ItemType.GameProp_ClearArea] = 0;
        _propUseTimes[E_ItemType.GameProp_Refresh] = 0;
    }


    #endregion

}
#endif