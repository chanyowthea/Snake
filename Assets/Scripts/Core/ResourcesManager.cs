using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesManager : TSingleton<ResourcesManager>
{
    public Dictionary<string, UnityEngine.Object> _LoadedAssetDict = new Dictionary<string, Object>();
    public const string _PicturePrefix = "Textures/";
    public const string _UIPrefix = "UI/";
    public const string _TurretPrefix = "Prefabs/Turret/";

    UnityEngine.Object Load(string path)
    {
        return Resources.Load(path);
    }

    public void UnloadAll()
    {
        Resources.UnloadUnusedAssets();
    }

    public void UnloadAsset(UnityEngine.Object asset)
    {
        Resources.UnloadAsset(asset);
    }

    public T GetAsset<T>(string path)
        where T : UnityEngine.Object
    {
        if (_LoadedAssetDict.ContainsKey(path))
        {
            return (T)_LoadedAssetDict[path];
        }
        return Resources.Load<T>(path);
    }

    public Sprite GetSprite(string path)
    {
        return GetAsset<Sprite>(_PicturePrefix.Append(path));
    }

    public T GetUI<T>()
        where T : UnityEngine.Object
    {
        var csv = ConfigDataManager.instance.GetData<UICSV>(typeof(T).ToString());
        if (csv != null)
        {
            var obj = Load(_UIPrefix.Append(csv._Path));
            var go = GameObject.Instantiate(obj);
            return ((GameObject)go).GetComponent<T>();
        }
        return null;
    }
}
