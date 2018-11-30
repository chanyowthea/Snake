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
        _BevTree
                //.AddChild(new TBTActionSequence()
                //    .SetPrecondition(new TBTPreconditionNOT(new CON_HasReachedTarget()))
                //    .AddChild(new NOD_MoveTo())
                //    )
                .AddChild(new NOD_MoveTo())
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
        return dis < ConstValue._BodyUnitSize * 0.9f;
    }
}

class NOD_MoveTo : TBTActionLeaf
{
    protected override bool onEvaluate(TBTWorkingData wData)
    {
        BotWorkingData thisData = wData.As<BotWorkingData>();
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
        if (distToTarget < ConstValue._BodyUnitSize * 0.9f)
        {
            return TBTRunningStatus.FINISHED;
        }
        else
        {
            int ret = TBTRunningStatus.EXECUTING;
            Vector3 toTarget = (targetPos - currentPos).normalized * thisData._Character.MoveSpeed;
            float movingStep = thisData._Character.MoveSpeed;
            if (movingStep > distToTarget || thisData._Character.GetTargetEnemy() == null)
            {
                movingStep = distToTarget;
                ret = TBTRunningStatus.FINISHED;
            }
            thisData._Character.Move(toTarget);
            //Debugger.LogErrorFormat("NOD_MoveTo ret={0},toTarget={1}, targetPos={2}", ret, toTarget, targetPos); 
            return ret;
        }
    }
}

class NodeWander : TBTActionLeaf
{
    Vector3 _TargetPos;
    Vector3 _MoveDir;

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
        GenerateTargetPos(thisData._Character.Head.transform.position);
    }

    protected override int onExecute(TBTWorkingData wData)
    {
        BotWorkingData thisData = wData.As<BotWorkingData>();
        // redirect
        if (Vector3.Distance(thisData._Character.Head.transform.position, _TargetPos) < 0.1f)
        {
            GenerateTargetPos(thisData._Character.Head.transform.position);
        }
        // move to target position. 
        else
        {
            var rs = thisData._Character.Move(_MoveDir * thisData._Character.MoveSpeed);
            if (!rs)
            {
                GenerateTargetPos(thisData._Character.Head.transform.position);
            }
        }

        (thisData._Character as Enemy).CheckEnemy();
        if (thisData._Character.GetTargetEnemy() != null)
        {
            return TBTRunningStatus.FINISHED;
        }
        return TBTRunningStatus.EXECUTING;
    }

    void GenerateTargetPos(Vector3 curPos)
    {
        _TargetPos = MapManager.instance.GetRandPosInCurMap(ESpawnType.Character);
        _MoveDir = (_TargetPos - curPos).normalized;
    }
}