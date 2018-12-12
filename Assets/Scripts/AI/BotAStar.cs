using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Assertions;

public struct Point2D
{
    public int X,
               Y;
    public Point2D(int x, int y)
    {
        X = x;
        Y = y;
    }
}

public class BotAStar : MonoBehaviour
{
    private PathNode[,] _Matrix;
    BaseCharacter _Character;
    [SerializeField] Vector3 _TargetPos;
    [SerializeField] Vector3 _FinalTargetPos;
    List<Vector3> _PathList;
    public bool _IsInSteer { private set; get; }
    private SimpleAStar _PathFinder = new SimpleAStar();
    Action _OnSteerFailed;
    Action _OnSteerFinished;
    float _GridSize = 0.25f;

    public void SetData(BaseCharacter character)
    {
        _Character = character;

        _Matrix = new PathNode[(int)(MapManager.instance.CurMapSize.x / _GridSize), (int)(MapManager.instance.CurMapSize.y / _GridSize)];
        for (int x = 0, length = _Matrix.GetUpperBound(0) + 1; x < length; x++)
        {
            for (int y = 0, max = _Matrix.GetUpperBound(1) + 1; y < max; y++)
            {
                _Matrix[x, y] = new PathNode { _X = x, _Y = y, _CanWalk = true };
                var barrier = Physics2D.OverlapBox(ConvertVector3FromGrid(x, y), Vector2.one * _GridSize, 0,
                    LayerMask.GetMask("Barrier"));
                if (barrier != null)
                {
                    _Matrix[x, y]._CanWalk = false;
                }
            }
        }
        _PathFinder.Init(_Matrix);
    }

    public void ClearData()
    {
        _Matrix = null;
        _PathFinder = null;
        _Character = null;
        _TargetPos = Vector3.zero;
        _FinalTargetPos = Vector3.zero;
        if (_PathList != null)
        {
            _PathList.Clear();
            _PathList = null;
        }
        _LastStepPoses.Clear();
        _IsInSteer = false;
    }

#if UNITY_EDITOR
    List<Rect> _Grids = new List<Rect>();
    //for test
    public void GenGrid()
    {
        _Grids.Clear();
        for (int y = 0; y < _Matrix.GetUpperBound(1); y++)
        {
            for (int x = 0; x < _Matrix.GetUpperBound(0); x++)
            {
                if (!_Matrix[x, y]._CanWalk)
                {
                    var pos = ConvertVector3(new PathNode { _X = x, _Y = y });
                    _Grids.Add(new Rect(pos.x - _GridSize / 2f, pos.y - _GridSize / 2f, _GridSize * 0.8f, _GridSize * 0.8f));
                }
            }
        }
    }

    List<Rect> _PathGrids = new List<Rect>();
    //for test
    public void GenPathGrid()
    {
        _PathGrids.Clear();
        if (_PathList != null)
        {
            for (int i = 0, length = _PathList.Count; i < length; i++)
            {
                var pos = _PathList[i];
                _PathGrids.Add(new Rect(pos.x - _GridSize / 2f, pos.y - _GridSize / 2f, _GridSize * 0.8f, _GridSize * 0.8f));
            }
        }
    }

    public void OnDrawGizmos()
    {
        GenPathGrid();
        _PathGrids.ForEach((r) =>
        {
            Grid.DrawRect(r, Color.cyan);
        });

        if (_Character.CharacterID != 2)
        {
            //return;
        }

        GenGrid();
        _Grids.ForEach((r) =>
        {
            Grid.DrawRect(r, Color.grey);
        });
    }
#endif

    public void SteerToTargetPos(Vector3 pos, Action onFinish, Action onFailed = null)
    {
        //var enumerator = _SteerQueue.GetEnumerator();
        //while (enumerator.MoveNext())
        //{
        //    var item = enumerator.Current;
        //    Assert.IsNotNull(item.Key);
        //    // if contains this util already, add another action. 
        //    if (item.Key == this)
        //    {
        //        _SteerQueue.Add(new KeyValuePair<BotAStar, Action>(this, () => OnStartSteer(pos, onFinish, onFailed)));
        //        return;
        //    }
        //}

        // if do not contains this util, run immediately. 
        OnStartSteer(pos, onFinish, onFailed);
    }

    void OnStartSteer(Vector3 pos, Action onFinish, Action onFailed = null)
    {
        _OnSteerFinished = onFinish;
        _OnSteerFailed = onFailed;
        _FinalTargetPos = pos;
        _PathList = FindPath(_Character.Head.transform.position, pos);
        if (_PathList != null)
        {
            //_StartSteerTime = Singleton._DelayUtil.GameTime;
            _IsInSteer = true;
            _PathList.Reverse();
            if (_PathList.Count > 0)
            {
                _PathList.RemoveAt(0);
            }
            _PathList.Add(_FinalTargetPos);
            if (_PathList.Count > 0)
            {
                _TargetPos = _PathList[0];
            }
        }
        else
        {
            Debugger.LogError("_IsInSteer = false");
            SteerFailed();
        }
        //Debugger.LogError(LogUtil.GetCurMethodName() + "_TargetPos=" + _TargetPos + ", headPos=" + _Character.Head.transform.position);
    }

