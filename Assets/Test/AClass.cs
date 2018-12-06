using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class AClass : MonoBehaviour, ITest
{
    private void Start()
    {
        Run();
    }

    public virtual void Run()
    {
        Debug.Log("Run " + this.GetType()); 
        int a = 0;
        string s = null; 
        Assert.IsNotNull(s, "s is empty! "); 
        int b = 2; 
        Debug.Log("b=" + b);
    }
}
