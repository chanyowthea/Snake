using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CharacterUtil
{
    public static Color GetStrongBodyColor(Color bodyColor)
    {
        var color = bodyColor;
        color.a = 1;
        return color;
    }

    //public static void SortByTotalLength(List<BaseCharacter> cs)
    //{
    //    for (int i = 0; i < length; i++)
    //    {

    //    }
    //}

    public static void InsertionSort<T>(IList<T> list, Comparison<T> comparison)
    {
        if (list == null)
            return;
        if (comparison == null)
            return;

        int count = list.Count;
        for (int j = 1; j < count; j++)
        {
            T key = list[j];
            int i = j - 1;
            for (; i >= 0 && comparison(list[i], key) > 0; i--)
            {
                list[i + 1] = list[i];
            }
            list[i + 1] = key;
        }
    }

    public static int Compare(BaseCharacter x, BaseCharacter y)
    {
        int result = 1;
        if (x != null && x is BaseCharacter &&
            y != null && y is BaseCharacter)
        {
            result = x.CompareTo(y);
        }
        return result;
    }
}
