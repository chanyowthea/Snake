using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarTest : MonoBehaviour
{
    SimpleAStar _PathFinder = new SimpleAStar(); 
    void Start()
    {
        _PathFinder.Init(new Components.Struct.Point2D(20, 20));
        var nodes = _PathFinder.FindPath(new PathNode(0, 0), new PathNode(10, 0));
        Debug.Log("nodes.Count=" + nodes.Count);
        for (int i = 0, length = nodes.Count; i < length; i++)
        {
            Debug.LogFormat("node {0}={1}", i, nodes[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
