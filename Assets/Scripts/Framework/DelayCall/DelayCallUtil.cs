using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayCallUtil : MonoBehaviour
{
    //public static DelayCallUtil instance;
    public static ObjectPool<TimerAction> GlobalTimerActionPool = new ObjectPool<TimerAction>();
    private UniqueIDGenerator m_UniqueIDGen = new UniqueIDGenerator();
    private Dictionary<uint, TimerAction> m_TimerActions = new Dictionary<uint, TimerAction>();
    private List<uint> m_ExpiredTimerActions = new List<uint>();
    private Dictionary<uint, Action> m_TimerActionsToBeCalled = new Dictionary<uint, Action>();

    public TimeService Timer { private set; get; }
    public float GameTime
    {
        get
        {
            return Timer.GameTime;
        }
    }

    void Awake()
    {
        //instance = this;
        Timer = new TimeService();
        Timer.Reset();
    }

    public uint DelayCall(float delayTime, Action action, bool isRepeated = false)
    {
        TimerAction tAction = GlobalTimerActionPool.AllocObject();
        tAction.SetActionAt(Timer.GameTime, Timer.GameTime + delayTime, action, isRepeated);
        uint nextID = m_UniqueIDGen.GetUniqueID();
        m_TimerActions.Add(nextID, tAction);
        return nextID;
    }

    public void CancelDelayCall(uint id)
    {
        TimerAction action;
        if (m_TimerActions.TryGetValue(id, out action))
        {
            GlobalTimerActionPool.CollectObject(action);
            m_TimerActions.Remove(id);
        }
    }

    public void RunOneFrame()
    {
        //update engine timer
        Timer.UpdateTime();
        //update timer action
        m_TimerActionsToBeCalled.Clear();
        m_ExpiredTimerActions.Clear();
        foreach (KeyValuePair<uint, TimerAction> p in m_TimerActions)
        {
            if (p.Value.Update(Timer.GameTime, p.Key, m_TimerActionsToBeCalled) == true)
            {
                //expired
                m_ExpiredTimerActions.Add(p.Key);
            }
        }
        UnityEngine.Profiling.Profiler.BeginSample("m_EngineTimer called");
        foreach (KeyValuePair<uint, Action> action in m_TimerActionsToBeCalled)
        {
            if (m_TimerActions.ContainsKey(action.Key))
            {
                try
                {
                    action.Value.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogError(e.StackTrace);
                }
            }
        }
        UnityEngine.Profiling.Profiler.EndSample();
        foreach (uint timeActionID in m_ExpiredTimerActions)
        {
            if (m_TimerActions.ContainsKey(timeActionID))
            {
                GlobalTimerActionPool.CollectObject(m_TimerActions[timeActionID]);
                m_TimerActions.Remove(timeActionID);
            }
        }
    }
    public void FixedRunOneFrame()
    {

    }
}
