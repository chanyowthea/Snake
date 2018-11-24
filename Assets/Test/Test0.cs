using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test0 : MonoBehaviour
{
    [SerializeField] float MoveSpeed = 0.2f;
    void Start()
    {
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        if (h != 0 || v != 0)
        {
            Move(new Vector3(h, v, 0) * MoveSpeed);
        }
    }

    Vector3 lastPos;
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(lastPos, new Vector2(1, 1));
    }

    void Move(Vector3 pos)
    {
        lastPos = transform.position + pos;
        //var hit = Physics2D.BoxCast(transform.position + pos, new Vector2(0.5f, 0.5f), 0, transform.right); 
        var c = Physics2D.OverlapBox(transform.position + pos, new Vector2(1f, 1f), 0, LayerMask.GetMask("Enemy"));
        //Debug.DrawLine(transform.position + pos); 
        if (c != null)
        {
            Debug.Log("c.name=" + c.name);
        }
        else
        {
            transform.position += pos;
            transform.transform.right = pos;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.LogFormat("Test0.OnCollisionEnter2D collider name={0}, this.name={1}", collision.collider.name, this.name);
    }
}
