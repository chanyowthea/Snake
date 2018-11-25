using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseCharacter
{
    [SerializeField] Camera _PlayerCamera;

    private void Start()
    {
        //InputManager.instance.onValueChanged += OnMove;
    }

    private void OnDestroy()
    {
        //InputManager.instance.onValueChanged -= OnMove;
    }

    public override void SetData(PlayerData data, int initBodyLength, float strongRatio = 0.3333f)
    {
        base.SetData(data, initBodyLength, strongRatio);

        if (CharacterID != 0)
        {
            _PlayerCamera.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (CharacterID == 0)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            if (h != 0 || v != 0)
            {
                Move(new Vector3(h, v, 0).normalized * MoveSpeed);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.LogError("BodyLength=" + BodyLength);
            }
            return;
            if (Input.GetKeyDown(KeyCode.D))
            {
                Move(new Vector3(1, 0, 0) * MoveSpeed);
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                Move(new Vector3(-1, 0, 0) * MoveSpeed);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                Move(new Vector3(0, -1, 0) * MoveSpeed);
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                Move(new Vector3(0, 1, 0) * MoveSpeed);
            }
        }
        else
        {
            float h = Input.GetAxisRaw("Horizontal1");
            float v = Input.GetAxisRaw("Vertical1");
            if (h != 0 || v != 0)
            {
                Move(new Vector3(h, v, 0).normalized * MoveSpeed);
            }
        }
    }

    private void OnMove(Vector2 pos)
    {
        Debug.Log(pos);
        if (pos.x != 0 || pos.y != 0)
        {
            Move(new Vector3(pos.x, pos.y, 0).normalized * MoveSpeed);
        }
    }

    public override void Die()
    {
        base.Die();
        GameManager.instance.RespawnCharacter(CharacterID);
    }
}
