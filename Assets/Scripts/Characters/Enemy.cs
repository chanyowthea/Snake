using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TsiU;

public class Enemy : BaseCharacter
{
    private TBTAction _BevTree;
    private BotWorkingData _BevWorkingData;
    PathFindingUtil _PathUtil;
    public override void SetData(PlayerInfo data, int initBodyLength)
    {
        _PathUtil = new PathFindingUtil();
        _PathUtil.SetData(this);

        _BevTree = BotFactory.GetBehaviourTree();
        _BevWorkingData = new BotWorkingData();
        _BevWorkingData._Character = this;
        base.SetData(data, initBodyLength);
    }

    public override void Die()
    {
        base.Die();
        GameManager.instance.RespawnCharacter(RandomUtil.instance.Next(1, 3), CharacterUniqueID);
    }

    //bool _DrawGizmo;
    void Update()
    {
        //_DrawGizmo = Input.GetKey(KeyCode.G);
        _PathUtil.Update();
        UpdateBehavior(GameManager.instance.GameTime, GameManager.instance.DeltaTime);
    }

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

    public void CheckEnemy()
    {
        var colliders = Physics2D.OverlapCircleAll(this.Head.transform.position, VisualField);
        float minDis = float.MaxValue;
        Body target = null;
        Food targetFood = null;

        // get nearest body. 
        List<Body> bodies = new List<Body>();
        for (int i = 0, length = colliders.Length; i < length; i++)
        {
            var collider = colliders[i];
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

        // find path for body
        var bodyPath = new List<Vector3>();
        for (int i = 0, length = bodies.Count; i < length; i++)
        {
            var body = bodies[i];
            var tempPath = _PathUtil.FindPath(this.Head.transform.position, body.transform.position);
            if (tempPath != null && tempPath.Count > 0)
            {
                target = body;
                bodyPath = tempPath;
                minDis = Vector3.Distance(this.Head.transform.position, body.transform.position);
                break;
            }
        }


        // get nearest food.        
        List<Food> foods = new List<Food>();
        float minDisFood = float.MaxValue;
        for (int i = 0, length = colliders.Length; i < length; i++)
        {
            var collider = colliders[i];
            var food = collider.GetComponent<Food>();
            if (food == null)
            {
                continue;
            }
            foods.Add(food);
        }

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
        // find path for food
        var foodPath = new List<Vector3>();
        for (int i = 0, length = foods.Count; i < length; i++)
        {
            var food = foods[i];
            var tempPath = _PathUtil.FindPath(this.Head.transform.position, food.transform.position);
            if (tempPath != null && tempPath.Count > 0)
            {
                targetFood = food;
                foodPath = tempPath;
                minDisFood = Vector3.Distance(this.Head.transform.position, food.transform.position);
                break;
            }
        }

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
                    SetTargetEnemy(target.transform);
                }
            }
        }
    }

    public void SteerToTargetPos(Vector3 pos)
    {
        _PathUtil.SteerToTargetPos(pos);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(this.Head.transform.position, VisualField);
        //if (_DrawGizmo)
        //{
        //    _PathUtil.ResetMap();
        //    _PathUtil.DrawGrid();
        //    _PathUtil.OnDrawGizmos();
        //}
    }
}
