using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : BaseCharacter
{
    public override void Die()
    {
        base.Die();
        GameManager.instance.RespawnCharacter(RandomUtil.instance.Next(1, 3), CharacterUniqueID); 
    }
}
