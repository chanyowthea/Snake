using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Body : BaseMonoObject, IScore, IAddStrongBody
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

    public virtual void ClearData()
    {
        _Character = null;
        Index = -1;
        _PrevBody = null;
        ClearAllChasePoints();
        _ChangeDirTimes = 0;
        _Motion0 = Vector3.zero;
        IsStrong = false;
    }

    public override void OnAllocated()
    {
        base.OnAllocated();
    }

    public override void OnCollected()
    {
        base.OnCollected();
    }

    public virtual void UpdatePos()
    {
        var points = _PrevBody.GetAllChasePoints();
        Assert.IsTrue(points.Count == 5, string.Format("count={0}, name={1}, index={2}", points.Count, _PrevBody._Character.Name, _PrevBody.Index));
        var tailPos = _PrevBody.transform.position + -points[0].right * _PrevBody.Radius;
        if (Vector3.Distance(tailPos, transform.position) < Radius)
        {
            return;
        }
        var dir = (tailPos - transform.position).normalized;
        transform.right = dir.normalized;
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

    protected virtual void CreateChasePoints()
    {
        Vector3[] bodyOffsets = new Vector3[] { new Vector3(0, 0), new Vector3(1, 0), new Vector3(-1, 0), new Vector3(0, 1), new Vector3(0, -1) };
        for (int i = 0, length = bodyOffsets.Length; i < length; i++)
        {
            var offset = bodyOffsets[i];
            float angle = Vector3.Angle(Vector3.right, this.transform.right);
            if (this.transform.position.y < 0)
            {
                angle *= -1;
            }
            offset = MathUtil.V3RotateAround(offset, -Vector3.forward, -angle);
            Vector3 pos = this.transform.position + offset * this.Radius;
            var point = new GameObject("ChasePoint" + i).transform;
            point.position = pos;
            point.SetParent(this.transform);
            _ChasePoints.Add(point);
        }
    }

    void ClearAllChasePoints()
    {
        for (int i = 0, length = _ChasePoints.Count; i < length; i++)
        {
            var point = _ChasePoints[i];
            GameObject.Destroy(point.gameObject);
        }
        _ChasePoints.Clear();
        _CurChasePoint = null;
    }
}
