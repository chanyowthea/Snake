using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void OnStarted();
public delegate void OnFinished();

public class CountDownConfig
{
    public ulong m_EndTime;
    public string m_Prefix;
    public string m_Suffix;
    public bool m_Formated;
    public OnStarted m_OnStarted;
    public OnFinished m_OnFinished;
}

public class CountDownText : MonoBehaviour
{
    public Text m_Label;

    float endTime = 0;
    private string m_Prefix = "";
    private string m_Suffix = "";
    private bool m_Formated = true;
    public OnStarted m_OnStarted = null;
    public OnFinished m_OnFinished = null;

    private uint m_DelayCall = 0;

    public void SetCountDownEndTime(float end, string prefix = "", string suffix = "", bool formated = true, OnStarted onStarted = null, OnFinished onFinished = null)
    {
        endTime = end;
        m_OnStarted = onStarted;
        m_OnFinished = onFinished;
        m_Prefix = prefix;
        m_Suffix = suffix;
        m_Formated = formated;

        GameManager.instance.CancelDelayCall(m_DelayCall);
        if (m_Label != null && enabled)
            CountDown();//call it once immediately
        m_DelayCall = GameManager.instance.DelayCall(1.0f, CountDown, true);
    }

    public void SetCountDownEndTime(CountDownConfig config)
    {
        SetCountDownEndTime(config.m_EndTime, config.m_Prefix, config.m_Suffix, config.m_Formated, config.m_OnStarted, config.m_OnFinished);
    }

    void OnEnable()
    {
        if (m_Label == null)
        {
            m_Label = GetComponent<Text>();
        }
    }

    void OnDestroy()
    {
        GameManager.instance.CancelDelayCall(m_DelayCall);
    }

    void CountDown()
    {
        if (m_OnStarted != null)
            m_OnStarted();

        float nowStamp = GameManager.instance.GameTime;
        if (nowStamp <= endTime)
        {
            if (m_Formated)
                m_Label.text = m_Prefix + TimeUtil.FormatTimeStampCD(endTime) + m_Suffix;
            else
                m_Label.text = m_Prefix + endTime + m_Suffix;

        }
        else
        {
            if (m_OnFinished != null)
                m_OnFinished();
            GameManager.instance.CancelDelayCall(m_DelayCall);
        }
    }

    public bool IsDuringCountDown()
    {
        return m_DelayCall != 0;
    }

    public void Cancel()
    {
        GameManager.instance.CancelDelayCall(m_DelayCall);
    }
}
