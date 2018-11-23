using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCharacter : MonoBehaviour
{
    public float MoveSpeed { get; private set; }
    public int CharacterID { get; private set; }

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

    // set body in order. 
    List<Body> _Bodies = new List<Body>();
    [SerializeField] Body _Head; 

    public virtual void SetData(int characterId)
    {
        CharacterID = characterId;
        MoveSpeed = 0.2f; 
    }

    public virtual void Move(Vector2 pos)
    {

    }

    public virtual void Attack(Body body)
    {

    }

    public virtual void AddBody()
    {

    }

    public virtual void RemoveBody(int index)
    {

    }
}
