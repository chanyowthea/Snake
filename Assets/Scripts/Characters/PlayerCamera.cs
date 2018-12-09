using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerCamera : MonoBehaviour
{
    float _SmoothTime = 0.15f;
    float _VelocityY = 0.0f;
    float _VelocityX = 0.0f;

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
        float newPosY = Mathf.SmoothDamp(transform.position.y, _Follow.position.y,
            ref _VelocityY, _SmoothTime);
        float newPosX = Mathf.SmoothDamp(transform.position.x, _Follow.position.x,
          ref _VelocityX, _SmoothTime);
        SetCemraPos(new Vector3(newPosX, newPosY, transform.position.z));
    }

    public void SetCemraPos(Vector2 pos)
    {
        Vector3 origin = pos;
        origin.z = transform.position.z;
        transform.position = origin;
    }
}
