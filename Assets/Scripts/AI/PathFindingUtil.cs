using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Components.AStar;
using Components.Struct;

public class PathFindingUtil
{
    private byte[,] _Matrix;
    private byte[,] _BaseMatrix;
    private float _GridSize = 0.25f;
    private IPathFinder _PathFinder;
    Vector2 CurMapSize
    {
        get
        {
            return MapManager.instance.CurMapSize;
        }
    }
    BaseCharacter _Character;
    Vector3 _TargetPos;
    Vector3 _FinalTargetPos;
    List<Vector3> _PathList;
    public bool _IsInSteer { private set; get; }
    static bool _IsGenBaseMatrix;
    const byte _GRID_ROAD = 1;
    const byte _GRID_BARRIER = 0;

    public void SetData(BaseCharacter character)
    {
        _Character = character;
        _GridSize = 0.25f;

        // size needs to be power of 2. 
        _Matrix = new byte[256, 256];
        GenBaseMatrix();
        ResetDynamicBarriers();
        _PathFinder = new PathFinderFast(_Matrix);
        _PathFinder.Formula = HeuristicFormula.Manhattan; //使用我个人觉得最快的曼哈顿A*算法
        _PathFinder.SearchLimit = 2000; //即移动经过方块(20*20)不大于2000个(简单理解就是步数)

        // reset the map every one minute. 
        //GameManager.instance.DelayCall(1, ResetDynamicBarriers, true);
    }

    public void ClearData()
    {
        if (_IsGenBaseMatrix)
        {
            _IsGenBaseMatrix = false;
        }
    }

#if UNITY_EDITOR
    List<Rect> _Grids = new List<Rect>();
    //for test
    public void DrawGrid()
    {
        _Grids.Clear();
        for (int y = 0; y < _Matrix.GetUpperBound(1); y++)
        {
            for (int x = 0; x < _Matrix.GetUpperBound(0); x++)
            {
                if (_Matrix[x, y] == _GRID_BARRIER)
                {
                    var pos = ConvertVector3(new PathFinderNode { X = x, Y = y });
                    _Grids.Add(new Rect(pos.x - _GridSize / 2f, pos.y - _GridSize / 2f, _GridSize * 0.8f, _GridSize * 0.8f));
                }
            }
        }
        var pos1 = ConvertVector3(new PathFinderNode { X = 0, Y = 0 });
        _Grids.Add(new Rect(pos1.x - _GridSize / 2f, pos1.y - _GridSize / 2f, _GridSize * 0.8f, _GridSize * 0.8f));
    }

    public void OnDrawGizmos()
    {
        _Grids.ForEach((r) =>
        {
            Grid.DrawRect(r, Color.green);
        });
    }
#endif

