using System.Collections;
using System.Collections.Generic;
using TsiU;
using UnityEngine;

public class BotFactory : MonoBehaviour
{
    /// <summary>
    /// 行为树
    /// </summary>
    //private static TBTAction _BevTree;
    static public TBTAction GetBehaviourTree()
    {
        //if (_BevTree != null)
        //{
        //    return _BevTree;
        //}
        var _BevTree = new TBTActionPrioritizedSelector();
        _BevTree.AddChild(new NodeChase())
                .AddChild(new NodeWander());
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
        return dis < RunTimeData._MinMoveDelta;
    }
}

class NodeChase : TBTActionLeaf
{
    //float _SteerGapTime = 3;
    //float _CurSteerTime;
    protected override bool onEvaluate(TBTWorkingData wData)
    {
        BotWorkingData thisData = wData.As<BotWorkingData>();
        //if (thisData._Character.GetTargetEnemy() == null)
        //{
        //    var player = (thisData._Character as Enemy);
        //    if (player != null)
        //    {
        //        player.CheckEnemy();
        //    }
        //}
        //return thisData._Character.GetTargetEnemy() != null;
        return false;
    }

    protected override void onEnter(TBTWorkingData wData)
    {
        base.onEnter(wData);
        //Debugger.LogFormat("class={0}, method={1}", LogColor.Blue, false, LogUtil.GetCurClassName(), LogUtil.GetCurMethodName());
    }

    protected override void onExit(TBTWorkingData wData, int runningStatus)
    {
        //_CurSteerTime = 0;
        base.onExit(wData, runningStatus);
    }

    protected override int onExecute(TBTWorkingData wData)
    {
        BotWorkingData thisData = wData.As<BotWorkingData>();
        Vector3 targetPos = thisData._Character.GetTargetPos();
        Vector3 currentPos = thisData._Character.Head.transform.position;
        float distToTarget = Vector3.Distance(targetPos, currentPos);
        if (distToTarget < RunTimeData._MinMoveDelta)
        {
            //_CurSteerTime = 0;
            return TBTRunningStatus.FINISHED;
        }
        else if (distToTarget > thisData._Character.VisualField)
        {
            thisData._Character.SetTargetEnemy(null);
            return TBTRunningStatus.FINISHED;
        }
        else
        {
            //float movingStep = thisData._Character.MoveSpeed;
            //if (movingStep > distToTarget)
            //{
            //    _CurSteerTime = 0;
            //    movingStep = distToTarget;
            //    ret = TBTRunningStatus.FINISHED;
            //}

            //if (_CurSteerTime > 0)
            //{
            //    _CurSteerTime -= thisData._DeltaTime;
            //}
            //else
            {
                //_CurSteerTime = _SteerGapTime;
                Enemy bot = thisData._Character as Enemy;
                Debug.DrawLine(currentPos, targetPos, Color.green, 1);
                if (bot != null)
                {
                    bot.SteerToTargetPos(targetPos);
                }
            }
            //Debugger.LogErrorFormat("NodeChase ret={0},_CurSteerTime={1}, targetPos={2}", ret, _CurSteerTime, targetPos);
            return TBTRunningStatus.EXECUTING;
        }
    }
}

class NodeWander : TBTActionLeaf
{
    Vector3 _TargetPos;
    float _SteerGapTime = 3;
    float _CurSteerTime;

    protected override bool onEvaluate(TBTWorkingData wData)
    {
        BotWorkingData thisData = wData.As<BotWorkingData>();

        //thisData._Character.SetTargetEnemy(null);
        //return thisData._Character.GetTargetEnemy() == null;
        return true;
    }

    protected override void onEnter(TBTWorkingData wData)
    {
        BotWorkingData thisData = wData.As<BotWorkingData>();
        //Debugger.LogFormat("class={0}, method={1}", LogColor.Green, false, LogUtil.GetCurClassName(), LogUtil.GetCurMethodName());
        GenerateTargetPos(thisData._Character.Head.transform.position, wData);
    }

    protected override void onExit(TBTWorkingData wData, int runningStatus)
    {
        _CurSteerTime = 0;
        base.onExit(wData, runningStatus);
    }

    protected override int onExecute(TBTWorkingData wData)
    {
        BotWorkingData thisData = wData.As<BotWorkingData>();
        //Debugger.LogErrorFormat("dis={0}, delta={1}, Name={2}, _TargetPos={3}, NodeWander.HashCode={4}",
        //Vector3.Distance(_TargetPos, thisData._Character.Head.transform.position), ConstValue._MinMoveDelta, thisData._Character.Name, _TargetPos, GetHashCode());
        // redirect
        if (Vector3.Distance(thisData._Character.Head.transform.position, _TargetPos) < RunTimeData._MinMoveDelta)
        {
            _CurSteerTime = 0;
            GenerateTargetPos(thisData._Character.Head.transform.position, wData);
        }
        // move to target position. 
        else
        {
            //Debugger.LogGreen("curtime=" + _CurSteerTime);
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
                    bot.SteerToTargetPos(_TargetPos, () =>
                    {
                        _CurSteerTime = 0;
                        GenerateTargetPos(thisData._Character.Head.transform.position, wData);
                    });
                }
            }
        }

        var player = (thisData._Character as Enemy);
        if (player != null)
        {
            //player.CheckEnemy();
        }
        return TBTRunningStatus.EXECUTING;
    }

    void GenerateTargetPos(Vector3 curPos, TBTWorkingData wData)
    {
        BotWorkingData thisData = wData.As<BotWorkingData>();
        _TargetPos = MapManager.instance.GetRandPosInRect(
            MathUtil.GetNextRandomRect(curPos, thisData._Character.VisualField, thisData._Character.Head.Radius * 2));
        //_TargetPos = MapManager.instance.GetRandPosInCurMap(ESpawnType.Character);
#if UNITY_EDITOR
        Debug.DrawLine(curPos, _TargetPos, Color.red, 1);
        //Debug.LogError("name==" + thisData._Character.Name + ", target pos=" + _TargetPos + ", NodeWander.HashCode=" + GetHashCode());
        // for test
        //var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //go.transform.position = _TargetPos;
#endif
    }
}