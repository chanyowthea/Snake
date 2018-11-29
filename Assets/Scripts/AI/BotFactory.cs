using System.Collections;
using System.Collections.Generic;
using TsiU;
using UnityEngine;

public class BotFactory : MonoBehaviour
{
    /// <summary>
    /// 行为树
    /// </summary>
    private static TBTAction _bevTreeDemo1;
    static public TBTAction GetBehaviorTreeDemo1()
    {
        if (_bevTreeDemo1 != null)
        {
            return _bevTreeDemo1;
        }
        _bevTreeDemo1 = new TBTActionPrioritizedSelector();
        _bevTreeDemo1
            .AddChild(new TBTActionSequence()
                .SetPrecondition(new TBTPreconditionNOT(new CON_HasReachedTarget()))
                .AddChild(new NOD_MoveTo())
                )
            ;
        return _bevTreeDemo1;
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
        Vector3 targetPos = thisData._Character.GetTargetEnemy().transform.position;
        Vector3 currentPos = thisData._Character.transform.position;
        var dis = Vector3.Distance(targetPos, currentPos);
        return dis < ConstValue._BodyUnitSize * 0.9f;
    }
}

class NOD_MoveTo : TBTActionLeaf
{
    protected override int onExecute(TBTWorkingData wData)
    {
        BotWorkingData thisData = wData.As<BotWorkingData>();
        Vector3 targetPos = thisData._Character.GetTargetEnemy().transform.position;
        Vector3 currentPos = thisData._Character.transform.position;
        float distToTarget = Vector3.Distance(targetPos, currentPos);
        if (distToTarget < ConstValue._BodyUnitSize * 0.9f)
        {
            return TBTRunningStatus.FINISHED;
        }
        else
        {
            // TODO
            int ret = TBTRunningStatus.EXECUTING;
            Vector3 toTarget = (targetPos - currentPos).normalized;
            float movingStep = thisData._Character.MoveSpeed;
            if (movingStep > distToTarget)
            {
                movingStep = distToTarget;
                ret = TBTRunningStatus.FINISHED;
            }
            thisData._Character.Move(toTarget); 
            return ret;
        }
    }
}

public static class LogUtil
{
    public static string GetCurMethodName()
    {
        return new System.Diagnostics.StackFrame(1).GetMethod().Name;
    }

    public static string GetCurClassName()
    {
        return new System.Diagnostics.StackFrame(1).GetMethod().DeclaringType.Name;
    }
}