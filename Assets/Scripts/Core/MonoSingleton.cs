using UnityEngine;
using System.Collections;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static string _singletonRootName = "SingletonRoot";
    private static GameObject _singletonRoot;
    private static T _instance;

    public static T instance
    {
        get
        {
            CreateRoot();
            CreateInstance();
            return _instance;
        }
        set
        {
            _instance = value;
        }
    }

    protected virtual void Awake()
    {
        _instance = this as T;
    }

    static void CreateRoot()
    {
        if (_singletonRoot == null) //如果是第一次调用单例类型就查找所有单例类的总结点  
        {
            _singletonRoot = GameObject.Find(_singletonRootName);
            if (_singletonRoot == null) //如果没有找到则创建一个所有继承MonoBehaviour单例类的节点  
            {
                _singletonRoot = new GameObject();
            }
            _singletonRoot.name = _singletonRootName;
            //DontDestroyOnLoad(_singletonRoot); //防止被销毁  
        }
    }

    static void CreateInstance()
    {
        if (_instance == null) //为空表示第一次获取当前单例类  
        {
            _instance = _singletonRoot.GetComponent<T>();
            if (_instance == null) //如果当前要调用的单例类不存在则添加一个  
            {
                _instance = _singletonRoot.AddComponent<T>();
            }
        }
    }
}

public class TSingleton<T> where T : class, new()
{
    private static T _instance = null;
    public static T instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new T();
            }
            return _instance;
        }
    }

    public virtual void Init()
    {

    }

    public virtual void Clear()
    {

    }
}