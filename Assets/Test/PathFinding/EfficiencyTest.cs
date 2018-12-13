using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EfficiencyTest : MonoBehaviour
{
    // foreach
    // 拼接字符串
    void Start()
    {
        List<int> list = new List<int> { 5, 4, 3, 7 };
        for (int i = list.Count / 2 - 1; i >= 0; i--)
        {
            ConstructMaxHeap(list, i, list.Count);
        }
        Sort(list);
        string s = "";
        for (int i = 0, length = list.Count; i < length; i++)
        {
            s += list[i] + ", ";
        }
        Debug.Log(s);
    }
    
    // 时间复杂度和空间复杂度
    void ConstructMaxHeap(List<int> list, int parentIndex, int constructLength)
    {
        int parentValue = list[parentIndex];
        for (int childIndex = parentIndex * 2 + 1; childIndex < constructLength; childIndex = childIndex * 2 + 1)
        {
            if (childIndex + 1 > constructLength && list[childIndex] < list[childIndex + 1])
            {
                childIndex++;
            }
            if (list[childIndex] > parentValue)
            {
                list[parentIndex] = list[childIndex];
                parentIndex = childIndex;
            }
            else
            {
                break;
            }
        }
        list[parentIndex] = parentValue;
    }

    void Sort(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            Swap(list, 0, i);
            ConstructMaxHeap(list, 0, i);
        }
    }

    void Swap(List<int> list, int index0, int index1)
    {
        var temp = list[index0];
        list[index0] = list[index1];
        list[index1] = temp;
    }
}
