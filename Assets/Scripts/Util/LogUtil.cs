using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LogUtil
{
    public static string GetCurMethodName()
    {
        return new System.Diagnostics.StackFrame(1).GetMethod().Name;
    }

    public static string GetCurClassName()
    {
        return new System.Diagnostics.StackFrame(1).GetMethod().DeclaringType.Name;
    }
}