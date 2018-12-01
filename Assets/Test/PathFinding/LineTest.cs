using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineTest : MonoBehaviour
{
    [SerializeField] LineRenderer _Line;

    public LineRenderer GetLineRenderer()
    {
        var line = new GameObject().AddComponent<LineRenderer>(); 
        line.material = _Line.material;
        line.startWidth = _Line.startWidth; 
        return line;
    }
}
