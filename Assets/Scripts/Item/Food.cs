using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Food : BaseMonoObject, IScore, IAddStrongBody
{
    [SerializeField] CircleCollider2D _Collider;
    [SerializeField] SpriteRenderer _Sprite;

    public float Radius
    {
        get
        {
            return _Collider.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
        }
    }

    public FoodData FoodData_ { private set; get; }

    public CircleCollider2D GetCollider()
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

    public void ClearData()
    {
        Assert.IsNotNull(FoodData_);
        FoodData_ = null;
    }
}
