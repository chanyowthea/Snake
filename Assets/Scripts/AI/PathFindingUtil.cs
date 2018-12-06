using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Components.AStar;
using Components.Struct;

public class PathFindingUtil : MonoBehaviour
{
    // recaculate matrix every frame. but only run once in one frame even if there are several players. 
    public byte[,] _Matrix { private set; get; }
    // caculate only once after enter the map. 
    private IPathFinder _PathFinder;
    public Vector2 CurMapSize
    {
        get
        {
            return MapManager.instance.CurMapSize;
        }
    }

    public const float _GridSize = 0.25f;
    public const byte _GRID_ROAD = 1;
    public const byte _GRID_BARRIER = 0;
    public int _XMaxBound;
    public int _YMaxBound;
    bool _HasResetDynamicBarriersInThisFrame;

    void Start()
    {
        // size needs to be power of 2. 
        _Matrix = new byte[256, 256];
        GenBaseMatrix();
        _PathFinder = new PathFinderFast(_Matrix);
        _PathFinder.Formula = HeuristicFormula.Manhattan; //使用我个人觉得最快的曼哈顿A*算法
        _PathFinder.SearchLimit = 2000; //即移动经过方块(20*20)不大于2000个(简单理解就是步数)
        // clear dynamic barriers. 
        _XMaxBound = _Matrix.GetUpperBound(0);
        _YMaxBound = _Matrix.GetUpperBound(1);

        // reset the map every one minute. 
        //GameManager.instance.DelayCall(1, ResetDynamicBarriers, true);
    }

    private void Update1()
    {
        _HasResetDynamicBarriersInThisFrame = false;
    }

    public void GenBaseMatrix()
    {
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

    float _MaxMoveDelta = 0; // ConstValue._DefaultBaseMoveSpeed * GameManager.instance.TimeScale;
    float _HeadSize = ConstValue._BodyUnitSize;
    float _BodySize = ConstValue._BodyUnitSize;
    public List<Point2D> _PrevBarrierGrids = new List<Point2D>();
    public void ResetDynamicBarriers()
    {
        if (_HasResetDynamicBarriersInThisFrame)
        {
            return;
        }

        for (int i = 0, length = _PrevBarrierGrids.Count; i < length; i++)
        {
            var grid = _PrevBarrierGrids[i];
            _Matrix[grid.X, grid.Y] = _GRID_ROAD;
        }
        _PrevBarrierGrids.Clear();

        var characters = GameManager.instance.GetCharacters();
        for (int i = 0, length = characters.Count; i < length; i++)
        {
            var character = characters[i];
            for (int j = 0, max = character.BodyLength; j < max; j++)
            {
                var body = character.GetBody(j);
                if (!body.IsStrong)
                {
                    continue;
                }

                var pos = body.transform.position;
                // grid in this rectange cannot be reached. 
                Rect rect = new Rect(pos.x, pos.y, _BodySize + _HeadSize + _MaxMoveDelta * 2, _BodySize + _HeadSize + _MaxMoveDelta * 2);
                Point2D bottomLeft = ConvertPoint2D(new Vector3(rect.x - rect.width / 2f, rect.y - rect.height / 2f));
                Point2D topRight = ConvertPoint2D(new Vector3(rect.x + rect.width / 2f, rect.y + rect.height / 2f));
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

    public Point2D ConvertPoint2D(Vector3 pos)
    {
        pos.x += MapManager.instance.CurMapSize.x / 2f - _GridSize / 2f;
        pos.y += MapManager.instance.CurMapSize.y / 2f - _GridSize / 2f;
        return new Point2D(Mathf.RoundToInt(pos.x / _GridSize), Mathf.RoundToInt(pos.y / _GridSize));
    }

    public Vector3 ConvertVector3(PathFinderNode point)
    {
        return ConvertVector3FromGrid(point.X, point.Y);
    }

    public Vector3 ConvertVector3FromGrid(int x, int y)
    {
        Vector3 pos = new Vector3();
        pos.x = x * _GridSize - MapManager.instance.CurMapSize.x / 2f + _GridSize / 2f;
        pos.y = y * _GridSize - MapManager.instance.CurMapSize.y / 2f + _GridSize / 2f;
        return pos;
    }
}
