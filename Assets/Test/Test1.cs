using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test1 : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.LogFormat("Test1.OnCollisionEnter2D collider name={0}, this.name={1}", collision.collider.name, this.name);
    }
}
