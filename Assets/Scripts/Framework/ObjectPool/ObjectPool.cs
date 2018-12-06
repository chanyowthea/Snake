using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class ObjectPool<T> where T : IObjectPoolCallback, new()
{
    protected Queue<T> m_FreeObjects;
    public ObjectPool()
    {
        m_FreeObjects = new Queue<T>();
    }
    public virtual T AllocObject()
    {
        IObjectPoolCallback allocatedObj = AllocObjectInternal();
        if (allocatedObj == null)
        {
            allocatedObj = new T();
        }
        allocatedObj.OnAllocated();
        return (T)allocatedObj;
    }
    public virtual void CollectObject(T obj)
    {
        if (obj == null)
        {
            return;
        }
        obj.OnCollected();
        CollectObjectInternal(obj);
    }
    protected virtual IObjectPoolCallback AllocObjectInternal()
    {
        if (m_FreeObjects.Count > 0)
        {
            return m_FreeObjects.Dequeue();
        }
        return null;
    }
    protected virtual void CollectObjectInternal(T obj)
    {
        m_FreeObjects.Enqueue(obj);
    }
}

public class BaseMonoObject : MonoBehaviour, IObjectPoolCallback
{
    public virtual void OnAllocated()
    {
        gameObject.SetActive(true);
    }

    public virtual void OnCollected()
    {
        gameObject.SetActive(false);
    }
}

public class CharacterPool : ObjectPool<BaseCharacter>
{
    BaseCharacter _Prefab; 
    public void SetData(BaseCharacter prefab)
    {
        _Prefab = prefab; 
    }

    public void ClearData()
    {
        Assert.IsNotNull(_Prefab); 
        _Prefab = null; 
    }

    public sealed override BaseCharacter AllocObject()
    {
        IObjectPoolCallback allocatedObj = AllocObjectInternal();
        Assert.IsNotNull(allocatedObj);
        allocatedObj.OnAllocated();
        return (BaseCharacter)allocatedObj;
    }

    protected sealed override IObjectPoolCallback AllocObjectInternal()
    {
        if (m_FreeObjects.Count > 0)
        {
            return m_FreeObjects.Dequeue();
        }
        Assert.IsNotNull(_Prefab);
        return GameObject.Instantiate(_Prefab);
    }
}

public class BodyPool : ObjectPool<Body>
{
    Body _Prefab;
    public void SetData(Body prefab)
    {
        _Prefab = prefab;
    }

    public void ClearData()
    {
        Assert.IsNotNull(_Prefab);
        _Prefab = null;
    }

    public sealed override Body AllocObject()
    {
        IObjectPoolCallback allocatedObj = AllocObjectInternal();
        Assert.IsNotNull(allocatedObj);
        allocatedObj.OnAllocated();
        return (Body)allocatedObj;
    }

    protected sealed override IObjectPoolCallback AllocObjectInternal()
    {
        if (m_FreeObjects.Count > 0)
        {
            return m_FreeObjects.Dequeue();
        }
        Assert.IsNotNull(_Prefab);
        return GameObject.Instantiate(_Prefab);
    }
}

public class FoodPool : ObjectPool<Food>
{
    Food _Prefab;
    public void SetData(Food prefab)
    {
        _Prefab = prefab;
    }

    public void ClearData()
    {
        Assert.IsNotNull(_Prefab);
        _Prefab = null;
    }

    public sealed override Food AllocObject()
    {
        IObjectPoolCallback allocatedObj = AllocObjectInternal();
        Assert.IsNotNull(allocatedObj);
        allocatedObj.OnAllocated();
        return (Food)allocatedObj;
    }

    protected sealed override IObjectPoolCallback AllocObjectInternal()
    {
        if (m_FreeObjects.Count > 0)
        {
            return m_FreeObjects.Dequeue();
        }
        Assert.IsNotNull(_Prefab);
        return GameObject.Instantiate(_Prefab);
    }
}