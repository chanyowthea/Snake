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
    [SerializeField] PlayerCamera _PlayerCamera;

    public override void SetData(PlayerInfo data, int initBodyLength)
    {
        base.SetData(data, initBodyLength);
        _PlayerCamera.SetData(_Head.transform);
        if (CharacterID == 0)
        {
            _PlayerCamera.gameObject.SetActive(true);
        }
    }

    public override void ClearData()
    {
        _PlayerCamera.ClearData(); 
        _PlayerCamera.gameObject.SetActive(false);
        base.ClearData();
    }

    public sealed override void OnAllocated()
    {
        instance = this; 
        base.OnAllocated();
    }

    public sealed override void OnCollected()
    {
        instance = null; 
        _PlayerCamera.gameObject.SetActive(false);
        base.OnCollected();
    }

    void Update()
    {
        if (CharacterID == 0)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            if (h != 0 || v != 0)
            {
                //Debug.Log("x=" + h + ", y=" + v);
                Move(new Vector3(h, v, 0).normalized * MoveSpeed * Singleton._DelayUtil.Timer.DeltaTime);
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
                Move(new Vector3(1, 0, 0) * MoveSpeed * Singleton._DelayUtil.Timer.DeltaTime);
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                Move(new Vector3(-1, 0, 0) * MoveSpeed * Singleton._DelayUtil.Timer.DeltaTime);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                Move(new Vector3(0, -1, 0) * MoveSpeed * Singleton._DelayUtil.Timer.DeltaTime);
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                Move(new Vector3(0, 1, 0) * MoveSpeed * Singleton._DelayUtil.Timer.DeltaTime);
            }
        }
        else
        {
            float h = Input.GetAxisRaw("Horizontal1");
            float v = Input.GetAxisRaw("Vertical1");
            if (h != 0 || v != 0)
            {
                Move(new Vector3(h, v, 0).normalized * MoveSpeed * Singleton._DelayUtil.Timer.DeltaTime);
            }
        }
    }

    Vector3 _MoveDir;
    public void OnMove(Vector2 pos)
    {
        //return; 

        if (pos.x != 0 || pos.y != 0)
        {
            //if (pos.x != 0)
            //{
            //    pos.x = Mathf.Sign(pos.x) * 1;
            //}
            //if (pos.y != 0)
            //{
            //    pos.y = Mathf.Sign(pos.y) * 1;
            //}

            //Debug.Log("os.x=" + pos.x + ", pos.y=" + pos.y);
            _MoveDir = pos;
            Move(_MoveDir.normalized * MoveSpeed * Singleton._DelayUtil.Timer.DeltaTime);
        }
    }

    public override void Die()
    {
        base.Die();
        instance = null;
        GameManager.instance.RemoveCharacter(this);
        GameManager.instance.RespawnCharacter(CharacterID, CharacterUniqueID);
    }
}
