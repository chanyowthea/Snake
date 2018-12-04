using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head : Body
{
    Vector3 _LastPos;
    //int _ChangeDirTimes = 0;
    //Vector3 _Motion0;
    //bool _TurnRight;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(_LastPos, Radius);
    }

    public bool Move(Vector3 pos)
    {
        Vector3 targetPos = this.transform.position + pos;
        var colliders = Physics2D.OverlapCircleAll(targetPos, Radius);
        _LastPos = targetPos;

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
            _ChangeDirTimes = 0;
            //Debugger.LogBlue("pass pos=" + pos);
        }
        else
        {
            if (_ChangeDirTimes == 0)
            {
                Vector3 dir = Vector3.zero;
                int rs = (int)(Mathf.Abs(pos.x) - Mathf.Abs(pos.y));
                if (rs > 0)
                {
                    dir = new Vector3(1 * Mathf.Sign(pos.x), 0, 0);
                }
                else if (rs < 0)
                {
                    dir = new Vector3(0, 1 * Mathf.Sign(pos.y), 0);
                }
                else
                {
                    dir = _TurnRight ? new Vector3(1 * Mathf.Sign(pos.x), 0, 0) : new Vector3(0, 1 * Mathf.Sign(pos.y), 0);
                }
                _Motion0 = pos.magnitude * dir.normalized;
                //Debugger.LogGreen("Motion=" + _Motion0);
            }
            _ChangeDirTimes += 1;
            // left and right only. 
            if (_ChangeDirTimes == 1)
            {
                pass = Move(_Motion0);
            }
            else if (_ChangeDirTimes >= 2 && _ChangeDirTimes < 4)
            {
                // while the _ChangeDirTimes == 1 is failed. 
                if (_ChangeDirTimes == 3)
                {
                    _TurnRight = !_TurnRight;
                }

                //if (_Character.CharacterID != 0)
                //    Debugger.LogGreen(string.Format("pass={0}, times={1}, pos={2}", pass,
                        //_ChangeDirTimes,
                        //MathUtil.V3RotateAround(_Motion0, -Vector3.forward, -90 * (_TurnRight ? 1 : -1))));
                pass = Move(MathUtil.V3RotateAround(_Motion0, -Vector3.forward, -90 * (_TurnRight ? 1 : -1)));
            }
            else
            {
                pass = false;
            }
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
                if (body != null)
                {
                    Eat(body.GetCollider());
                }
                _Character.Kill(body._Character);
                body._Character.Die();
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

    void Eat(CircleCollider2D food)
    {
        if (food != null)
        {
            //Debug.Log("Eat food=" + food.radius);
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
        return ConstValue._HeadScores + ((_Character.BodyLength - _Character.StrongLength) * ConstValue._OneBodyScores
            + _Character.StrongLength * ConstValue._StrongBodyScores) * ConstValue._KillScoresRatio;
    }
}