    public List<Vector3> FindPath(Vector3 start, Vector3 end)
    {
        UnityEngine.Profiling.Profiler.BeginSample("BotAStar.FindPath");
        //ResetDynamicBarriers();
        // set the left bottom corner of map to zero point. 
        List<PathNode> path = _PathFinder.FindPath(ConvertPoint2D(start), ConvertPoint2D(end)); //开始寻径
        var result = new List<Vector3>();
        if (path == null)
        {
            return result;
        }
        for (int i = 0, length = path.Count; i < length; i++)
        {
            result.Add(ConvertVector3(path[i]));
        }
        UnityEngine.Profiling.Profiler.EndSample();
        return result;
    }

    Queue<Vector3> _LastStepPoses = new Queue<Vector3>();
    const int _MaxRecordStepCount = 5;
    Vector3 _LastFrameHeadPos;
    public void Update()
    {
        if (!_IsInSteer || _PathList == null)
        {
            return;
        }
        _LastFrameHeadPos = _Character.Head.transform.position;
        if (_PathList.Count > 0)
        {
            if (Vector3.Distance(_TargetPos, _Character.Head.transform.position) < _Character.MoveMotion)
            {
                _PathList.RemoveAt(0);
                if (_PathList.Count > 0)
                {
                    _TargetPos = _PathList[0];
                }
            }
        }

        //Debug.LogErrorFormat("_TargetPos={0}, HeadPos={1}, MoveSpeed={2}",
        //_TargetPos, _Character.Head.transform.position, _Character.MoveMotion);
        if (Vector3.Distance(_TargetPos, _Character.Head.transform.position) >= _Character.MoveMotion)
        {
            var motion = (_TargetPos - _Character.Head.transform.position).normalized
                * _Character.MoveSpeed * Singleton._DelayUtil.Timer.DeltaTime;
            //Debugger.LogBlue("pathutil motion=" + motion);
            bool rs = _Character.Move(motion);
            if (!rs)
            {
                SteerFailed();
                return;
            }
        }
        // has reach the final destination?
        else
        {
            //Debugger.LogError("has reach the final destination!!!");
            if (_PathList.Count == 0)
            {
                SteerFinish();
                return;
            }
        }

        if (_LastStepPoses.Count == _MaxRecordStepCount)
        {
            int nearCount = 0;
            foreach (var item in _LastStepPoses)
            {
                if (Vector3.Distance(item, _Character.Head.transform.position) < _Character.MoveMotion / 2f)
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
                SteerFailed();
                return;
            }
        }
        Assert.IsTrue(_LastStepPoses.Count <= _MaxRecordStepCount);
        if (_LastStepPoses.Count == _MaxRecordStepCount)
        {
            _LastStepPoses.Dequeue();
        }
        _LastStepPoses.Enqueue(_Character.Head.transform.position);
    }

    void SteerFailed()
    {
        SteerFinish();
        if (_OnSteerFailed != null)
        {
            _OnSteerFailed();
        }
    }

    void SteerFinish()
    {
        var enumerator = _SteerQueue.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var item = enumerator.Current;
            Assert.IsNotNull(item.Key);
            // if contains this util already, add another action. 
            if (item.Key == this)
            {
                _SteerQueue.Remove(item);
            }
        }

        _LastStepPoses.Clear();
        _IsInSteer = false;
        if (_OnSteerFinished != null)
        {
            _OnSteerFinished();
        }
    }

    public Point2D ConvertPoint2D(Vector3 pos)
    {
        pos.x += MapManager.instance.CurMapSize.x / 2f - _GridSize / 2f;
        pos.y += MapManager.instance.CurMapSize.y / 2f - _GridSize / 2f;
        return new Point2D(Mathf.RoundToInt(pos.x / _GridSize), Mathf.RoundToInt(pos.y / _GridSize));
    }

    public Vector3 ConvertVector3(PathNode point)
    {
        return ConvertVector3FromGrid(point._X, point._Y);
    }

    public Vector3 ConvertVector3FromGrid(int x, int y)
    {
        Vector3 pos = new Vector3();
        pos.x = x * _GridSize - MapManager.instance.CurMapSize.x / 2f + _GridSize / 2f;
        pos.y = y * _GridSize - MapManager.instance.CurMapSize.y / 2f + _GridSize / 2f;
        return pos;
    }

    static List<KeyValuePair<BotAStar, Action>> _SteerQueue = new List<KeyValuePair<BotAStar, Action>>();
    public static void RunOneFrame()
    {
        if (_SteerQueue.Count > 0)
        {
            var item = _SteerQueue[0];
            Assert.IsNotNull(item.Key);
            Assert.IsNotNull(item.Value);
            _SteerQueue.RemoveAt(0);
            item.Value();
        }
    }
}
