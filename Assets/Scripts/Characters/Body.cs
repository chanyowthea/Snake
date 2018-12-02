using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour, IScore, IAddStrongBody
{
    [SerializeField] protected SpriteRenderer _Sprite;
    [SerializeField] protected CircleCollider2D _Collider;
    protected Body _PrevBody;
    public BaseCharacter _Character { protected set; get; }

    public int Index { protected set; get; }
    public bool IsStrong { protected set; get; }

    public float Radius
    {
        get
        {
            return _Collider.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
        }
    }

    public virtual void SetData(BaseCharacter character, int index, Body prev)
    {
        SetMask("Body");
        _Character = character;
        Index = index;
        _PrevBody = prev;
    }

    public virtual void UpdatePos()
    {
        var tailPos = _PrevBody.transform.position + -_PrevBody.transform.right * _PrevBody.Radius;
        if (Vector3.Distance(tailPos, transform.position) < Radius)
        {
            return;
        }
        var dir = (tailPos - transform.position).normalized;
        transform.position = tailPos + -dir * Radius;
        transform.right = dir;
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
}
