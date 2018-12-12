using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EfficiencyTest : MonoBehaviour
{
    // foreach, 拼接字符串
    void Start()
    {
        float time = Time.realtimeSinceStartup;
        for (int i = 0; i < 10000; i++)
        {
            Run();
        }
        Debug.Log(Time.realtimeSinceStartup - time);
        UnityEngine.Profiling.Profiler.BeginSample("a=Run");
        time = Time.realtimeSinceStartup;
        for (int i = 0; i < 10000; i++)
        {
            Action a = Run;
            if (a != null)
            {
                a();
            }
        }
        UnityEngine.Profiling.Profiler.EndSample();
        Debug.Log(Time.realtimeSinceStartup - time);
        UnityEngine.Profiling.Profiler.BeginSample("a=()=>Run");
        time = Time.realtimeSinceStartup;
        for (int i = 0; i < 10000; i++)
        {
            Action a = () => Run();
            if (a != null)
            {
                a();
            }
        }
        UnityEngine.Profiling.Profiler.EndSample();
        Debug.Log(Time.realtimeSinceStartup - time);
    }

    void Run()
    {

    }

    void Update()
    {

    }
}
