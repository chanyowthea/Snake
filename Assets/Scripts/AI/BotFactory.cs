using System.Collections;
using System.Collections.Generic;
using TsiU;
using UnityEngine;
using UnityEngine.Assertions;

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
        return dis < thisData._Character.MoveMotion;
    }
}

class NodeChase : TBTActionLeaf
{
    float _SteerGapTime = 0.2f;
    float _CurSteerTime;
    protected override bool onEvaluate(TBTWorkingData wData)
    {
        UnityEngine.Profiling.Profiler.BeginSample("NodeChase.onEvaluate");
        BotWorkingData thisData = wData.As<BotWorkingData>();
        UnityEngine.Profiling.Profiler.EndSample();
        //return false;
        return thisData._Character.GetTargetEnemy() != null;
    }

    protected override void onEnter(TBTWorkingData wData)
    {
        UnityEngine.Profiling.Profiler.BeginSample("NodeChase.onEnter");
        base.onEnter(wData);
        //Debugger.LogFormat("class={0}, method={1}", LogColor.White, false, LogUtil.GetCurClassName(), LogUtil.GetCurMethodName());
        UnityEngine.Profiling.Profiler.EndSample();
    }

    protected override void onExit(TBTWorkingData wData, int runningStatus)
    {
        UnityEngine.Profiling.Profiler.BeginSample("NodeChase.onExit");
        _CurSteerTime = 0;
        base.onExit(wData, runningStatus);
        UnityEngine.Profiling.Profiler.EndSample();
    }

    protected override int onExecute(TBTWorkingData wData)
    {
        UnityEngine.Profiling.Profiler.BeginSample("NodeChase.onExecute in");
        BotWorkingData thisData = wData.As<BotWorkingData>();
        Vector3 targetPos = thisData._Character.GetTargetPos();
        Vector3 currentPos = thisData._Character.Head.transform.position;
        float distToTarget = Vector3.Distance(targetPos, currentPos);
        UnityEngine.Profiling.Profiler.EndSample();
        if (distToTarget < thisData._Character.MoveMotion)
        {
            _CurSteerTime = 0;
            thisData._Character.SetTargetEnemy(null);
            return TBTRunningStatus.FINISHED;
        }
        else if (distToTarget > thisData._Character.VisualField)
        {
            thisData._Character.SetTargetEnemy(null);
            return TBTRunningStatus.FINISHED;
        }
        else
        {
            UnityEngine.Profiling.Profiler.BeginSample("NodeChase.onExecute else");
            var player = thisData._Character.GetTargetEnemy().GetComponent<BaseCharacter>();
            if (player != null && player.TotalLength >= thisData._Character.TotalLength)
            {
                thisData._Character.SetTargetEnemy(null);
                return TBTRunningStatus.FINISHED;
            }
            if (_CurSteerTime > 0)
            {
                _CurSteerTime -= thisData._DeltaTime;
            }
            else
            {
                _CurSteerTime = _SteerGapTime;
                Enemy bot = thisData._Character as Enemy;
                Debug.DrawLine(currentPos, targetPos, Color.green, 1);
                if (bot != null)
                {
                    bot.SteerToTargetPos(targetPos, () =>
                    {
                        thisData._Character.SetTargetEnemy(null);
                    }, null);
                }
            }
            //Debugger.LogFormat("NodeChase targetPos={0}", targetPos);
            UnityEngine.Profiling.Profiler.EndSample();
            return TBTRunningStatus.EXECUTING;
        }
    }
}

class NodeWander : TBTActionLeaf
{
    Vector3 _TargetPos;
    bool _InWander;
    float _SteerGapTime = 0.5f;
    float _CurSteerTime;
    float _CheckGapTime = 1f;
    float _CurCheckTime;
    Queue<Vector3> _LastStepPoses = new Queue<Vector3>();
    const int _MaxRecordStepCount = 5;

    protected override bool onEvaluate(TBTWorkingData wData)
    {
        BotWorkingData thisData = wData.As<BotWorkingData>();
        //thisData._Character.SetTargetEnemy(null);
        //return true;
        return thisData._Character.GetTargetEnemy() == null;
    }

    protected override void onEnter(TBTWorkingData wData)
    {
        BotWorkingData thisData = wData.As<BotWorkingData>();
        //Debugger.LogFormat("class={0}, method={1}", LogColor.Green, false, LogUtil.GetCurClassName(), LogUtil.GetCurMethodName());
        //GenerateTargetPos(thisData._Character.Head.transform.position, wData);
        _InWander = false;
        _CurSteerTime = _SteerGapTime;
        _CurCheckTime = _CheckGapTime;
    }

