using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCharacter : MonoBehaviour, IComparable
{
    protected float _MoveSpeed;
    public virtual float MoveSpeed
    {
        get
        {
            return _MoveSpeed * Mathf.Exp(-BodyLength * GetMoveSpeedFactor(30, 0.6f)) * GameManager.instance.TimeScale;
        }
        protected set
        {
            _MoveSpeed = value;
        }
    }
    public int CharacterID
    {
        get
        {
            if (PlayerInfo_ == null || PlayerInfo_._PlayerData == null)
            {
                return int.MinValue;
            }
            return PlayerInfo_._PlayerData._ID;
        }
    }

    public uint CharacterUniqueID
    {
        get
        {
            if (PlayerInfo_ == null)
            {
                return uint.MinValue;
            }
            return PlayerInfo_._UniqueID;
        }
    }

    float _Scores;
    public float Scores
    {
        get
        {
            return _Scores;
        }

        protected set
        {
            _Scores = value;
            RunTimeData.instance.UpdateScores(CharacterUniqueID, (int)_Scores);
        }
    }
    protected float LengthPoints;
    public string Name
    {
        get
        {
            if (PlayerInfo_ == null)
            {
                return "";
            }
            return PlayerInfo_._Name;
        }
    }

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
    public int StrongLength { private set; get; }

    public Body Head
    {
        get
        {
            return _Head;
        }
    }

    public PlayerData PlayerData_
    {
        get
        {
            if (PlayerInfo_ == null)
            {
                return null;
            }
            return PlayerInfo_._PlayerData;
        }
    }
    public PlayerInfo PlayerInfo_ { private set; get; }

    // set body in order. 
    List<Body> _Bodies = new List<Body>();
    [SerializeField] Head _Head;
    [SerializeField] CharacterName _CharacterName;

    public virtual void SetData(PlayerInfo data, int initBodyLength)
    {
        if (data == null || data._PlayerData == null)
        {
            return;
        }
        PlayerInfo_ = data; 
        _CharacterName.SetData(PlayerInfo_._Name);
        Scores = PlayerInfo_._Scores; 
        MoveSpeed = PlayerInfo_._PlayerData._MoveSpeed;
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

    public virtual void Kill(BaseCharacter character)
    {
        PlayerInfo_._KillCount += 1; 
    }

    public virtual void AddBody(bool isStrong = false)
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

        // set strong body information. 
        if (isStrong)
        {
            StrongLength += 1;
        }
        int minStrongLength = Mathf.RoundToInt(ConstValue._DefaultStrongRatio * BodyLength);
        StrongLength = StrongLength < minStrongLength ? minStrongLength : StrongLength;
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
        }
        _Bodies.RemoveRange(index, _Bodies.Count - index);
        if (StrongLength > BodyLength)
        {
            StrongLength = BodyLength;
        }
        UpdateStrongBody();
    }

    void UpdateStrongBody()
    {
        for (int i = 0, length = _Bodies.Count; i < length; i++)
        {
            var b = _Bodies[i];
            b.UpdateStrongBody(i < StrongLength);
        }
    }

    public virtual void Die()
    {
        PlayerInfo_._DieTimes += 1; 
        Scores -= ConstValue._MinusScorePerDie;
        if (Scores < 0)
        {
            Scores = 0;
        }
        RemoveBody(0);
        _Head.Break();
        GameManager.instance.RemoveCharacter(this);
        GameObject.Destroy(this.gameObject);
    }

    public virtual void AddScore(float delta, bool addBody = true)
    {
        Scores += delta;
        if (addBody)
        {
            LengthPoints += delta;
            while (LengthPoints >= ConstValue._OneBodyScores)
            {
                LengthPoints -= ConstValue._OneBodyScores;
                AddBody();
            }
        }
    }

    public int CompareTo(object obj)
    {
        int result = 1;
        if (obj != null && obj is BaseCharacter)
        {
            BaseCharacter person = (BaseCharacter)obj;
            result = -this.Scores.CompareTo(person.Scores);
        }
        return result;
    }
}
