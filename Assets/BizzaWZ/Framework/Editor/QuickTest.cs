#if UNITY_EDITOR
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class QuickLaunch : MonoBehaviour
{
    [MenuItem("Tools/快捷方式/进入Loading场景 %q")]
    public static void EnterScene_Loading()
    {
        EditorSceneManager.OpenScene(WZProjectSetting.EditorResPath + "Scenes/InitWZ.unity");
        EditorApplication.isPlaying = true;
    }

    [MenuItem("Tools/快捷方式/进入战斗测试场景 %w")]
    public static void EnterScene_BattleTest()
    {
        EditorSceneManager.OpenScene(WZProjectSetting.EditorResPath + "Scenes/GamePlay.unity");
    }

    [MenuItem("Tools/快捷方式/打开表格目录 %t")]
    static void OpenTableFolder()
    {
        var path = Application.dataPath + "\\..\\..\\_Data\\Data";
        Debug.Log(path);
        Process.Start(path);
    }

    [MenuItem("Tools/快捷方式/打开SVN目录 %&[")]
    static void OpenSVNFolder1()
    {
        var path = Application.dataPath + "\\..\\..\\..";
        Debug.Log(path);
        Process.Start(path);
    }

    [MenuItem("Tools/快捷方式/打开框架SVN目录 %&]")]
    static void OpenSVNFolder2()
    {
        var path = Application.dataPath + "\\BizzaGame";
        Debug.Log(path);
        Process.Start(path);
    }

    [MenuItem("Tools/快捷方式/SVN更新 %&u")]
    static void SVNUpdate()
    {
        var path = Application.dataPath + "\\..\\..";
        _SVNCommand(path, "update");
    }

    [MenuItem("Tools/快捷方式/SVN提交 %&c")]
    static void SVNCommit()
    {
        var path = Application.dataPath + "\\..\\..";
        _SVNCommand(path, "commit");
    }

    static async Task<bool> _SVNCommand(string workingCopyPath, string command)
    {
        var process = new Process
        {
            StartInfo =
            {
                FileName = "svn",
                Arguments = $"{command} \"{workingCopyPath}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = false
            }
        };

        process.Start();
        string output = await process.StandardOutput.ReadToEndAsync();
        process.WaitForExit();

        Debug.Log(output);
        var ret = process.ExitCode == 0;
        return ret;
    }

    // [MenuItem("Tools/快捷方式/Action folder %&w")]
    // public static void PingFolder_Action()
    // {
    //     var targetPath = "Assets/GameRes/ConfigAssets/Actions/BL_BV_PlayerWeapon1.txt";
    //     Debug.LogError(targetPath);
    //     Object targetObject = AssetDatabase.LoadAssetAtPath<Object>(targetPath);
    //     Debug.LogError(targetObject == null);
    //     if (targetObject != null)
    //     {
    //         Selection.activeObject = targetObject;
    //         EditorGUIUtility.PingObject(targetObject);
    //     }
    // }
}
#endif