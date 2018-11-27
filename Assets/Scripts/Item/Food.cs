using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour, IScore, IAddStrongBody
{
    [SerializeField] BoxCollider2D _Collider;
    [SerializeField] SpriteRenderer _Sprite;

    public Vector2 Size
    {
        get
        {
            return new Vector2(_Collider.size.x * transform.lossyScale.x, _Collider.size.y * transform.lossyScale.y);
        }
    }
    public FoodData FoodData_ { private set; get; }

    public BoxCollider2D GetCollider()
    {
        return _Collider;
    }

    public virtual float GetScore()
    {
        return ConstValue._ScoreUnit;
    }

    public bool IsAddStrongBody()
    {
        return FoodData_._IsAddStrongBody; 
    }

    public void SetData(FoodData data)
    {
        if (data == null)
        {
            return;
        }
        FoodData_ = data;
        _Sprite.color = FoodData_._Color; 
    }
}