    public void SteerToTargetPos(Vector3 pos)
    {
        _FinalTargetPos = pos;
        _PathList = FindPath(_Character.Head.transform.position, pos);
        if (_PathList != null)
        {
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
        //Debugger.LogError(LogUtil.GetCurMethodName() + "_TargetPos=" + _TargetPos + ", headPos=" + _Character.Head.transform.position);
    }

    public void Update()
    {
        ResetDynamicBarriers();
        if (!_IsInSteer || _PathList == null)
        {
            return;
        }

        if (_PathList.Count > 0)
        {
            if (Vector3.Distance(_TargetPos, _Character.Head.transform.position) < ConstValue._MinMoveDelta)
            {
                _PathList.RemoveAt(0);
                if (_PathList.Count > 0)
                {
                    _TargetPos = _PathList[0];
                }
            }
        }

        if (Vector3.Distance(_TargetPos, _Character.Head.transform.position) >= ConstValue._MinMoveDelta)
        {
            var motion = (_TargetPos - _Character.Head.transform.position).normalized * _Character.MoveSpeed;
            bool rs = _Character.Move(motion);
        }
        // has reach the final destination?
        else
        {
            //Debugger.LogError("has reach the final destination!!!");
            if (_PathList.Count == 0)
            {
                _IsInSteer = false;
            }
        }
    }

    public void GenBaseMatrix()
    {
        if (_IsGenBaseMatrix)
        {
            return;
        }
        _IsGenBaseMatrix = true;

        for (int y = 0; y < _Matrix.GetUpperBound(1); y++)
        {
            for (int x = 0; x < _Matrix.GetUpperBound(0); x++)
            {
                // the point of out map is forbidden. 
                if (x >= (int)(CurMapSize.x / _GridSize) || y >= (int)(CurMapSize.y / _GridSize))
                {
                    _Matrix[x, y] = _GRID_BARRIER;
                }
                else
                {
                    var barrier = Physics2D.OverlapBox(ConvertVector3FromGrid(x, y), Vector2.one * _GridSize, 0,
                        LayerMask.GetMask("Barrier"));
                    if (barrier != null)
                    {
                        _Matrix[x, y] = _GRID_BARRIER;
                    }
                    else
                    {
                        // set default value. indicates a pass road
                        _Matrix[x, y] = _GRID_ROAD;
                    }
                }
            }
        }
    }

    List<Point2D> _PrevBarrierGrids = new List<Point2D>();
    public void ResetDynamicBarriers()
    {
        // clear dynamic barriers. 
        int xMaxBound = _Matrix.GetUpperBound(0);
        int yMaxBound = _Matrix.GetUpperBound(1);
        for (int i = 0, length = _PrevBarrierGrids.Count; i < length; i++)
        {
            var grid = _PrevBarrierGrids[i];
            _Matrix[grid.X, grid.Y] = _GRID_ROAD;
        }

        var characters = GameManager.instance.GetCharacters();
        float headSize = _Character.Head.Radius * 2;
        float bodySize = ConstValue._BodyUnitSize;
        float maxMoveDelta = ConstValue._DefaultBaseMoveSpeed * GameManager.instance.TimeScale;
        for (int i = 0, length = characters.Count; i < length; i++)
        {
            var character = characters[i];
            if (character == this._Character)
            {
                continue;
            }
            if (_Character.BodyLength < 1)
            {
                Debugger.LogError("_Character.BodyLength < 1");
            }
            else
            {
                bodySize = _Character.GetBody(0).Radius * 2;
            }

            for (int j = 0, max = character.BodyLength; j < max; j++)
            {
                var body = character.GetBody(j);
                if (!body.IsStrong)
                {
                    continue;
                }

                var pos = body.transform.position;
                // grid in this rectange cannot be reached. 
                Rect rect = new Rect(pos.x, pos.y, bodySize + headSize + maxMoveDelta * 2, bodySize + headSize + maxMoveDelta * 2);
                Point2D bottomLeft = ConvertPoint2D(new Vector3(rect.x - rect.width / 2f, rect.y - rect.height / 2f));
                Point2D topRight = ConvertPoint2D(new Vector3(rect.x + rect.width / 2f, rect.y + rect.height / 2f));
                int xMin = Mathf.Clamp(bottomLeft.X, 0, xMaxBound);
                int xMax = Mathf.Clamp(topRight.X, 0, xMaxBound);
                int yMin = Mathf.Clamp(bottomLeft.Y, 0, yMaxBound);
                int yMax = Mathf.Clamp(topRight.Y, 0, yMaxBound);
                //Debug.LogErrorFormat("xMin={0}, xMax={1}, yMin={2}, yMax={3}, origin={4}",
                //xMin, xMax, yMin, yMax, pos);
                for (int y = yMin; y <= yMax; y++)
                {
                    for (int x = xMin; x <= xMax; x++)
                    {
                        if (_Matrix[x, y] == _GRID_ROAD)
                        {
                            _Matrix[x, y] = _GRID_BARRIER;
                            _PrevBarrierGrids.Add(new Point2D(x, y));
                        }
                    }
                }
            }
        }
    }

    public List<Vector3> FindPath(Vector3 start, Vector3 end)
    {
        // set the left bottom corner of map to zero point. 
        List<PathFinderNode> path = _PathFinder.FindPath(ConvertPoint2D(start), ConvertPoint2D(end)); //开始寻径
        var result = new List<Vector3>();
        if (path == null)
        {
            return result;
        }
        for (int i = 0, length = path.Count; i < length; i++)
        {
            result.Add(ConvertVector3(path[i]));
        }
        return result;
    }

    Point2D ConvertPoint2D(Vector3 pos)
    {
        pos.x += MapManager.instance.CurMapSize.x / 2f - _GridSize / 2f;
        pos.y += MapManager.instance.CurMapSize.y / 2f - _GridSize / 2f;
        return new Point2D(Mathf.RoundToInt(pos.x / _GridSize), Mathf.RoundToInt(pos.y / _GridSize));
    }

    Vector3 ConvertVector3(PathFinderNode point)
    {
        return ConvertVector3FromGrid(point.X, point.Y);
    }

    Vector3 ConvertVector3FromGrid(int x, int y)
    {
        Vector3 pos = new Vector3();
        pos.x = x * _GridSize - MapManager.instance.CurMapSize.x / 2f + _GridSize / 2f;
        pos.y = y * _GridSize - MapManager.instance.CurMapSize.y / 2f + _GridSize / 2f;
        return pos;
    }
}
