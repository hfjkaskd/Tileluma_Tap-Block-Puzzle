using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ReviewModeSwitch
{
    [MenuItem("Tools/审核开关/打开")]
    static void ReviewMode_On()
    {
        EditorPrefs.SetBool("Editor_ReviewMode", true);
    }

    [MenuItem("Tools/审核开关/关闭")]
    static void ReviewMode_Off()
    {
        EditorPrefs.SetBool("Editor_ReviewMode", false);
    }

    [MenuItem("Tools/审核开关/清除")]
    static void ReviewMode_Clear()
    {
        EditorPrefs.DeleteKey("Editor_ReviewMode");
    }



}
