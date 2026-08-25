#if BIZZA_REAL_WITHDRAW
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class PropUtils
{
    public static bool UseProp(E_ItemType itemType)
    {
        bool used = false;
        switch (itemType)
        {
            case E_ItemType.GameProp_Back:
                used = UseProp_1();
                break;

            case E_ItemType.GameProp_ClearArea:
                used = UseProp_2();
                break;

            case E_ItemType.GameProp_Refresh:
                used = UseProp_3();
                break;
        }
        return used;
    }

    public static bool UseProp_1()
    {
        BridgingUtil.PropUse_1();
        return true;
    }

    public static bool UseProp_2()
    {
        BridgingUtil.PropUse_2();
        return true;
    }

    public static bool UseProp_3()
    {
        BridgingUtil.PropUse_3();
        return true;
    }

}
#endif