using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] Head _Head;
    void Update()
    {
        if (!this.gameObject.activeSelf)
        {
            return;
        }
        SetCemraPos(_Head.transform.position);
    }

    public void SetCemraPos(Vector2 pos)
    {
        Vector3 origin = pos;
        origin.z = transform.position.z;
        transform.position = origin;
    }
}
