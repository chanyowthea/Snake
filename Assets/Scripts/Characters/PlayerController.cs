using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseCharacter
{
    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        if (h != 0 || v != 0)
        {
            Move(new Vector3(h, v, 0) * MoveSpeed);
        }
    }
    
    public override void Die()
    {
        base.Die();
        GameManager.instance.RespawnCharacter(0);
    }
}
