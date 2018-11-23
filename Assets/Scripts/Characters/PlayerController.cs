using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseCharacter
{
    void Start()
    {
        SetData(1); 
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        if (h != 0 || v != 0)
        {
            transform.position += new Vector3(h, v, 0) * MoveSpeed;
        }
    }
}
