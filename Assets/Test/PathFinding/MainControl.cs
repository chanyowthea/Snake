using System;using UnityEngine;using System.Collections;using System.Collections.Generic;
using Components.AStar;using Components.Struct;

public class MainControl : MonoBehaviour
{
    private byte[,] Matrix = new byte[64, 64]; //寻路用二维矩阵
    private float GridSize = 0.2f; //单位格子大小
    List<Rect> lst = new List<Rect>();
    List<Rect> lst2 = new List<Rect>();
    List<Rect> lst3 = new List<Rect>();
    List<Rect> lst4 = new List<Rect>();
    private IPathFinder PathFinder = null;

    private Point2D Start; //移动起点坐标
    private Point2D End;  //移动终点坐标
                          // Use this for initialization
    void Awake()
    {
        ResetMatrix(); //初始化二维矩阵
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            LeftMouseClick(0);
        }
    }


    List<LineRenderer> _PathLines = new List<LineRenderer>();
    List<LineRenderer> _PathLines4 = new List<LineRenderer>();
    private void LeftMouseClick(long l)
    {
        lst2.Clear();
        Rect rect = new Rect();
        rect.width = GridSize;
        rect.height = GridSize;
        rect.x = GridSize;
        rect.y = GridSize;
        lst2.Add(rect);

        Start = new Point2D(15, 1);

        //获得屏幕坐标
        var p = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        int x = Mathf.RoundToInt(p.x / GridSize);
        int y = Mathf.RoundToInt(p.y / GridSize);

        //Vector3 p = Input.mousePosition;
        //int x = ((int)p.x / (int)(GridSize * 100));
        //int y = ((int)p.y / (int)(GridSize * 100));
        End = new Point2D(x, y);  //计算终点坐标
        Debug.Log("mouse:" + p + " x/GridSize:" + x + " y/GridSize:" + y);

        lst4.Clear();
        lst4.Add(IndexConvertToRect(x, y));

        PathFinder = new PathFinderFast(Matrix);
        PathFinder.Formula = HeuristicFormula.Manhattan; //使用我个人觉得最快的曼哈顿A*算法
        PathFinder.SearchLimit = 2000; //即移动经过方块(20*20)不大于2000个(简单理解就是步数)

        List<PathFinderNode> path = PathFinder.FindPath(Start, End); //开始寻径

        if (path == null)
        {
            Debug.Log("路径不存在！");
        }
        else
        {
            string output = string.Empty;
            for (int i = path.Count - 1; i >= 0; i--)
            {
                output = string.Format(output
                    + "{0}"
                    + path[i].X.ToString()
                    + "{1}"
                    + path[i].Y.ToString()
                    + "{2}",
                    "(", ",", ") ");


                lst2.Add(IndexConvertToRect(path[i].X, path[i].Y));
            }
            Debug.Log("路径坐标分别为:" + output);


        }

        for (int i = 0, length = _PathLines.Count; i < length; i++)
        {
            var p1 = _PathLines[i];
            GameObject.Destroy(p1.gameObject); 
        }
        _PathLines.Clear();
        // draw path. 
        for (int i = 0, length = lst2.Count; i < length; i++)
        {
            var r = lst2[i];
            var line = _Line.GetLineRenderer();
            _PathLines.Add(line);
            line.positionCount = 5;
            line.material = null; 
            Vector3 leftBottom1 = new Vector3(r.x, r.y);
            Vector3 leftTop1 = new Vector3(r.x, r.y + r.height);
            Vector3 rightBottom1 = new Vector3(r.x + r.width, r.y);
            Vector3 rightTop1 = new Vector3(r.x + r.width, r.y + r.height);
            line.SetPosition(0, leftBottom1);
            line.SetPosition(1, leftTop1);
            line.SetPosition(2, rightTop1);
            line.SetPosition(3, rightBottom1);
            line.SetPosition(4, leftBottom1);
        }

        for (int i = 0, length = _PathLines4.Count; i < length; i++)
        {
            var p1 = _PathLines4[i];
            GameObject.Destroy(p1.gameObject);
        }
        _PathLines4.Clear();
        // draw dest. 
        for (int i = 0, length = lst4.Count; i < length; i++)
        {
            var r = lst4[i];
            var line = _Line.GetLineRenderer();
            _PathLines4.Add(line);
            line.positionCount = 5;
            Vector3 leftBottom1 = new Vector3(r.x, r.y);
            Vector3 leftTop1 = new Vector3(r.x, r.y + r.height);
            Vector3 rightBottom1 = new Vector3(r.x + r.width, r.y);
            Vector3 rightTop1 = new Vector3(r.x + r.width, r.y + r.height);
            line.SetPosition(0, leftBottom1);
            line.SetPosition(1, leftTop1);
            line.SetPosition(2, rightTop1);
            line.SetPosition(3, rightBottom1);
            line.SetPosition(4, leftBottom1);
        }
    }

    private void ResetMatrix()
    {
        for (int y = 0; y < Matrix.GetUpperBound(1); y++)
        {
            for (int x = 0; x < Matrix.GetUpperBound(0); x++)
            {
                //默认值可以通过在矩阵中用1表示
                Matrix[x, y] = 1;

                Rect rectx = new Rect();
                rectx.width = GridSize;
                rectx.height = GridSize;
                rectx.x = x * GridSize;
                rectx.y = y * GridSize;
                lst3.Add(rectx);
            }
        }

        Rect rect = new Rect();

        //构建障碍物
        for (int i = 2; i < 18; i++)
        {
            //障碍物在矩阵中用0表示
            Matrix[i, 12] = 0;
            rect = new Rect();
            rect.width = GridSize;
            rect.height = GridSize;
            rect.x = i * GridSize;
            rect.y = 12 * GridSize;
            lst.Add(rect);
        }
        for (int i = 14; i < 17; i++)
        {
            Matrix[17, i] = 0;
            rect = new Rect();
            rect.width = GridSize;
            rect.height = GridSize;

            rect.x = 17 * GridSize;
            rect.y = i * GridSize;
            lst.Add(rect);
        }

        for (int i = 3; i < 18; i++)
        {
            Matrix[i, 16] = 0;
            rect = new Rect();
            rect.width = GridSize;
            rect.height = GridSize;

            rect.x = i * GridSize;
            rect.y = 16 * GridSize;
            lst.Add(rect);
        }

        for (int i = 15; i < 30; i++)
        {
            Matrix[i, 18] = 0;
            rect = new Rect();
            rect.width = GridSize;
            rect.height = GridSize;

            rect.x = i * GridSize;
            rect.y = 18 * GridSize;
            lst.Add(rect);
        }

        // draw grids. 
        for (int i = 0, length = lst3.Count; i < length; i++)
        {
            var r = lst3[i];
            var line = _Line.GetLineRenderer();
            line.positionCount = 5;
            line.startWidth = 0.01f; 
            Vector3 leftBottom1 = new Vector3(r.x, r.y);
            Vector3 leftTop1 = new Vector3(r.x, r.y + r.height);
            Vector3 rightBottom1 = new Vector3(r.x + r.width, r.y);
            Vector3 rightTop1 = new Vector3(r.x + r.width, r.y + r.height);
            line.SetPosition(0, leftBottom1);
            line.SetPosition(1, leftTop1);
            line.SetPosition(2, rightTop1);
            line.SetPosition(3, rightBottom1);
            line.SetPosition(4, leftBottom1);
        }

        // draw barrier. 
        for (int i = 0, length = lst.Count; i < length; i++)
        {
            var r = lst[i];
            var line = _Line.GetLineRenderer();
            line.positionCount = 5;
            Vector3 leftBottom1 = new Vector3(r.x, r.y);
            Vector3 leftTop1 = new Vector3(r.x, r.y + r.height);
            Vector3 rightBottom1 = new Vector3(r.x + r.width, r.y);
            Vector3 rightTop1 = new Vector3(r.x + r.width, r.y + r.height);
            line.SetPosition(0, leftBottom1);
            line.SetPosition(1, leftTop1);
            line.SetPosition(2, rightTop1);
            line.SetPosition(3, rightBottom1);
            line.SetPosition(4, leftBottom1);
        }
    }

    //[SerializeField] Material _GridMat;
    [SerializeField] LineTest _Line;
    void OnDrawGizmos()
    {
        //lst3.ForEach((r) =>
        //{
        //    otherTest.DrawRect(r, Color.green);
        //});

        lst.ForEach((r) =>
        {
            Grid.DrawRect(r, Color.yellow);
        });

        lst2.ForEach((r) =>
        {
            Grid.DrawRect(r, Color.red);
        });

        lst4.ForEach((r) =>
        {
            Grid.DrawRect(r, Color.white);
        });

    }

    List<Vector3> GetVertexFromRect(List<Rect> list)
    {
        var vs = new List<Vector3>();
        Vector3 leftBottom = Vector3.zero;
        Vector3 leftTop = Vector3.zero;
        Vector3 rightBottom = Vector3.zero;
        Vector3 rightTop = Vector3.zero;
        for (int i = 0, length = list.Count; i < length; i++)
        {
            var rect = list[i];
            Vector3 leftBottom1 = new Vector3(rect.x, rect.y);
            Vector3 leftTop1 = new Vector3(rect.x, rect.y + rect.height);
            Vector3 rightBottom1 = new Vector3(rect.x + rect.width, rect.y);
            Vector3 rightTop1 = new Vector3(rect.x + rect.width, rect.y + rect.height);

            // initialize
            if (i == 0)
            {
                leftBottom = leftBottom1;
                leftTop = leftTop1;
                rightBottom = rightBottom1;
                rightTop = rightTop1;
            }
            Debug.LogFormat("leftBottom={0}, leftBottom1={1}", leftBottom, leftBottom1);
            Debug.LogFormat("leftTop={0}, leftTop1={1}", leftTop, leftTop1);
            Debug.LogFormat("rightBottom={0}, rightBottom1={1}", rightBottom, rightBottom1);
            Debug.LogFormat("rightTop={0}, rightTop1={1}", rightTop, rightTop1);
            if (leftBottom1.x <= leftBottom.x && leftBottom1.y <= leftBottom.y)
            {
                leftBottom = leftBottom1;
            }
            if (leftTop1.x <= leftTop.x && leftTop1.y >= leftTop.y)
            {
                leftTop = leftTop1;
            }
            if (rightBottom1.x >= rightBottom.x && rightBottom1.y <= rightBottom.y)
            {
                rightBottom = rightBottom1;
            }
            if (rightTop1.x >= rightTop.x && rightTop1.y >= rightTop.y)
            {
                rightTop = rightTop1;
            }
        }
        vs.Add(leftBottom);
        vs.Add(leftTop);
        vs.Add(rightTop);
        vs.Add(rightBottom);
        return vs;
    }

    public Rect IndexConvertToRect(int x, int y)
    {
        Rect rect2 = new Rect();

        rect2.x = Convert.ToSingle(x * GridSize);
        rect2.y = Convert.ToSingle(y * GridSize);
        rect2.width = GridSize;
        rect2.height = GridSize;

        return rect2;
    }
}