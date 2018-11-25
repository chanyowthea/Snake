using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AClass : MonoBehaviour, ITest
{
    public virtual void Run()
    {
        Debug.Log("Run " + this.GetType()); 
    }
}
