using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class StringUtil
{
    public static string Append(this string sourceContent, string appendContent)
    {
        StringBuilder sb = new StringBuilder(sourceContent); 
        return sb.Append(appendContent).ToString();
    }
}
