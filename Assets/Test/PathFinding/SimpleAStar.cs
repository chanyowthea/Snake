using Components.Struct;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    public float F
    {
        get
        {
            return _H + _G;
        }
    }
    public float _H;
    public float _G;
    public PathNode _Parent;
    public int _X;
    public int _Y;
    public bool _CanWalk = true;

    public PathNode()
    {

    }

    public PathNode(int x, int y)
    {
        _X = x;
        _Y = y;
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
        {
            return false;
        }
        var node = obj as PathNode;
        return node._X == this._X && node._Y == this._Y;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return string.Format("_G={0}, _H={1}, F={2}, _X={3}, _Y={4}, _CanWalk={5}", _G, _H, F, _X, _Y, _CanWalk);
    }
}

public class SimpleAStar
{
    PathNode[,] _Matrix;

    public void Init(Point2D mapSize)
    {
        _Matrix = new PathNode[mapSize.X, mapSize.Y];
        for (int i = 0, length = _Matrix.GetUpperBound(0) + 1; i < length; i++)
        {
            for (int j = 0, max = _Matrix.GetUpperBound(1) + 1; j < max; j++)
            {
                _Matrix[i, j] = new PathNode(i, j);
            }
        }
    }

    public List<PathNode> FindPath(PathNode start, PathNode end)
    {
        if (start == null || end == null)
        {
            return null;
        }
        for (int i = 0, length = _Matrix.GetUpperBound(0) + 1; i < length; i++)
        {
            for (int j = 0, max = _Matrix.GetUpperBound(1) + 1; j < max; j++)
            {
                var node = _Matrix[i, j];
                node._H = Mathf.Abs(node._X - end._X) + Mathf.Abs(node._Y - end._Y);
                if (node.Equals(start))
                {
                    start = node;
                }
                else if (node.Equals(end))
                {
                    end = node;
                }
            }
        }

        Debug.Log("start=" + start);
        Debug.Log("end=" + end);

        List<PathNode> openSet = new List<PathNode>();
        List<PathNode> closeSet = new List<PathNode>();
        PathNode curNode = start;
        openSet.Add(start);
        while (openSet.Count > 0)
        {
            for (int i = 0, length = openSet.Count; i < length; i++)
            {
                var node = openSet[i];
                if (node.F < curNode.F) // TODO
                {
                    curNode = node;
                }
            }
            Debug.Log("curNode=" + curNode);
            if (curNode.Equals(end)) // TODO Equals
            {
                break;
            }

            openSet.Remove(curNode);
            closeSet.Add(curNode);

            var neighs = GetNeighbours(curNode);
            Debug.Log("neighs=" + neighs.Count);
            for (int i = 0, length = neighs.Count; i < length; i++)
            {
                var node = neighs[i];
                if (!node._CanWalk)
                {
                    continue;
                }
                if (closeSet.Contains(node))
                {
                    continue;
                }
                var cost = curNode._G + GetGoneDistance(curNode, node);
                if (!openSet.Contains(node))
                {
                    openSet.Add(node);
                    node._Parent = curNode;
                    node._G = cost;
                }
                else
                {
                    if (node._G < cost)
                    {
                        node._Parent = curNode;
                        node._G = cost;
                    }
                }
            }
        }

        List<PathNode> pathNodes = new List<PathNode>();
        while (curNode._Parent != null)
        {
            pathNodes.Add(curNode);
            curNode = curNode._Parent;
        }
        pathNodes.Reverse();
        return pathNodes;
    }

    public float GetGoneDistance(PathNode start, PathNode end)
    {
        int xGap = Mathf.Abs(start._X - end._X);
        int yGap = Mathf.Abs(start._Y - end._Y);
        if (xGap > yGap)
        {
            return (xGap - yGap) + yGap * 1.4f;
        }
        else
        {
            return (yGap - xGap) + xGap * 1.4f;
        }
    }

    public List<PathNode> GetNeighbours(PathNode node)
    {
        List<PathNode> neighbours = new List<PathNode>();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }
                int x = i + node._X;
                int y = j + node._Y;
                //Debug.LogFormat("neighs x={0}, y={1}", x, y);
                if (x >= 0 && x <= _Matrix.GetUpperBound(0) && y >= 0 && y <= _Matrix.GetUpperBound(1))
                {
                    neighbours.Add(_Matrix[x, y]);
                }
            }
        }
        return neighbours;
    }
}
