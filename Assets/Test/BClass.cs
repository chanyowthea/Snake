using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BClass : AClass
{
    private void Start()
    {
        var c = this as AClass;
        c.GetComponent<ITest>().Run();
    }

    public override void Run()
    {
        Debug.Log("Run " + this.GetType());
    }
}
