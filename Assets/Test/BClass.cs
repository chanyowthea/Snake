using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BClass : AClass
{
    private void Start()
    {

    }

    public override void Run()
    {
        Debug.Log("Run " + this.GetType());
    }
}
