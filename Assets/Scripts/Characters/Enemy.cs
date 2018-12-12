using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TsiU;
using System;

public class Enemy : BaseCharacter
{
    [SerializeField] PlayerCamera _PlayerCamera;
    private TBTAction _BevTree;
    private BotWorkingData _BevWorkingData;
    BotAStar _PathUtil;
    public override void SetData(PlayerInfo data, int initBodyLength)
    {
        _BevTree = BotFactory.GetBehaviourTree();
        _BevWorkingData = new BotWorkingData();
        _BevWorkingData._Character = this;
        base.SetData(data, initBodyLength);

        _PathUtil = this.gameObject.AddComponent<BotAStar>();
        _PathUtil.SetData(this);
        _PlayerCamera.SetData(_Head.transform);
    }

    public override void ClearData()
    {
        _BevTree = null;
        _BevWorkingData = null;
        _PathUtil.ClearData();
        base.ClearData();
    }

    public override void Die()
    {
        GameObject.Destroy(_PathUtil);
        base.Die();
        GameManager.instance.RemoveCharacter(this);
        GameManager.instance.RespawnCharacter(RandomUtil.instance.Next(1, 3), CharacterUniqueID);
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(1))
        {
            var targetPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
            _PathUtil.SteerToTargetPos(targetPos, null);
        }
#endif
        UpdateBehavior(Singleton._DelayUtil.GameTime, Singleton._DelayUtil.Timer.DeltaTime);
    }

    //public override void AddBody(bool isStrong = false)
    //{

    //}

    public int UpdateBehavior(float gameTime, float deltaTime)
    {
        _BevWorkingData._GameTime = gameTime;
        _BevWorkingData._DeltaTime = deltaTime;
        if (_BevTree.Evaluate(_BevWorkingData))
        {
            _BevTree.Update(_BevWorkingData);
        }
        else
        {
            _BevTree.Transition(_BevWorkingData);
        }
        return 0;
    }

    Collider2D[] _CheckColliders = new Collider2D[64];
    public void CheckEnemy()
    {
        UnityEngine.Profiling.Profiler.BeginSample("Check Non Alloc");
        var collidersCount = Physics2D.OverlapCircleNonAlloc(this.Head.transform.position, VisualField, _CheckColliders);
        float minDis = float.MaxValue;
        Body target = null;
        Food targetFood = null;
        UnityEngine.Profiling.Profiler.EndSample();

        UnityEngine.Profiling.Profiler.BeginSample("Get Nearest Body");
        // get nearest body. 
        List<Body> bodies = new List<Body>();
        for (int i = 0, length = collidersCount; i < length; i++)
        {
            var collider = _CheckColliders[i];
            var enemyBody = collider.GetComponent<Body>();
            if (enemyBody == null)
            {
                continue;
            }
            if (enemyBody._Character != null)
            {
                if (enemyBody._Character == this)
                {
                    continue;
                }
                if (enemyBody.IsStrong)
                {
                    continue;
                }
                if (this.TotalLength <= enemyBody._Character.TotalLength)
                {
                    continue;
                }
            }
            bodies.Add(enemyBody);
        }
        UnityEngine.Profiling.Profiler.EndSample();

        UnityEngine.Profiling.Profiler.BeginSample("Sort Bodies");
        // sort bodies from small to large. 
        int minIndex = 0;
        for (int i = 0, length = bodies.Count - 1; i < length; i++)
        {
            minIndex = i;
            for (int j = i + 1, max = bodies.Count - 1; j < max; j++)
            {
                var value = bodies[minIndex];
                var curValue = bodies[j];
                var dis = Vector3.Distance(this.Head.transform.position, value.transform.position);
                var curDis = Vector3.Distance(this.Head.transform.position, curValue.transform.position);
                if (dis > curDis)
                {
                    minIndex = j;
                }
            }
            if (i != minIndex)
            {
                var temp = bodies[i];
                bodies[i] = bodies[minIndex];
                bodies[minIndex] = temp;
            }
        }
        UnityEngine.Profiling.Profiler.EndSample();

        //UnityEngine.Profiling.Profiler.BeginSample("Find Path For Body");
        // find path for body
        //var bodyPath = new List<Vector3>();
        //for (int i = 0, length = bodies.Count; i < length; i++)
        //{
        //    var body = bodies[i];
        //    bool findFath = false;
        //    var ps = body.GetAllChasePoints();
        //    for (int j = 0, max = ps.Count; j < max; j++)
        //    {
        //        var point = ps[j];
        //        var tempPath = _PathUtil.FindPath(this.Head.transform.position, point.position);
        //        if (tempPath != null && tempPath.Count > 0)
        //        {
        //            target = body;
        //            bodyPath = tempPath;
        //            minDis = Vector3.Distance(this.Head.transform.position, point.position);
        //            body._CurChasePoint = point;
        //            break;
        //        }
        //    }
        //    if (findFath)
        //    {
        //        break;
        //    }
        //}
        //UnityEngine.Profiling.Profiler.EndSample();
        if (bodies.Count > 0)
        {
            target = bodies[0];
        }

        UnityEngine.Profiling.Profiler.BeginSample("Get Nearest Food");
        // get nearest food.        
        List<Food> foods = new List<Food>();
        float minDisFood = float.MaxValue;
        for (int i = 0, length = collidersCount; i < length; i++)
        {
            var collider = _CheckColliders[i];
            var food = collider.GetComponent<Food>();
            if (food == null)
            {
                continue;
            }
            foods.Add(food);
        }
        UnityEngine.Profiling.Profiler.EndSample();

        UnityEngine.Profiling.Profiler.BeginSample("Sort Foods");
        // sort foods from small to large. 
        minIndex = 0;
        for (int i = 0, length = foods.Count - 1; i < length; i++)
        {
            minIndex = i;
            for (int j = i + 1, max = foods.Count - 1; j < max; j++)
            {
                var food = foods[minIndex];
                var curFood = foods[j];
                var dis = Vector3.Distance(this.Head.transform.position, food.transform.position);
                var curDis = Vector3.Distance(this.Head.transform.position, curFood.transform.position);
                // 除了distance，还要考虑分数
                if (dis > curDis)
                {
                    minIndex = j;
                }
            }
            if (i != minIndex)
            {
                var temp = foods[i];
                foods[i] = foods[minIndex];
                foods[minIndex] = temp;
            }
        }
        UnityEngine.Profiling.Profiler.EndSample();
        //UnityEngine.Profiling.Profiler.BeginSample("Find Path For Food");
        //// find path for food
        //var foodPath = new List<Vector3>();
        //for (int i = 0, length = foods.Count; i < length; i++)
        //{
        //    var food = foods[i];
        //    var tempPath = _PathUtil.FindPath(this.Head.transform.position, food.transform.position);
        //    if (tempPath != null && tempPath.Count > 0)
        //    {
        //        targetFood = food;
        //        foodPath = tempPath;
        //        minDisFood = Vector3.Distance(this.Head.transform.position, food.transform.position);
        //        break;
        //    }
        //}
        //UnityEngine.Profiling.Profiler.EndSample();
        if (foods.Count > 0)
        {
            targetFood = foods[0];
        }
        UnityEngine.Profiling.Profiler.BeginSample("Final Decide Target");
        if (target != null || targetFood != null)
        {
            if (target == null && targetFood != null)
            {
                SetTargetEnemy(targetFood.transform);
            }
            else if (target != null && targetFood == null)
            {
                SetTargetEnemy(target.transform);
            }
            else if (target != null && targetFood != null)
            {
                // if the target body is too far, eat food first. 
                if (minDis > minDisFood * ConstValue._OneBodyScores / (ConstValue._ScoreUnit * 2))
                {
                    SetTargetEnemy(targetFood.transform);
                }
                else
                {
                    if (target is Head)
                    {
                        target._CurChasePoint = target.GetAllChasePoints()[1];
                    }
                    else if (target._Character != null)
                    {
                        target._CurChasePoint = target.GetAllChasePoints()[2];
                    }
                    SetTargetEnemy(target.transform);
                }
            }
        }
        UnityEngine.Profiling.Profiler.EndSample();
    }

    public void SteerToTargetPos(Vector3 pos, Action onFinish, Action onFailed = null)
    {
        _PathUtil.SteerToTargetPos(pos, onFinish, onFailed);
        //Singleton._PathUtil.AddToPathFindingQueue(() => _PathUtil.SteerToTargetPos(pos, onFinish, onFailed));
    }

    public bool IsSteering()
    {
        return _PathUtil._IsInSteer;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (this.Head == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(this.Head.transform.position, VisualField);
    }
#endif
}
