using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour, IScore
{
    [SerializeField] protected SpriteRenderer _Sprite;
    [SerializeField] protected BoxCollider2D _Collider; // TODO circle？
    protected Body _PrevBody;
    public BaseCharacter _Character { protected set; get; }

    public int Index { protected set; get; }
    public bool IsStrong { protected set; get; }

    public Vector2 Size
    {
        get
        {
            return new Vector2(_Collider.size.x * transform.lossyScale.x, _Collider.size.y * transform.lossyScale.y);
        }
    }

    public virtual void SetData(BaseCharacter character, int index, Body prev)
    {
        _Character = character;
        Index = index;
        _PrevBody = prev;
    }

    public virtual void UpdatePos()
    {
        var tailPos = _PrevBody.transform.position + -_PrevBody.transform.right * _PrevBody.Size.x / 2f;
        if (Vector3.Distance(tailPos, transform.position) < Size.x / 2f)
        {
            return;
        }
        var dir = (tailPos - transform.position).normalized;
        transform.position = tailPos + -dir * Size.x / 2f;
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
        _Character = null;
        _PrevBody = null;
        transform.SetParent(GameManager.instance.FoodRoot.transform); 
    }

    public BoxCollider2D GetCollider()
    {
        return _Collider;
    }

    public virtual float GetScore()
    {
        return Size.x * 2; 
    }
}
