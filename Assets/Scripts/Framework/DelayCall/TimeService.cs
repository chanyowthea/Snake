using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeService
{
    public float _TimeScale = 1;

    private float m_GameTime;
    public float GameTime
    {
        get
        {
            return m_GameTime;
        }
    }
    private float m_LastGameTime;
    public float LastGameTime
    {
        get
        {
            return m_LastGameTime;
        }
    }

    private float m_DeltaTime;
    public float DeltaTime
    {
        get
        {
            return m_DeltaTime;
        }
    }

    public void Reset()
    {
        m_GameTime = 0f;
        m_LastGameTime = 0f;
        m_DeltaTime = 0f;
    }

    public void UpdateTime()
    {
        m_DeltaTime = Time.deltaTime * _TimeScale;
        m_LastGameTime = m_GameTime;
        m_GameTime += m_DeltaTime;
    }
    public void ClearDeltaTime()
    {
        m_DeltaTime = 0;
    }
}
