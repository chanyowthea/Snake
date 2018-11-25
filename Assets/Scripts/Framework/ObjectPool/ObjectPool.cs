using UnityEngine;
using System.Collections.Generic;

public class ObjectPool<T> where T : IObjectPoolCallback, new()
{
    protected Queue<T> m_FreeObjects;
    public ObjectPool()
    {
        m_FreeObjects = new Queue<T>();
    }
    public T AllocObject()
    {
        IObjectPoolCallback allocatedObj = AllocObjectInternal();
        if (allocatedObj == null)
        {
            allocatedObj = new T();
        }
        allocatedObj.OnAllocated();
        return (T)allocatedObj;
    }
    public void CollectObject(T obj)
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