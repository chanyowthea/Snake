using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// #代表Shift，%代表Ctrl，&代表Alt
public class EditorUtil
{
    [MenuItem("Tools/Start Launcher %G")] //Ctrl+G  
    public static void StartLauncher()
    {
        //if (EditorUtility.DisplayDialog("Start game",
        //    "Do you want to run game anyway? \n\nAll unsaved datas in current scene will be LOST!!!!!!", "GoGoGo!!!", "No") == false)
        //{
        //    return;
        //}

        EditorApplication.isPaused = false;
        EditorApplication.isPlaying = false;
        EditorSceneManager.OpenScene("Assets/Scenes/Launcher.unity");
    }

    [MenuItem("Tools/Start Test %T")]
    public static void StartTest()
    {
        EditorApplication.isPaused = false;
        EditorApplication.isPlaying = false;
        EditorSceneManager.OpenScene("Assets/Test/Scenes/Test.unity");
    }

    static public void SetDirty(UnityEngine.Object obj)
    {
#if UNITY_EDITOR
        if (obj)
        {
            //if (obj is Component) Debug.Log(NGUITools.GetHierarchy((obj as Component).gameObject), obj);
            //else if (obj is GameObject) Debug.Log(NGUITools.GetHierarchy(obj as GameObject), obj);
            //else Debug.Log("Hmm... " + obj.GetType(), obj);
            UnityEditor.EditorUtility.SetDirty(obj);
        }
#endif
    }

    /// <summary>
    /// Convenience function that converts Class + Function combo into Class.Function representation.
    /// </summary>

    static public string GetFuncName(object obj, string method)
    {
        if (obj == null) return "<null>";
        string type = obj.GetType().ToString();
        int period = type.LastIndexOf('/');
        if (period > 0) type = type.Substring(period + 1);
        return string.IsNullOrEmpty(method) ? type : type + "/" + method;
    }
}
