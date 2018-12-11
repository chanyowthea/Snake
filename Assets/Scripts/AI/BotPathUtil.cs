using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Components.AStar;
using Components.Struct;
using System;
using UnityEngine.Assertions;

public class BotPathUtil : MonoBehaviour
{
    private byte[,] _Matrix;
    BaseCharacter _Character;
    [SerializeField] Vector3 _TargetPos;
    [SerializeField] Vector3 _FinalTargetPos;
    List<Vector3> _PathList;
    public bool _IsInSteer { private set; get; }
    private IPathFinder _PathFinder;
    Point2D _TempPoint2D;
    Action _OnSteerFailed;
    Action _OnSteerFinished;
    float _StartSteerTime;
    float _TimeOutTime = 1f;
    Vector3 _PrevTimeOutPos;

    public void SetData(BaseCharacter character)
    {
        _XMaxBound = Singleton._PathUtil._XMaxBound + 1;
        _YMaxBound = Singleton._PathUtil._YMaxBound + 1;
        _Matrix = new byte[_XMaxBound, _YMaxBound];
        _PathFinder = new PathFinderFast(_Matrix);
        _PathFinder.Formula = HeuristicFormula.Manhattan; //使用我个人觉得最快的曼哈顿A*算法
        _PathFinder.SearchLimit = 2000; //即移动经过方块(20*20)不大于2000个(简单理解就是步数) 
        _Character = character;
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
                if (_Matrix[x, y] == PathFindingUtil._GRID_BARRIER)
                {
                    var pos = Singleton._PathUtil.ConvertVector3(new PathFinderNode { X = x, Y = y });
                    _Grids.Add(new Rect(pos.x - PathFindingUtil._GridSize / 2f, pos.y - PathFindingUtil._GridSize / 2f, PathFindingUtil._GridSize * 0.8f, PathFindingUtil._GridSize * 0.8f));
                }
            }
        }
        var pos1 = Singleton._PathUtil.ConvertVector3(new PathFinderNode { X = 0, Y = 0 });
        _Grids.Add(new Rect(pos1.x - PathFindingUtil._GridSize / 2f, pos1.y - PathFindingUtil._GridSize / 2f, PathFindingUtil._GridSize * 0.8f, PathFindingUtil._GridSize * 0.8f));
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
                _PathGrids.Add(new Rect(pos.x - PathFindingUtil._GridSize / 2f, pos.y - PathFindingUtil._GridSize / 2f, PathFindingUtil._GridSize * 0.8f, PathFindingUtil._GridSize * 0.8f));
            }
        }
        //var pos1 = Singleton._PathUtil.ConvertVector3(new PathFinderNode { X = 0, Y = 0 });
        //_PathGrids.Add(new Rect(pos1.x - PathFindingUtil._GridSize / 2f, pos1.y - PathFindingUtil._GridSize / 2f, PathFindingUtil._GridSize * 0.8f, PathFindingUtil._GridSize * 0.8f));
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
        _OnSteerFinished = onFinish;
        _OnSteerFailed = onFailed;
        _FinalTargetPos = pos;
        _PathList = FindPath(_Character.Head.transform.position, pos);
        if (_PathList != null)
        {
            _StartSteerTime = Singleton._DelayUtil.GameTime;
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

    int _XMaxBound;
    int _YMaxBound;
    float _MaxMoveDelta = 0; // ConstValue._DefaultBaseMoveSpeed * GameManager.instance.TimeScale;
    float _HeadSize = ConstValue._BodyUnitSize;
    float _BodySize = ConstValue._BodyUnitSize;
    bool _HasResetDynamicBarriersInThisFrame;
    public List<Point2D> _PrevRoadGrids = new List<Point2D>();

    // elude enemy's attack. 
    int _MaxEludeRadius = 3; 
    void ResetDynamicBarriers()
    {
        if (_HasResetDynamicBarriersInThisFrame)
        {
            return;
        }

        Singleton._PathUtil.ResetDynamicBarriers();
        System.Array.Copy(Singleton._PathUtil._Matrix, _Matrix, _Matrix.Length);

        // set this character's bodies as roads. 
        for (int j = 0, max = _Character.BodyLength; j < max; j++)
        {
            var body = _Character.GetBody(j);
            if (!body.IsStrong)
            {
                continue;
            }

            var pos = body.transform.position;
            // grid in this rectange cannot be reached. 
            Rect rect = new Rect(pos.x, pos.y, _BodySize + _HeadSize + _MaxMoveDelta * 2, _BodySize + _HeadSize + _MaxMoveDelta * 2);
            Point2D bottomLeft = Singleton._PathUtil.ConvertPoint2D(new Vector3(rect.x - rect.width / 2f, rect.y - rect.height / 2f));
            Point2D topRight = Singleton._PathUtil.ConvertPoint2D(new Vector3(rect.x + rect.width / 2f, rect.y + rect.height / 2f));
            int xMin = Mathf.Clamp(bottomLeft.X, 0, _XMaxBound);
            int xMax = Mathf.Clamp(topRight.X, 0, _XMaxBound);
            int yMin = Mathf.Clamp(bottomLeft.Y, 0, _YMaxBound);
            int yMax = Mathf.Clamp(topRight.Y, 0, _YMaxBound);
            //Debug.LogErrorFormat("xMin={0}, xMax={1}, yMin={2}, yMax={3}, origin={4}",
            //xMin, xMax, yMin, yMax, pos);
            for (int y = yMin; y <= yMax; y++)
            {
                for (int x = xMin; x <= xMax; x++)
                {
                    if (_Matrix[x, y] == PathFindingUtil._GRID_BARRIER)
                    {
                        _TempPoint2D.X = x;
                        _TempPoint2D.Y = y;
                        if (Singleton._PathUtil._PrevBarrierGrids.Contains(_TempPoint2D))
                        {
                            //_Matrix[x, y] = PathFindingUtil._GRID_ROAD;
                        }
                    }
                }
            }
        }

        // set heads whose total length is longer than this character as barrier. 
        for (int i = 0, length = _PrevRoadGrids.Count; i < length; i++)
        {
            var grid = _PrevRoadGrids[i];
            _Matrix[grid.X, grid.Y] = PathFindingUtil._GRID_ROAD;
        }
        _PrevRoadGrids.Clear();

        var characters = GameManager.instance.GetCharacters(); 
        BaseCharacter tempCharacter = null; 
        int curEludeRadius = _MaxEludeRadius; 
        for (int j = 0, max = characters.Count; j < max; j++)
        {
            tempCharacter = characters[j];
            Assert.IsNotNull(tempCharacter);
            if (tempCharacter == this._Character
                || tempCharacter.TotalLength < this._Character.TotalLength
                )
            {
                continue;
            }
            var body = tempCharacter.Head;
            curEludeRadius = Mathf.Clamp((tempCharacter.TotalLength - this._Character.TotalLength) / 4, 0, _MaxEludeRadius);
            var pos = body.transform.position;
            // grid in this rectange cannot be reached. 
            Rect rect = new Rect(pos.x, pos.y, curEludeRadius * 2, curEludeRadius * 2);
            Point2D bottomLeft = Singleton._PathUtil.ConvertPoint2D(new Vector3(rect.x - rect.width / 2f, rect.y - rect.height / 2f));
            Point2D topRight = Singleton._PathUtil.ConvertPoint2D(new Vector3(rect.x + rect.width / 2f, rect.y + rect.height / 2f));
            int xMin = Mathf.Clamp(bottomLeft.X, 0, _XMaxBound);
            int xMax = Mathf.Clamp(topRight.X, 0, _XMaxBound);
            int yMin = Mathf.Clamp(bottomLeft.Y, 0, _YMaxBound);
            int yMax = Mathf.Clamp(topRight.Y, 0, _YMaxBound);
            //Debug.LogErrorFormat("xMin={0}, xMax={1}, yMin={2}, yMax={3}, origin={4}",
            //xMin, xMax, yMin, yMax, pos);
            for (int y = yMin; y <= yMax; y++)
            {
                for (int x = xMin; x <= xMax; x++)
                {
                    if (_Matrix[x, y] == PathFindingUtil._GRID_ROAD)
                    {
                        _TempPoint2D.X = x;
                        _TempPoint2D.Y = y;
                        if (!_PrevRoadGrids.Contains(_TempPoint2D))
                        {
                            _Matrix[x, y] = PathFindingUtil._GRID_BARRIER; 
                            _PrevRoadGrids.Add(_TempPoint2D);
                        }
                    }
                }
            }
        }
    }

    public List<Vector3> FindPath(Vector3 start, Vector3 end)
    {
        UnityEngine.Profiling.Profiler.BeginSample("BotPathUtil.FindPath");
        ResetDynamicBarriers();
        // set the left bottom corner of map to zero point. 
        List<PathFinderNode> path = _PathFinder.FindPath(Singleton._PathUtil.ConvertPoint2D(start), Singleton._PathUtil.ConvertPoint2D(end)); //开始寻径
        var result = new List<Vector3>();
        if (path == null)
        {
            return result;
        }
        for (int i = 0, length = path.Count; i < length; i++)
        {
            result.Add(Singleton._PathUtil.ConvertVector3(path[i]));
        }
        UnityEngine.Profiling.Profiler.EndSample();
        return result;
    }

    Queue<Vector3> _LastStepPoses = new Queue<Vector3>();
    const int _MaxRecordStepCount = 5;
    float _AccumulateTime;
    Vector3 _LastFrameHeadPos;
    public void Update()
    {
        _HasResetDynamicBarriersInThisFrame = false;
        //Singleton._PathUtil.ResetDynamicBarriers();
        if (!_IsInSteer || _PathList == null)
        {
            return;
        }
        _LastFrameHeadPos = _Character.Head.transform.position;
        //if (Singleton._DelayUtil.GameTime - _StartSteerTime >= _TimeOutTime)
        //{
        //    _PrevTimeOutPos = _Character.Head.transform.position;
        //}
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
            //&& Vector3.Distance(_LastStepPoses.Peek(), _Character.Head.transform.position)
            //< RunTimeData._MinMoveDelta * 2; 
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
        _LastStepPoses.Clear();
        _IsInSteer = false;
        if (_OnSteerFinished != null)
        {
            _OnSteerFinished();
        }
    }
}
