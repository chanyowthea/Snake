using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathUtil
{
    /// <summary>
    /// 计算一个Vector3绕旋转中心旋转指定角度后所得到的向量。
    /// </summary>
    /// <param name="source">旋转前的源Vector3</param>
    /// <param name="axis">旋转轴</param>
    /// <param name="angle">旋转角度</param>
    /// <returns>旋转后得到的新Vector3</returns>
    public static Vector3 V3RotateAround(Vector3 source, Vector3 axis, float angle)
    {
        Quaternion q = Quaternion.AngleAxis(angle, axis);// 旋转系数
        return q * source;// 返回目标点
    }

    public static Rect GetNextRandomRect(Vector3 prevPoint, float sideLength, float headSize = ConstValue._BodyUnitSize)
    {
        // x,y is the center of this rectangle. 
        float signx = Mathf.Sign(prevPoint.x);
        float signy = Mathf.Sign(prevPoint.y);
        Vector2[] signs = new Vector2[]{ new Vector2(-signx, -signy), new Vector2(-signx, signy) ,
            new Vector2(signx, -signy),new Vector2(signx, signy)};
        Rect rect = new Rect();
        for (int i = 0, length = signs.Length; i < length; i++)
        {
            float x = prevPoint.x + signs[i].x * sideLength;
            float y = prevPoint.y + signs[i].y * sideLength;
            if (MapManager.instance.IsInMap(new Vector3(x, y), headSize))
            {
                rect = new Rect(x, y, sideLength, sideLength);
                break;
            }
        }
        return rect;// 返回目标点
    }
}
