using System;
using System.Collections.Generic;

public class Timer
{
    protected float m_ExpiredTime = 0;
    public void Reset()
    {
        m_ExpiredTime = -1;
    }
    public void SetExpiredTime(float expiredTime)
    {
        m_ExpiredTime = expiredTime;
    }
    public bool IsExpired(float gameTime)
    {
        return m_ExpiredTime >= 0 && gameTime >= m_ExpiredTime;
    }
    public void Copy(Timer t)
    {
        m_ExpiredTime = t.m_ExpiredTime;
    }
}

public class TimerAction : Timer, IObjectPoolCallback
{
    private Action m_Action;

    private bool m_IsRepeated;
    private float m_Duration;
    private bool m_IsInPool = true;
    public void SetActionAt(float gameTime, float expiredTime, Action action, bool isRepeated)
    {
        SetExpiredTime(expiredTime);
        m_Action = action;
        m_Duration = expiredTime - gameTime;
        m_IsRepeated = isRepeated;
    }
    public bool Update(float gameTime, uint actionId, Dictionary<uint, Action> timerActionsToBeCalled)
    {
        if (IsExpired(gameTime) && m_Action != null)
        {
            //Action callBack = m_Action;
            //callBack.Invoke();
            timerActionsToBeCalled.Add(actionId, m_Action); //not call action in iterator
            if (m_IsRepeated)
            {
                float exceededTimeSlice = gameTime - m_ExpiredTime;
                SetExpiredTime(gameTime + m_Duration - exceededTimeSlice);
                return false;
            }
            else
            {
                m_Action = null;
                return true;
            }
        }
        return false;
    }
    public void OnAllocated()
    {
        m_IsInPool = false;
    }
    public void OnCollected()
    {
        Reset();
        m_Action = null;
        m_IsRepeated = false;
        m_Duration = 0;
        m_IsInPool = true;
    }
    public bool IsInPool()
    {
        return m_IsInPool;
    }
}
