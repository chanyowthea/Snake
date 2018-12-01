using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrawRectTest : MonoBehaviour
{
    private float GridSize = 0.2f; //单位格子大小
    List<Rect> lst = new List<Rect>();

    void Awake()
    {
        //构建障碍物
        for (int y = 13; y <= 28; y++)
        {
            for (int x = 0; x <= 7; x++)
            {
                //障碍物在矩阵中用0表示

                //Matrix[x, y] = 0;

                Rect rect = new Rect();
                rect.width = GridSize;
                rect.height = GridSize;
                rect.x = x * GridSize;
                rect.y = 6 - y * GridSize;

                lst.Add(rect);
            }

        }

        int move = 0;

        for (int x = 8; x <= 15; x++)
        {
            for (int y = 13; y <= 19; y++)
            {
                Rect rect = new Rect();
                rect.width = GridSize;
                rect.height = GridSize;
                rect.x = x * GridSize;
                rect.y = 6 - (y - move) * GridSize;

                lst.Add(rect);
            }

            move = x % 2 == 0 ? move + 1 : move;
        }

        int start_y = 4;

        int end_y = 10;

        for (int x = 16; x <= 23; x++)
        {
            for (int y = start_y + 1; y <= end_y + 1; y++)
            {
                Rect rect = new Rect();
                rect.width = GridSize;
                rect.height = GridSize;
                rect.x = x * GridSize;
                rect.y = 6 - (y + move) * GridSize;

                lst.Add(rect);
            }
            start_y = x % 3 == 0 ? start_y + 1 : start_y;
            end_y = x % 3 == 0 ? end_y - 1 : end_y;
        }
    }
    void OnDrawGizmos()
    {
        lst.ForEach((r) =>
        {
            Grid.DrawRect(r, Color.yellow);
        });

    }
}