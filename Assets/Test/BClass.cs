using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BClass : AClass
{
    private void Start()
    {
        int count = 0; 
        Debug.Log(Mathf.Exp(-count * GetMoveSpeedFactor(30, 0.6f)));
        count = 5;
        Debug.Log(Mathf.Exp(-count * GetMoveSpeedFactor(30, 0.6f)));
        count = 10;
        Debug.Log(Mathf.Exp(-count * GetMoveSpeedFactor(30, 0.6f)));
        count = 30; 
        Debug.Log(Mathf.Exp(-count * GetMoveSpeedFactor(30, 0.6f)));
        count = 60; 
        Debug.Log(Mathf.Exp(-count * GetMoveSpeedFactor(30, 0.6f)));
        count = 1000; 
        Debug.Log(Mathf.Exp(-count * GetMoveSpeedFactor(30, 0.6f)));
    }

    float GetMoveSpeedFactor(int x, float y)
    {
        if (x <= 0)
        {
            return 1;
        }
        return -Mathf.Log(y) / x;
    }

    public override void Run()
    {
        Debug.Log("Run " + this.GetType());
    }
}
