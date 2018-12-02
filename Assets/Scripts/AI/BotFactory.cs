using System.Collections;
using System.Collections.Generic;
using TsiU;
using UnityEngine;

public class BotFactory : MonoBehaviour
{
    /// <summary>
    /// 行为树
    /// </summary>
    private static TBTAction _BevTree;
    static public TBTAction GetBehaviourTree()
    {
        if (_BevTree != null)
        {
            return _BevTree;
        }
        _BevTree = new TBTActionPrioritizedSelector();
        _BevTree.AddChild(new NodeChase())
                .AddChild(new NodeWander())
            ;
        return _BevTree;
    }
}

class BotWorkingData : TBTWorkingData
{
    public BaseCharacter _Character;
    public float _GameTime;
    public float _DeltaTime;
}

class CON_HasReachedTarget : TBTPreconditionLeaf
{
    public override bool IsTrue(TBTWorkingData wData)
    {
        BotWorkingData thisData = wData.As<BotWorkingData>();
        Vector3 targetPos = thisData._Character.GetTargetPos();
        Vector3 currentPos = thisData._Character.Head.transform.position;
        var dis = Vector3.Distance(targetPos, currentPos);
        return dis < ConstValue._BodyUnitSize * 0.02f;
    }
}

class NodeChase : TBTActionLeaf
{
    float _SteerGapTime = 3;
    float _CurSteerTime;
    protected override bool onEvaluate(TBTWorkingData wData)
    {
        BotWorkingData thisData = wData.As<BotWorkingData>();
        Debug.Log("GetTargetEnemy() != null=" + (thisData._Character.GetTargetEnemy() != null));
        return thisData._Character.GetTargetEnemy() != null;
    }

    protected override int onExecute(TBTWorkingData wData)
    {
        BotWorkingData thisData = wData.As<BotWorkingData>();
        var enemy = thisData._Character.GetTargetEnemy();
        if (enemy == null)
        {
            return TBTRunningStatus.FINISHED;
        }
        Vector3 targetPos = thisData._Character.GetTargetPos();
        Vector3 currentPos = thisData._Character.Head.transform.position;
        float distToTarget = Vector3.Distance(targetPos, currentPos);
        if (distToTarget < ConstValue._BodyUnitSize * 0.1f)
        {
            return TBTRunningStatus.FINISHED;
        }
        else
        {
            int ret = TBTRunningStatus.EXECUTING;
            float movingStep = thisData._Character.MoveSpeed;
            if (movingStep > distToTarget || thisData._Character.GetTargetEnemy() == null)
            {
                movingStep = distToTarget;
                ret = TBTRunningStatus.FINISHED;
            }
            if (_CurSteerTime > 0)
            {
                _CurSteerTime -= thisData._DeltaTime;
            }
            else
            {
                _CurSteerTime = _SteerGapTime;
                Enemy bot = thisData._Character as Enemy;
                if (bot != null)
                {
                    bot.SteerToTargetPos(targetPos);
                }
            }
            //Debugger.LogErrorFormat("NodeChase ret={0},_CurSteerTime={1}, targetPos={2}", ret, _CurSteerTime, targetPos);
            return ret;
        }
    }
}

class NodeWander : TBTActionLeaf
{
    Vector3 _TargetPos;
    Vector3 _MoveDir;
    float _SteerGapTime = 3;
    float _CurSteerTime;

    protected override bool onEvaluate(TBTWorkingData wData)
    {
        BotWorkingData thisData = wData.As<BotWorkingData>();
        return thisData._Character.GetTargetEnemy() == null;
    }

    protected override void onEnter(TBTWorkingData wData)
    {
        BotWorkingData thisData = wData.As<BotWorkingData>();
        if (thisData == null)
        {
            return;
        }
        GenerateTargetPos(thisData._Character.Head.transform.position, wData);
    }

    protected override int onExecute(TBTWorkingData wData)
    {
        BotWorkingData thisData = wData.As<BotWorkingData>();
        // redirect
        if (Vector3.Distance(thisData._Character.Head.transform.position, _TargetPos) < ConstValue._BodyUnitSize * 0.1f)
        {
            GenerateTargetPos(thisData._Character.Head.transform.position, wData);
        }
        // move to target position. 
        else
        {
            if (_CurSteerTime > 0)
            {
                _CurSteerTime -= thisData._DeltaTime;
            }
            else
            {
                Enemy bot = thisData._Character as Enemy;
                if (bot != null)
                {
                    bot.SteerToTargetPos(_TargetPos);
                }
            }

            //Vector3 origin = _MoveDir * thisData._Character.MoveSpeed;
            //;
            //Vector3[] dirs = new Vector3[]{
            //    origin,
            //    MathUtil.V3RotateAround(origin, -Vector3.forward, 90),
            //    MathUtil.V3RotateAround(origin, -Vector3.forward, 90 * 2),
            //    MathUtil.V3RotateAround(origin, -Vector3.forward, 90 * 3)};
            //var rs = false;
            //int index = 0;
            //Vector3 dir = Vector3.zero;
            //while (!rs && index < dirs.Length)
            //{
            //    dir = dirs[index];
            //    rs = thisData._Character.Move(_MoveDir * thisData._Character.MoveSpeed);
            //    ++index;
            //    //GenerateTargetPos(thisData._Character.Head.transform.position);
            //}
        }

        (thisData._Character as Enemy).CheckEnemy();
        if (thisData._Character.GetTargetEnemy() != null)
        {
            return TBTRunningStatus.FINISHED;
        }
        return TBTRunningStatus.EXECUTING;
    }

    void GenerateTargetPos(Vector3 curPos, TBTWorkingData wData)
    {
        BotWorkingData thisData = wData.As<BotWorkingData>();
        _TargetPos = MapManager.instance.GetRandPosInRect(
            MathUtil.GetNextRandomRect(_TargetPos, thisData._Character.VisualField, thisData._Character.Head.Radius));
        Debug.DrawLine(curPos, _TargetPos, Color.red, 1);
    }
}