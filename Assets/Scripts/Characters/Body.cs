using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour, IScore, IAddStrongBody
{
    public BaseCharacter _Character { protected set; get; }
    [SerializeField] protected SpriteRenderer _Sprite;
    [SerializeField] protected CircleCollider2D _Collider;
    protected Body _PrevBody;
    protected List<Transform> _ChasePoints = new List<Transform>();
    public Transform _CurChasePoint;

    public int Index { protected set; get; }
    public bool IsStrong { protected set; get; }

    public float Radius
    {
        get
        {
            return _Collider.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
        }
    }

    protected int _ChangeDirTimes = 0;
    protected Vector3 _Motion0;
    protected bool _TurnRight;

    public virtual void SetData(BaseCharacter character, int index, Body prev)
    {
        SetMask("Body");
        _Character = character;
        Index = index;
        _PrevBody = prev;
        CreateChasePoints();
        _CurChasePoint = _ChasePoints[0];
    }

    //public bool Move(Vector3 pos)
    //{
    //    Vector3 targetPos = this.transform.position + pos;
    //    var colliders = Physics2D.OverlapCircleAll(targetPos, Radius);

    //    bool pass = true;
    //    if (colliders != null)
    //    {
    //        for (int i = 0, length = colliders.Length; i < length; i++)
    //        {
    //            var col = colliders[i];
    //            // cannot pass barrier. 
    //            if (col.gameObject.layer == LayerMask.NameToLayer("Barrier"))
    //            {
    //                pass = false;
    //                break;
    //            }

    //            var body = col.GetComponent<Body>();
    //            if (body != null && body._Character != null)
    //            {
    //                if (body._Character != this._Character)
    //                {
    //                    // cannot pass strong body or head. 
    //                    if (body is Head || body.IsStrong)
    //                    {
    //                        pass = false;
    //                        break; 
    //                    }
    //                }
    //            }
    //        }
    //    }
    //    if (pass)
    //    {
    //        this.transform.right = pos.normalized;
    //        this.transform.position += pos;
    //        _ChangeDirTimes = 0;
    //        Debugger.LogBlue("pass pos=" + pos);
    //    }
    //    //else
    //    //{
    //    //    if (_ChangeDirTimes == 0)
    //    //    {
    //    //        Vector3 dir = Vector3.zero;
    //    //        int rs = (int)(Mathf.Abs(pos.x) - Mathf.Abs(pos.y));
    //    //        if (rs > 0)
    //    //        {
    //    //            dir = new Vector3(1 * Mathf.Sign(pos.x), 0, 0);
    //    //        }
    //    //        else if (rs < 0)
    //    //        {
    //    //            dir = new Vector3(0, 1 * Mathf.Sign(pos.y), 0);
    //    //        }
    //    //        else
    //    //        {
    //    //            dir = _TurnRight ? new Vector3(1 * Mathf.Sign(pos.x), 0, 0) : new Vector3(0, 1 * Mathf.Sign(pos.y), 0);
    //    //        }
    //    //        _Motion0 = pos.magnitude * dir.normalized;
    //    //        Debugger.LogGreen("Motion=" + _Motion0);
    //    //    }
    //    //    _ChangeDirTimes += 1;
    //    //    // left and right only. 
    //    //    if (_ChangeDirTimes == 1)
    //    //    {
    //    //        pass = Move(_Motion0);
    //    //    }
    //    //    else if (_ChangeDirTimes >= 2 && _ChangeDirTimes < 4)
    //    //    {
    //    //        // while the _ChangeDirTimes == 1 is failed. 
    //    //        if (_ChangeDirTimes == 3)
    //    //        {
    //    //            _TurnRight = !_TurnRight;
    //    //        }

    //    //        if (_Character.CharacterID != 0)
    //    //            Debugger.LogGreen(string.Format("pass={0}, times={1}, pos={2}", pass,
    //    //                _ChangeDirTimes,
    //    //                MathUtil.V3RotateAround(_Motion0, -Vector3.forward, -90 * (_TurnRight ? 1 : -1))));
    //    //        pass = Move(MathUtil.V3RotateAround(_Motion0, -Vector3.forward, -90 * (_TurnRight ? 1 : -1)));
    //    //    }
    //    //    else
    //    //    {
    //    //        pass = false;
    //    //    }
    //    //}
    //    return pass;
    //}

    public virtual void UpdatePos()
    {
        var tailPos = _PrevBody.transform.position + -_PrevBody.transform.right * _PrevBody.Radius;
        if (Vector3.Distance(tailPos, transform.position) < Radius)
        {
            return;
        }
        var dir = (tailPos - transform.position).normalized;
        transform.right = dir; 
        transform.position = tailPos + -dir * Radius;
    }

    public virtual void UpdateStrongBody(bool isStrong)
    {
        IsStrong = isStrong;
        _Sprite.color = IsStrong ? CharacterUtil.GetStrongBodyColor(_Character.PlayerData_._BodyColor) : _Character.PlayerData_._BodyColor;
    }

    public virtual void SetColor(Color c)
    {
        _Sprite.color = c;
    }

    public void Break()
    {
        SetMask("Food");
        _Character = null;
        _PrevBody = null;
        transform.SetParent(GameManager.instance.FoodRoot.transform);
    }

    public void SetMask(string maskName)
    {
        this.gameObject.layer = LayerMask.NameToLayer(maskName);
        if (maskName == "Food")
        {
            _Sprite.sortingOrder = ConstValue._FoodMaskSortingLayer;
        }
        else if (maskName == "Body")
        {
            _Sprite.sortingOrder = ConstValue._BodyMaskSortingLayer;
        }
    }

    public CircleCollider2D GetCollider()
    {
        return _Collider;
    }

    public virtual float GetScore()
    {
        return IsStrong ? ConstValue._StrongBodyScores : ConstValue._OneBodyScores;
    }

    public bool IsAddStrongBody()
    {
        return IsStrong;
    }

    public List<Transform> GetAllChasePoints()
    {
        return _ChasePoints;
    }

    public void CreateChasePoints()
    {
        Vector3[] bodyOffsets = new Vector3[] { new Vector3(0, 0), new Vector3(1, 0), new Vector3(-1, 0), new Vector3(0, 1), new Vector3(0, -1) };
        for (int i = 0, length = bodyOffsets.Length; i < length; i++)
        {
            var offset = bodyOffsets[i];
            float angle = Vector3.Angle(Vector3.right, this.transform.right);
            offset = MathUtil.V3RotateAround(offset, -Vector3.forward, -angle);
            Vector3 pos = this.transform.position + offset * this.Radius;
            var point = new GameObject("ChasePoint" + i).transform;
            point.position = pos;
            point.SetParent(this.transform);
            _ChasePoints.Add(point);
        }
    }

    public void ClearAllChasePoints()
    {
        for (int i = 0, length = _ChasePoints.Count; i < length; i++)
        {
            var point = _ChasePoints[i];
            GameObject.Destroy(point.gameObject);
        }
    }
}
