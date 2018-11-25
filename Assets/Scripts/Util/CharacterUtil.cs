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
}
