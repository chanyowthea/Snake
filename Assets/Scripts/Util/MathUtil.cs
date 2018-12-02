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

    public static Rect GetNextRandomRect(Vector3 curPoint, float sideLength, float headSize = ConstValue._BodyUnitSize)
    {
        // x,y is the center of this rectangle. 
        Rect rect = new Rect(0, 0, sideLength, sideLength);
        float x = float.MaxValue;
        float y = float.MaxValue;
        bool getPos = false;
        for (int i = 0; i < ConstValue._MaxLoopTime; i++)
        {
            if (!MapManager.instance.IsInMap(new Vector3(x, y), headSize))
            {
                x = curPoint.x + RandomUtil.instance.Next(-1, 2) * sideLength;
                y = curPoint.y + RandomUtil.instance.Next(-1, 2) * sideLength;
            }
            else
            {
                getPos = true;
                break;
            }
        }
        if (!getPos)
        {
            Debug.LogError("does not get target pos! ");
            x = y = 0;
        }
        rect = new Rect(x, y, sideLength, sideLength);
        return rect;
    }
}
