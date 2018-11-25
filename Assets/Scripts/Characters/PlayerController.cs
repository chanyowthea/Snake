using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseCharacter
{
    private void Start()
    {
        //InputManager.instance.onValueChanged += OnMove;
    }

    private void OnDestroy()
    {
        //InputManager.instance.onValueChanged -= OnMove;
    }

    void Update()
    {
        if (CharacterID == 0)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            if (h != 0 || v != 0)
            {
                Move(new Vector3(h, v, 0) * MoveSpeed);
            }
        }
        else
        {
            float h = Input.GetAxisRaw("Horizontal1");
            float v = Input.GetAxisRaw("Vertical1");
            if (h != 0 || v != 0)
            {
                Move(new Vector3(h, v, 0) * MoveSpeed);
            }
        }
    }

    private void OnMove(Vector2 pos)
    {
        Debug.Log(pos);
        if (pos.x != 0 || pos.y != 0)
        {
            Move(new Vector3(pos.x, pos.y, 0) * MoveSpeed);
        }
    }

    public override void Die()
    {
        base.Die();
        GameManager.instance.RespawnCharacter(0);
    }
}