    protected override void onExit(TBTWorkingData wData, int runningStatus)
    {
        _CurCheckTime = _CheckGapTime;
        _CurSteerTime = _SteerGapTime;
        _InWander = false;
        _LastStepPoses.Clear();
        base.onExit(wData, runningStatus);
    }

    protected override int onExecute(TBTWorkingData wData)
    {
        BotWorkingData thisData = wData.As<BotWorkingData>();
        //Debugger.LogErrorFormat("dis={0}, delta={1}, Name={2}, _TargetPos={3}, NodeWander.HashCode={4}",
        //Vector3.Distance(_TargetPos, thisData._Character.Head.transform.position), ConstValue._MinMoveDelta, thisData._Character.Name, _TargetPos, GetHashCode());
        //Debugger.LogGreen("curtime=" + _CurSteerTime);
        Vector3 targetPos = _TargetPos;
        Vector3 currentPos = thisData._Character.Head.transform.position;
        float distToTarget = Vector3.Distance(targetPos, currentPos);
        if (distToTarget < thisData._Character.MoveMotion)
        {
            if (_InWander)
            {
                //Debugger.LogFormat("reach dest class={0}, method={1}", LogColor.Blue, false, LogUtil.GetCurClassName(), LogUtil.GetCurMethodName());
                _CurSteerTime = _SteerGapTime;
                _InWander = false;
                _LastStepPoses.Clear();
            }
        }

        // check enemy operation has a cool down time. 
        if (_CurCheckTime > 0)
        {
            _CurCheckTime -= thisData._DeltaTime;
        }
        else
        {
            UnityEngine.Profiling.Profiler.BeginSample("NodeWander.onExecute CheckEnemy");
            _CurCheckTime = _CheckGapTime;
            var player = (thisData._Character as Enemy);
            if (player != null)
            {
                player.CheckEnemy();
            }
            UnityEngine.Profiling.Profiler.EndSample();
            //Debugger.LogFormat("check enemy class={0}, method={1}", LogColor.Brown, false, LogUtil.GetCurClassName(), LogUtil.GetCurMethodName());
        }

        // wait for next wander command
        if (_CurSteerTime > 0)
        {
            _CurSteerTime -= thisData._DeltaTime;
        }
        else
        {
            if (!_InWander)
            {
                //Debugger.LogFormat("generate pos class={0}, method={1}", LogColor.Cyan, false, LogUtil.GetCurClassName(), LogUtil.GetCurMethodName());
                GenerateTargetPos(thisData._Character.Head.transform.position, wData);
                _InWander = true;
            }
        }

        if (_InWander)
        {
            //Debugger.LogFormat("steer to class={0}, method={1}", LogColor.Green, false, LogUtil.GetCurClassName(), LogUtil.GetCurMethodName());
            UnityEngine.Profiling.Profiler.BeginSample("NodeWander.onExecute InWander");
            var motion = (_TargetPos - thisData._Character.Head.transform.position).normalized
                * thisData._Character.MoveSpeed * Singleton._DelayUtil.Timer.DeltaTime;
            bool rs = thisData._Character.Move(motion);
            if (!rs)
            {
                if (_InWander)
                {
                    _CurSteerTime = _SteerGapTime;
                    _InWander = false;
                    _LastStepPoses.Clear();
                }
            }

            if (_LastStepPoses.Count == _MaxRecordStepCount)
            {
                int nearCount = 0;
                foreach (var item in _LastStepPoses)
                {
                    if (Vector3.Distance(item, thisData._Character.Head.transform.position) < thisData._Character.MoveMotion / 2f)
                    {
                        nearCount += 1;
                    }
                }
                if (nearCount >= 2)
                {
                    //Debug.LogErrorFormat("nearCount name={0}, peek={1}, dist={2}, delta={3}",
                    //    _Character.Name, _LastStepPoses.Peek(),
                    //    Vector3.Distance(_LastStepPoses.Peek(), _Character.Head.transform.position),
                    //    _Character.MoveMotion);
                    if (_InWander)
                    {
                        _CurSteerTime = _SteerGapTime;
                        _InWander = false;
                        _LastStepPoses.Clear();
                    }
                }
            }
            Assert.IsTrue(_LastStepPoses.Count <= _MaxRecordStepCount);
            if (_LastStepPoses.Count == _MaxRecordStepCount)
            {
                _LastStepPoses.Dequeue();
            }
            _LastStepPoses.Enqueue(thisData._Character.Head.transform.position);

            UnityEngine.Profiling.Profiler.EndSample();
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
        //Debug.LogError("GenerateTargetPos Name=" + thisData._Character.Name + ", target pos=" + _TargetPos + ", NodeWander.HashCode=" + GetHashCode());
        //for test
        //var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //go.transform.position = _TargetPos;
#endif
    }
}