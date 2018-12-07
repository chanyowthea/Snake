using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class CancelRayCastUtil : MonoBehaviour
{
    void Start()
    {
        var ts = transform.GetAllComponentsInChildren<Graphic>();
        foreach (var item in ts)
        {
            item.raycastTarget = false;
        }
        DestroyImmediate(this);
    }
}
