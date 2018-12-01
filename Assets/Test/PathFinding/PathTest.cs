using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathTest : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Grid.DrawLine(new Vector3[] { new Vector3(0, 0, 0), new Vector3(1, 1, 0), new Vector3(0, 1, 0) }, Color.red);
        Grid.DrawRect(new Rect(1, 1, 10, 10), Color.green);
        Grid.DrawPath(new Vector3[] { new Vector3(0, 0, 0), new Vector3(1, 1, 0), new Vector3(0, 1, 0) }, Color.white);
    }
}
