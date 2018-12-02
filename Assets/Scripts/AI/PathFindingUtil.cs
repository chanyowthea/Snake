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
    List<Vector3> _PathList;
    bool _IsInSteer;
    static bool _IsGenBaseMatrix; 

    public void SetData(BaseCharacter character)
    {
        _Character = character;
        _GridSize = 0.25f;

        // size needs to be power of 2. 
        _Matrix = new byte[256, 256];
        ResetMap();
        _PathFinder = new PathFinderFast(_Matrix);
        _PathFinder.Formula = HeuristicFormula.Manhattan; //使用我个人觉得最快的曼哈顿A*算法
        _PathFinder.SearchLimit = 2000; //即移动经过方块(20*20)不大于2000个(简单理解就是步数)

        // reset the map every one minute. 
        GameManager.instance.DelayCall(1, ResetMap, true);
    }

    // for test
    //public void DrawGrid()
    //{
    //    _Grids.Clear();
    //    for (int y = 0; y < _Matrix.GetUpperBound(1); y++)
    //    {
    //        for (int x = 0; x < _Matrix.GetUpperBound(0); x++)
    //        {
    //            if (_Matrix[x, y] == 0)
    //            {
    //                var pos = ConvertVector3(new PathFinderNode { X = x, Y = y });
    //                _Grids.Add(new Rect(pos.x - _GridSize / 2f, pos.y - _GridSize / 2f, _GridSize * 0.8f, _GridSize * 0.8f));
    //            }
    //        }
    //    }
    //    var pos1 = ConvertVector3(new PathFinderNode { X = 0, Y = 0 });
    //    _Grids.Add(new Rect(pos1.x - _GridSize / 2f, pos1.y - _GridSize / 2f, _GridSize * 0.8f, _GridSize * 0.8f));
    //}

    //List<Rect> _Grids = new List<Rect>();
    //public void OnDrawGizmos()
    //{
    //    _Grids.ForEach((r) =>
    //    {
    //        Grid.DrawRect(r, Color.green);
    //    });
    //}

    public void SteerToTargetPos(Vector3 pos)
    {
        //var p = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        _PathList = FindPath(_Character.Head.transform.position, pos);
        _IsInSteer = true;
        _TargetPos = Vector3.zero;
        _PathList.Reverse();
        if (_PathList.Count > 0)
        {
            _TargetPos = _PathList[0];
        }
    }

    public void Update()
    {
        if (!_IsInSteer || _PathList == null)
        {
            return;
        }

        if (_PathList.Count > 0)
        {
            if (Vector3.Distance(_TargetPos, _Character.Head.transform.position) < 0.1f)
            {
                _PathList.RemoveAt(0);
                if (_PathList.Count > 0)
                {
                    _TargetPos = _PathList[0];
                }
            }
        }

        if (Vector3.Distance(_TargetPos, _Character.Head.transform.position) >= 0.1f)
        {
            var motion = (_TargetPos - _Character.Head.transform.position).normalized * _Character.MoveSpeed;
            _Character.Move(motion);
        }
        // has reach the final destination?
        else
        {
            if (_PathList.Count == 0)
            {
                _IsInSteer = false;
            }
        }
    }

    public void GenBaseMatrix()
    {

    }

    public void ResetMap()
    {
        for (int y = 0; y < _Matrix.GetUpperBound(1); y++)
        {
            for (int x = 0; x < _Matrix.GetUpperBound(0); x++)
            {
                // the point of out map is forbidden. 
                if (x >= (int)(CurMapSize.x / _GridSize) || y >= (int)(CurMapSize.y / _GridSize))
                {
                    _Matrix[x, y] = 0;
                }
                else
                {
                    // set default value. indicates a pass road
                    _Matrix[x, y] = 1;

                    var barrier = Physics2D.OverlapBox(ConvertVector3FromGrid(x, y), Vector2.one * _GridSize, 0,
                        LayerMask.GetMask("Barrier"));
                    if (barrier != null)
                    {
                        _Matrix[x, y] = 0;
                    }
                    else
                    {
                        // TODO 待优化，可以写成逐个Body检测
                        // 并先生成一个BaseMatrix
                        var cs = Physics2D.OverlapBoxAll(ConvertVector3FromGrid(x, y),
                            Vector2.one * _Character.Head.Radius * 2, 0);
                        if (cs != null && cs.Length > 0)
                        {
                            for (int i = 0, length = cs.Length; i < length; i++)
                            {
                                var collider = cs[i];
                                var body = collider.GetComponent<Body>();
                                if (body != null && body._Character != null && body._Character != this._Character)
                                {
                                    if (body is Head && body._Character.TotalLength >= _Character.TotalLength)
                                    {
                                        _Matrix[x, y] = 0;
                                    }
                                    else if (body.IsStrong)
                                    {
                                        _Matrix[x, y] = 0;
                                    }
                                }
                            }
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
