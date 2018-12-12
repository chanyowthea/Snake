using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarTest : MonoBehaviour
{
    SimpleAStar _PathFinder = new SimpleAStar();
    List<Rect> _Path = new List<Rect>();
    PathNode _StartNode;
    PathNode _EndNode;
    PathNode[,] _Matrix;
    float _GridSize = 0.2f;
    List<Rect> _Map = new List<Rect>();
    List<Rect> _Barriers = new List<Rect>();

    Queue<Action> _PathFindingQueue = new Queue<Action>();
    int _MaxFindSimultaneous = 1;
    private void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        float h1 = Input.GetAxisRaw("Horizontal1");
        float v1 = Input.GetAxisRaw("Vertical1");
        if ((h != 0 || v != 0 || h1 != 0 || v1 != 0) //&& Input.anyKeyDown
            )
        {
            if (_StartNode._X + Mathf.RoundToInt(h) <= _Matrix.GetUpperBound(0) && _StartNode._X + Mathf.RoundToInt(h) >= 0)
            {
                _StartNode._X += Mathf.RoundToInt(h);
            }
            if (_StartNode._Y + Mathf.RoundToInt(v) <= _Matrix.GetUpperBound(1) && _StartNode._Y + Mathf.RoundToInt(v) >= 0)
            {
                _StartNode._Y += Mathf.RoundToInt(v);
            }
            if (_EndNode._X + Mathf.RoundToInt(h1) <= _Matrix.GetUpperBound(0) && _EndNode._X + Mathf.RoundToInt(h1) >= 0)
            {
                _EndNode._X += Mathf.RoundToInt(h1);
            }
            if (_EndNode._Y + Mathf.RoundToInt(v1) <= _Matrix.GetUpperBound(1) && _EndNode._Y + Mathf.RoundToInt(v1) >= 0)
            {
                _EndNode._Y += Mathf.RoundToInt(v1);
            }

            PathNode start = new PathNode(_StartNode._X, _StartNode._Y);
            PathNode end = new PathNode(_EndNode._Y, _EndNode._Y);
            for (int i = 0; i < 10; i++)
            {
                _PathFindingQueue.Enqueue(() =>
                {
                    Point2D temp0 = new Point2D(start._X, start._Y);
                    Point2D temp1 = new Point2D(end._X, end._Y);
                    var nodes = FindPath(temp0, temp1);
                    ReDrawGrids(nodes);
                });
            }
        }

        if (_PathFindingQueue.Count > 0)
        {
            for (int i = 0; i < _MaxFindSimultaneous; i++)
            {
                if (_PathFindingQueue.Count == 0)
                {
                    break;
                }
                var a = _PathFindingQueue.Dequeue();
                if (a != null)
                {
                    a();
                }
            }
        }
    }

    void ReDrawGrids(List<PathNode> nodes)
    {
        // for test. draw grids. 
        _Barriers.Clear();
        _Map.Clear();
        _Path.Clear();
        for (int i = 0, length = _Matrix.GetUpperBound(0) + 1; i < length; i++)
        {
            for (int j = 0, max = _Matrix.GetUpperBound(1) + 1; j < max; j++)
            {
                if (!_Matrix[i, j]._CanWalk)
                {
                    AddGrid(_Barriers, i, j);
                }
                AddGrid(_Map, i, j);
            }
        }
        //Debug.Log("nodes.Count=" + nodes.Count);
        for (int i = 0, length = nodes.Count; i < length; i++)
        {
            var n = nodes[i];
            AddGrid(_Path, n._X, n._Y);
            //Debug.LogFormat("node {0}={1}", i, nodes[i]);
        }
    }

    List<PathNode> FindPath(Point2D start, Point2D end)
    {
        return _PathFinder.FindPath(start, end);
    }

    void Start()
    {
        _Matrix = new PathNode[32, 32];
        for (int i = 0, length = _Matrix.GetUpperBound(0) + 1; i < length; i++)
        {
            for (int j = 0, max = _Matrix.GetUpperBound(1) + 1; j < max; j++)
            {
                if ((i == 2 && j >= 5 && j <= 16) || (i == 4 && j >= 5 && j <= 31) || (j == 2 && i >= 16 && i <= 20)
                    || (j == 14 && i >= 9 && i <= 31))
                {
                    _Matrix[i, j] = new PathNode { _X = i, _Y = j, _CanWalk = false };
                }
                else
                {
                    _Matrix[i, j] = new PathNode { _X = i, _Y = j, _CanWalk = true };
                }
            }
        }
        _PathFinder.Init(_Matrix);
        _StartNode = new PathNode(0, 5);
        _EndNode = new PathNode(10, 31);
        var nodes = _PathFinder.FindPath(ToPoint2D(_StartNode), ToPoint2D(_EndNode));

        // for test. draw grids. 
        for (int i = 0, length = _Matrix.GetUpperBound(0) + 1; i < length; i++)
        {
            for (int j = 0, max = _Matrix.GetUpperBound(1) + 1; j < max; j++)
            {
                if (!_Matrix[i, j]._CanWalk)
                {
                    AddGrid(_Barriers, i, j);
                }
                AddGrid(_Map, i, j);
            }
        }
        //Debug.Log("nodes.Count=" + nodes.Count);
        for (int i = 0, length = nodes.Count; i < length; i++)
        {
            var n = nodes[i];
            AddGrid(_Path, n._X, n._Y);
            //Debug.LogFormat("node {0}={1}", i, nodes[i]);
        }
    }

    void AddGrid(List<Rect> grids, int x, int y)
    {
        Rect rect = new Rect();
        rect.width = _GridSize;
        rect.height = _GridSize;
        rect.x = x * _GridSize;
        rect.y = y * _GridSize;
        grids.Add(rect);
    }

    void OnDrawGizmos()
    {
        _Map.ForEach((r) =>
        {
            Grid.DrawRect(r, Color.grey);
        });
        _Path.ForEach((r) =>
        {
            Grid.DrawRect(r, Color.blue);
        });
        _Barriers.ForEach((r) =>
        {
            Grid.DrawRect(r, Color.yellow);
        });

        if (_StartNode == null || _EndNode == null)
        {
            return;
        }
        Rect rect = new Rect();
        rect.width = _GridSize * 0.8f;
        rect.height = _GridSize * 0.8f;
        rect.x = _GridSize * (1 - 0.8f) / 2f + _StartNode._X * _GridSize;
        rect.y = _GridSize * (1 - 0.8f) / 2f + _StartNode._Y * _GridSize;
        Grid.DrawRect(rect, Color.green);

        rect.x = _GridSize * (1 - 0.8f) / 2f + _EndNode._X * _GridSize;
        rect.y = _GridSize * (1 - 0.8f) / 2f + _EndNode._Y * _GridSize;
        Grid.DrawRect(rect, Color.red);
    }

    Point2D ToPoint2D(PathNode node)
    {
        return new Point2D(node._X, node._Y);
    }
}
