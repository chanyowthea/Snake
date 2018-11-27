using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head : Body
{
    Vector3 lastPos;
    Vector2 lastSize;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(lastPos, Size);
    }

    public bool Move(Vector3 pos)
    {
        Vector3 targetPos = this.transform.position + pos;
        var colliders = Physics2D.OverlapBoxAll(targetPos, Size, 0);
        lastPos = targetPos;
        lastSize = Size;

        bool pass = true;
        Body attackTarget = null;
        if (colliders != null)
        {
            for (int i = 0, length = colliders.Length; i < length; i++)
            {
                var col = colliders[i];
                var body = col.GetComponent<Body>();
                if (body != null)
                {
                    if (body._Character == this._Character)
                    {
                        //pass = true;
                    }
                    else
                    {
                        if (attackTarget == null)
                        {
                            attackTarget = body;
                        }
                        // strong is priority
                        else if ((body.IsStrong && body._Character != null) && !attackTarget.IsStrong)
                        {
                            attackTarget = body;
                        }
                    }
                }
                // cannot pass wall
                else
                {
                    // attack enemy is priority. 
                    if (attackTarget == null)
                    {
                        var food = col.GetComponent<Food>();
                        if (food != null)
                        {
                            pass = TryAttack(food);
                        }
                        else
                        {
                            pass = false;
                        }
                    }
                    else
                    {
                        pass = false;
                    }
                    break;
                }
            }
        }
        if (attackTarget != null)
        {
            pass = TryAttack(attackTarget);
        }
        if (pass)
        {
            this.transform.position += pos;
            this.transform.right = pos.normalized;
        }
        return pass;
    }

    /// <summary>
    /// try attack other
    /// </summary>
    /// <param name="body"></param>
    /// <returns> can pass the road? </returns>
    bool TryAttack(Body body)
    {
        if (body == null)
        {
            return true;
        }
        if (body._Character == this._Character)
        {
            return true;
        }
        if (body.IsStrong && body._Character != null)
        {
            return false;
        }
        if (body is Head)
        {
            // if the snake attack the head of the other snake whose length is less than this, 
            // then the other snake would die. 
            if (body._Character.TotalLength < this._Character.TotalLength)
            {
                body._Character.Die();
                if (body != null)
                {
                    Eat(body.GetCollider());
                }
                return true;
            }
            return false;
        }
        else
        {
            if (body._Character == null)
            {
                Eat(body.GetCollider());
            }
            else
            {
                var food = body._Character.GetBody(body.Index);
                body._Character.RemoveBody(body.Index);
                if (food != null)
                {
                    Eat(food.GetCollider());
                }
            }
            return true;
        }
    }

    /// <summary>
    /// try attack food
    /// </summary>
    /// <param name="food"></param>
    /// <returns> can pass the road? </returns>
    bool TryAttack(Food food)
    {
        if (food == null)
        {
            return true;
        }
        if (food != null)
        {
            Eat(food.GetCollider());
        }
        return true;
    }

    void Eat(BoxCollider2D food)
    {
        if (food != null)
        {
            Debug.Log("Eat food=" + food.size);
            GameObject.Destroy(food.gameObject);
            var iAdd = food.GetComponent<IAddStrongBody>();
            if (iAdd != null)
            {
                if (iAdd.IsAddStrongBody())
                {
                    _Character.AddBody(true);
                }
            }
            var iScore = food.GetComponent<IScore>();
            if (iScore != null)
            {
                _Character.AddScore(iScore.GetScore(), !iAdd.IsAddStrongBody());
            }
        }
    }

    public override float GetScore()
    {
        // TODO
        return ConstValue._HeadScores;
    }
}
