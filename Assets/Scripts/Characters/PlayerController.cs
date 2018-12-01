using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseCharacter
{
    public override bool IsLocalPlayer
    {
        get
        {
            return true;
        }

        protected set
        {
            base.IsLocalPlayer = value;
        }
    }

    public static PlayerController instance { private set; get; }
    [SerializeField] Camera _PlayerCamera;

    private void Awake()
    {
#if UNITY_EDITOR
        if (instance != null)
        {
            return;
        }
#endif
        instance = this;
    }

    public override void SetData(PlayerInfo data, int initBodyLength)
    {
        base.SetData(data, initBodyLength);

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
                var go = GameObject.Find("Enemy(Clone)"); 
                go.GetComponent<Enemy>().SetTargetEnemy(null);
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                var ls = GameManager.instance.GetCharacters();
                PlayerController.instance._PlayerCamera.gameObject.SetActive(false);
                GameObject.Find("Enemy(Clone)").transform.GetChild(0).gameObject.SetActive(true);
            }
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                var ls = GameManager.instance.GetCharacters();
                PlayerController.instance._PlayerCamera.gameObject.SetActive(true);
                GameObject.Find("Enemy(Clone)").transform.GetChild(0).gameObject.SetActive(false);
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

    public void OnMove(Vector2 pos)
    {
        if (pos.x != 0 || pos.y != 0)
        {
            Move(new Vector3(pos.x, pos.y, 0).normalized * MoveSpeed);
        }
    }

    public override void Die()
    {
        base.Die();
        instance = null; 
        GameManager.instance.RespawnCharacter(CharacterID, CharacterUniqueID);
    }
}
