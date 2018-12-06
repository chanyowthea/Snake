using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerCamera : MonoBehaviour
{
    Transform _Follow;

    public void SetData(Transform follow)
    {
        _Follow = follow;
    }

    public void ClearData()
    {
        _Follow = null;
    }

    void Update()
    {
        Assert.IsNotNull(_Follow);
        if (!this.gameObject.activeSelf)
        {
            return;
        }
        SetCemraPos(_Follow.position);
    }

    public void SetCemraPos(Vector2 pos)
    {
        Vector3 origin = pos;
        origin.z = transform.position.z;
        transform.position = origin;
    }
}
