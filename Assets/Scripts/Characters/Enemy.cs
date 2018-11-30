using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TsiU;

public class Enemy : BaseCharacter
{
    private TBTAction _BevTree;
    private BotWorkingData _BevWorkingData;
    public override void SetData(PlayerInfo data, int initBodyLength)
    {
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

    void Update()
    {
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

    // 寻路【A星】
    // 碰撞停止了之后的转弯
    // 吃食物
    // 
    public void CheckEnemy()
    {
        var colliders = Physics2D.OverlapCircleAll(this.Head.transform.position, VisualField);
        float minDis = float.MaxValue;
        Body target = null;
        Food targetFood = null;

        // get nearest body. 
        for (int i = 0, length = colliders.Length; i < length; i++)
        {
            var collider = colliders[i];
            var enemyBody = collider.GetComponent<Body>();
            if (enemyBody == null)
            {
                continue;
            }
            if (enemyBody._Character == this)
            {
                continue;
            }
            if (enemyBody.IsStrong && enemyBody._Character != null)
            {
                continue;
            }
            if (this.TotalLength <= enemyBody._Character.TotalLength)
            {
                continue;
            }
            var dis = Vector3.Distance(this.Head.transform.position, enemyBody.transform.position);
            if (dis < minDis)
            {
                minDis = dis;
                target = enemyBody;
            }
        }

        // get nearest food. 
        float minDisFood = float.MaxValue;
        for (int i = 0, length = colliders.Length; i < length; i++)
        {
            var collider = colliders[i];
            var food = collider.GetComponent<Food>();
            if (food == null)
            {
                continue;
            }
            var dis = Vector3.Distance(this.Head.transform.position, food.transform.position);
            if (dis < minDisFood)
            {
                minDisFood = dis;
                targetFood = food;
            }
        }
        if (target != null || targetFood != null)
        {
            if (target == null && targetFood != null)
            {
                SetTargetEnemy(targetFood);
            }
            else if (target != null && targetFood == null)
            {
                SetTargetEnemy(target);
            }
            else if (target != null && targetFood == null)
            {

                SetTargetEnemy(target);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(this.transform.position, VisualField);
    }
}
