using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCharacter : MonoBehaviour
{
    protected float _MoveSpeed;
    public virtual float MoveSpeed
    {
        get
        {
            return _MoveSpeed * Mathf.Exp(-BodyLength * GetMoveSpeedFactor(30, 0.6f));
        }
        protected set
        {
            _MoveSpeed = value;
        }
    }
    public int CharacterID { get; private set; }
    public float StrongRatio { get; private set; }
    public float Scores { get; private set; }

    /// <summary>
    /// total body length
    /// </summary>
    public int BodyLength
    {
        get
        {
            return _Bodies.Count;
        }
    }

    /// <summary>
    /// total body length and head
    /// </summary>
    public int TotalLength
    {
        get
        {
            return _Bodies.Count + 1;
        }
    }

    /// <summary>
    /// strong body length
    /// </summary>
    public int StrongLength
    {
        get
        {
            return _Bodies.Count + 1;
        }
    }

    public Body Head
    {
        get
        {
            return _Head;
        }
    }

    public PlayerData PlayerData_ { private set; get; }

    // set body in order. 
    List<Body> _Bodies = new List<Body>();
    [SerializeField] Head _Head;

    public virtual void SetData(PlayerData data, int initBodyLength, float strongRatio = ConstValue._DefaultStrongRatio)
    {
        if (data == null)
        {
            return;
        }

        PlayerData_ = data;
        CharacterID = data._ID;
        MoveSpeed = data._MoveSpeed;
        StrongRatio = strongRatio;
        Head.SetData(this, 0, null);
        for (int i = 0; i < initBodyLength; i++)
        {
            AddBody();
        }
    }

    float GetMoveSpeedFactor(int x, float y)
    {
        if (x <= 0)
        {
            return 1;
        }
        return -Mathf.Log(y) / x;
    }

    public virtual bool Move(Vector3 pos)
    {
        var moveSuccess = _Head.Move(pos);
        if (!moveSuccess)
        {
            return false;
        }

        for (int i = 0, length = _Bodies.Count; i < length; i++)
        {
            var b = _Bodies[i];
            b.UpdatePos();
        }
        return true;
    }

    public virtual void Attack(Body body)
    {

    }

    public virtual void AddBody()
    {
        var body = GameManager.instance.RespawnBody();
        if (_Bodies.Count == 0)
        {
            body.transform.position = _Head.transform.position + -_Head.transform.right * body.Size.x;
        }
        else
        {
            var b = _Bodies[_Bodies.Count - 1];
            body.transform.position = b.transform.position + -b.transform.right * body.Size.x;
        }
        body.SetData(this, _Bodies.Count, _Bodies.Count == 0 ? _Head : _Bodies[_Bodies.Count - 1]);
        _Bodies.Add(body);
        body.transform.SetParent(this.transform);
        UpdateStrongBody();
    }

    public virtual Body GetBody(int index)
    {
        if (index < 0 || index > _Bodies.Count - 1)
        {
            return null;
        }
        return _Bodies[index];
    }

    public virtual void RemoveBody(int index)
    {
        for (int i = index, length = _Bodies.Count; i < length; i++)
        {
            var b = _Bodies[i];
            b.Break();
            //GameObject.Destroy(b.gameObject); 
        }
        Debug.LogFormat("RemoveBody rangeCount={0}, prev={1}", _Bodies.Count - index, _Bodies.Count);
        _Bodies.RemoveRange(index, _Bodies.Count - index);
        Debug.LogFormat("RemoveBody index={0}, now={1}", index, _Bodies.Count);
        UpdateStrongBody();
    }

    void UpdateStrongBody()
    {
        for (int i = 0, length = _Bodies.Count; i < length; i++)
        {
            var b = _Bodies[i];
            b.UpdateStrongBody(i < Mathf.RoundToInt(StrongRatio * (length - 1)));
        }
    }

    public virtual void Die()
    {
        RemoveBody(0);
        _Head.Break();
        GameManager.instance.RemoveCharacter(this);
        GameObject.Destroy(this.gameObject);
    }

    public virtual void AddScore(float delta)
    {
        Scores += delta;
        while (Scores >= 1)
        {
            Scores -= 1;
            AddBody();
        }
    }
}
