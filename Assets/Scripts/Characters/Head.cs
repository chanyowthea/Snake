using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head : Body
{
    public CharacterName CharacterName
    {
        get
        {
            return _CharacterName;
        }
    }

    [SerializeField] CharacterName _CharacterName;
    Vector3 _LastPos;
    float _LastCollidedTime;
    Vector3 _LastCollidedPos;
    float _CollidedGapTime = 2f;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(_LastPos, Radius);
    }

    Collider2D[] _CheckColliders = new Collider2D[16];
    public bool Move(Vector3 pos)
    {
        Vector3 targetPos = this.transform.position + pos;
        var collidersCount = Physics2D.OverlapCircleNonAlloc(targetPos, Radius, _CheckColliders);
        _LastPos = targetPos;

        bool pass = true;
        Body attackTarget = null;
        if (collidersCount > 0)
        {
            for (int i = 0, length = collidersCount; i < length; i++)
            {
                var col = _CheckColliders[i];
                if (col.gameObject.layer == LayerMask.NameToLayer("Barrier"))
                {
                    pass = false;
                    break;
                }

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
                        // if there is strong body in the road, cannot eat any food in the road. 
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
            _OriginMotion1 = Vector3.zero;
            this.transform.position += pos;
            this.transform.right = pos.normalized;
            this._CharacterName.transform.right = Vector3.right;
            _ChangeDirTimes = 0;
            //Debugger.LogBlue("pass pos=" + pos);
        }
        else
        {
            // revert the change direction times 
            if (Vector3.Angle(_OriginMotion1, pos) > 90)
            {
                _ChangeDirTimes = 0;
            }
            _ChangeDirTimes += 1;
            // left, right, top and down. 
            if (_OriginMotion1 == Vector3.zero)
            {
                _OriginMotion1 = pos;
                //Debugger.LogGreen(string.Format("start _OriginMotion1=x{0}, y{1}", _OriginMotion1.x, _OriginMotion1.y));
            }
            if (_OriginMotion1.x == 0 || _OriginMotion1.y == 0)
            {
                if (_ChangeDirTimes == 1)
                {
                    // only the times is 1 can set the origin motion. 
                    Vector3 dir = Vector3.zero;
                    if (_OriginMotion1.x > 0)
                    {
                        dir = _TurnRight ? new Vector3(0, -1, 0) : new Vector3(0, 1, 0);
                    }
                    else if (_OriginMotion1.x < 0)
                    {
                        dir = _TurnRight ? new Vector3(0, 1, 0) : new Vector3(0, -1, 0);
                    }
                    else if (_OriginMotion1.y > 0)
                    {
                        dir = _TurnRight ? new Vector3(1, 0, 0) : new Vector3(-1, 0, 0);
                    }
                    else if (_OriginMotion1.y < 0)
                    {
                        dir = _TurnRight ? new Vector3(-1, 0, 0) : new Vector3(1, 0, 0);
                    }
                    _Motion1 = _OriginMotion1.magnitude * dir.normalized;
                    //Debugger.LogGreen("Motion1=" + _Motion1);
                    pass = Move(_Motion1);
                }
                else if (_ChangeDirTimes == 2)
                {
                    _TurnRight = !_TurnRight;
                    Vector3 dir = Vector3.zero;
                    if (_OriginMotion1.x > 0)
                    {
                        dir = _TurnRight ? new Vector3(0, -1, 0) : new Vector3(0, 1, 0);
                    }
                    else if (_OriginMotion1.x < 0)
                    {
                        dir = _TurnRight ? new Vector3(0, 1, 0) : new Vector3(0, -1, 0);
                    }
                    else if (_OriginMotion1.y > 0)
                    {
                        dir = _TurnRight ? new Vector3(1, 0, 0) : new Vector3(-1, 0, 0);
                    }
                    else if (_OriginMotion1.y < 0)
                    {
                        dir = _TurnRight ? new Vector3(-1, 0, 0) : new Vector3(1, 0, 0);
                    }
                    _Motion1 = _OriginMotion1.magnitude * dir.normalized;
                    //Debugger.LogGreen("Motion1=" + _Motion1);
                    pass = Move(_Motion1);
                }
            }
            else
            {
                // strategy 1
                if (_ChangeDirTimes == 1)
                {
                    //Debugger.LogGreen(string.Format("else times=1 _OriginMotion1=x{0}, y{1}", _OriginMotion1.x, _OriginMotion1.y));
                    Vector3 dir = Vector3.zero;
                    int rs = (int)(Mathf.Abs(_OriginMotion1.x) - Mathf.Abs(_OriginMotion1.y));
                    if (rs > 0)
                    {
                        dir = new Vector3(1 * Mathf.Sign(_OriginMotion1.x), 0, 0);
                    }
                    else if (rs < 0)
                    {
                        dir = new Vector3(0, 1 * Mathf.Sign(_OriginMotion1.y), 0);
                    }
                    // abs x == abs y
                    else
                    {
                        var sx = Mathf.Sign(_OriginMotion1.x);
                        var sy = Mathf.Sign(_OriginMotion1.y);
                        if (sx > 0 && sy > 0 || sx < 0 && sy < 0)
                        {
                            dir = _TurnRight ? new Vector3(1 * sx, 0, 0) : new Vector3(0, 1 * sy, 0);
                        }
                        else if (sx > 0 && sy < 0 || sx < 0 && sy > 0)
                        {
                            dir = _TurnRight ? new Vector3(0, 1 * sy, 0) : new Vector3(1 * sx, 0, 0);
                        }
                    }
                    _Motion1 = _OriginMotion1.magnitude * dir.normalized;
                    //Debugger.LogGreen("Motion1=" + _Motion1);
                    //Debugger.LogError("collider time=" + _LastCollidedTime);
                    pass = Move(_Motion1);
                }
                else if (_ChangeDirTimes == 2)
                {
                    Vector3 dir = Vector3.zero;
                    int rs = (int)(Mathf.Abs(_OriginMotion1.x) - Mathf.Abs(_OriginMotion1.y));
                    if (rs > 0)
                    {
                        dir = new Vector3(0, 1 * Mathf.Sign(_OriginMotion1.y), 0);
                    }
                    else if (rs < 0)
                    {
                        dir = new Vector3(1 * Mathf.Sign(_OriginMotion1.x), 0, 0);
                    }
                    else
                    {
                        _TurnRight = !_TurnRight;
                        var sx = Mathf.Sign(_OriginMotion1.x);
                        var sy = Mathf.Sign(_OriginMotion1.y);
                        if (sx > 0 && sy > 0 || sx < 0 && sy < 0)
                        {
                            dir = _TurnRight ? new Vector3(1 * sx, 0, 0) : new Vector3(0, 1 * sy, 0);
                        }
                        else if (sx > 0 && sy < 0 || sx < 0 && sy > 0)
                        {
                            dir = _TurnRight ? new Vector3(0, 1 * sy, 0) : new Vector3(1 * sx, 0, 0);
                        }
                    }
                    _Motion1 = _OriginMotion1.magnitude * dir.normalized;
                    //Debugger.LogGreen("Motion1 2=" + _Motion1);
                    //Debugger.LogError("collider time=" + _LastCollidedTime);
                    pass = Move(_Motion1);
                }
            }

            //else if (_ChangeDirTimes >= 2 && _ChangeDirTimes < 4)
            //{
            //    Debugger.LogGreen("_ChangeDirTimes=" + _ChangeDirTimes + ", _OriginMotion1=" + _OriginMotion1);
            //    //if (Singleton._DelayUtil.GameTime - _LastCollidedTime > _CollidedGapTime
            //    //    || Vector3.Distance(transform.position, _LastCollidedPos) > RunTimeData._MinMoveDelta * 0.1f)
            //    {
            //        //_LastCollidedPos = this.transform.position;
            //        //_LastCollidedTime = Singleton._DelayUtil.GameTime;
            //        // while the _ChangeDirTimes == 1 is failed. 
            //        if (_ChangeDirTimes == 3)
            //        {
            //            _TurnRight = !_TurnRight;
            //        }

            //        //if (_Character.CharacterID != 0)
            //        //    Debugger.LogGreen(string.Format("pass={0}, times={1}, pos={2}", pass,
            //        //_ChangeDirTimes,
            //        //MathUtil.V3RotateAround(_Motion0, -Vector3.forward, -90 * (_TurnRight ? 1 : -1))));
            //        pass = Move(MathUtil.V3RotateAround(_OriginMotion1, -Vector3.forward, -90 * (_TurnRight ? 1 : -1)));
            //    }
            //}
        }
        if (!pass)
        {
            //_ChangeDirTimes = 0;
            //_LastCollidedTime = 0;
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
            return true;
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
                    body._Character.Die();
                }
                _Character.Kill(body._Character);
                return true;
            }
            return false;
        }
        else
        {
            if (body._Character == null)
            {
                Eat(body.GetCollider());
                GameManager.instance.RemoveBody(body);
            }
            else
            {
                var food = body._Character.GetBody(body.Index);
                body._Character.RemoveBody(body.Index);
                if (food != null)
                {
                    Eat(food.GetCollider());
                    GameManager.instance.RemoveBody(body);
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
            GameManager.instance.RemoveFood(food);
        }
        return true;
    }

    void Eat(CircleCollider2D food)
    {
        if (food != null)
        {
            //Debug.Log("Eat food=" + food.radius);
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
