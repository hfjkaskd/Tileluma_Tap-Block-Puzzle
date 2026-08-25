using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public static class LubanToolExt
{



    [MenuItem("Tools/导出表格 %e")]
    static void ExportTableAndGenCode()
    {
        var path = Application.dataPath + "/../_Bizza_Tools/Luban/gen.bat";
        Debug.Log(path);
        // Process.Start(path);
        using (Process myPro = new Process())
        {
            myPro.StartInfo.FileName = path;
            myPro.StartInfo.UseShellExecute = true;
            myPro.StartInfo.WorkingDirectory = Path.GetDirectoryName(path);
            myPro.StartInfo.CreateNoWindow = false;
            myPro.Start();
            // myPro.WaitForExit();
        }
    }
    
    [MenuItem("Tools/快捷方式/打开表格目录 %t")]
    static void OpenTableFolder()
    {
        var path = Application.dataPath + "\\..\\..\\_Data\\Data";
        Debug.Log(path);
        Process.Start(path);
    }

}