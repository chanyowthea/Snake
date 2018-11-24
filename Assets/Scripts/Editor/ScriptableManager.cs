using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;


public class ScriptableManager : MonoBehaviour
{
	[MenuItem("Configs/GameData")]
	static void CreateView()
	{
		CreateAsset<GameData>();
	}

	/// <param name="isSpecify">是否的指定新路径 <c>true</c> 是 </param>
	public static void CreateAsset<T>(string newDir = "Assets/Configs", bool isSpecify = false) where T : ScriptableObject
	{
		if(isSpecify)
		{
			Create<T>(newDir); 
			return; 
		}
		Create<T>(GetPath()); 
	}

	static void Create<T>(string dir) where T : ScriptableObject
	{
		if (!Directory.Exists(dir))
		{
			Directory.CreateDirectory(dir); 
		}
		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(dir + "/New" + typeof(T).ToString() + ".asset");
		T asset = ScriptableObject.CreateInstance<T>(); 
		AssetDatabase.CreateAsset(asset, assetPathAndName);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		EditorUtility.FocusProjectWindow();
		Selection.activeObject = asset;
	}

	static string GetPath()
	{
		string dir = AssetDatabase.GetAssetPath(Selection.activeObject);
		if (dir == "")
		{
			dir = "Assets/Configs";
		}
		else if (string.Compare(Path.GetExtension(dir), "") != 0)
		{
			// if the extension exists, then go to the parent directory
			dir = Path.GetDirectoryName(dir);
		}
		return dir; 
	}
}
