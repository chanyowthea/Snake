using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeUtil : MonoBehaviour
{
    public static string FormatTimeStampCD(double timeStamp, bool isBefore = false, bool needSec = true)
    {
        int secondSpan;

        if (isBefore)
        {
            secondSpan = (int)(Singleton._DelayUtil.GameTime - timeStamp);
        }
        else
        {
            secondSpan = (int)(timeStamp - Singleton._DelayUtil.GameTime);
        }

        if (secondSpan < 0)
        {
            secondSpan = 0;
        }
        TimeSpan span = new TimeSpan(0, 0, secondSpan);
        int daysSub = span.Days;
        int hoursSub = span.Hours + daysSub * 24;
        int minutesSub = span.Minutes;
        int secondsSub = span.Seconds;

        string result = null;

        if (isBefore)
        {
            if (span.Days > 0)
            {
                if (span.Days >= 7)
                {
                    result = "一周前";
                }
                else
                {
                    result = string.Format("{0}天前", span.Days);
                }
            }
            else if (span.Hours > 0)
            {
                result = string.Format("{0}小时前", span.Hours);
            }
            else
            {
                result = string.Format("{0}分钟前", span.Minutes > 0 ? span.Minutes : 1);
            }
        }
        else
        {
            if (needSec)
            {
                if (span.Days > 0)
                {
                    result = string.Format("{0}天", span.Days) +
                        string.Format("{0:D2}:{1:D2}:{2:D2}",
                        span.Hours, minutesSub, secondsSub
                        );
                }
                else
                {
                    result = string.Format("{0:D2}:{1:D2}:{2:D2}",
                        hoursSub, minutesSub, secondsSub
                        );
                }
            }
            else
            {
                if (span.Days > 0)
                {
                    result = string.Format("{0}天", span.Days) +
                        string.Format("{0:D2}:{1:D2}",
                        span.Hours, minutesSub
                        );
                }
                else
                {
                    result = string.Format("{0:D2}:{1:D2}",
                        hoursSub, minutesSub
                        );
                }
            }
        }

        return result;
    }
}