using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test0 : MonoBehaviour
{
    [SerializeField] float MoveSpeed = 0.2f;
    [SerializeField] CircleCollider2D _Collider;
    void Start()
    {
        Debug.Log("rs=" + (Mathf.RoundToInt(-0.6f)));
        Debug.Log("rs=" + (Mathf.CeilToInt(-0.6f)));
    }

    void Update()
    {
        //// for test
        //int layer = 0xffff ^ (1 << LayerMask.NameToLayer("Player"));
        //Debug.LogError("mask=" + layer);
        //var collider = Physics2D.OverlapBox(lastPos, new Vector2(_Collider.radius * 2, _Collider.radius * 2), 0, layer);
        //if (collider != null)
        //{
        //    Debug.LogError("box!!!");
        //}

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
        //Gizmos.DrawWireCube(lastPos, new Vector3(_Collider.radius * 2, _Collider.radius * 2, 1));
        Gizmos.DrawWireSphere(lastPos, _Collider.radius * transform.lossyScale.x);
    }

    void Move(Vector3 pos)
    {
        lastPos = transform.position + pos;
        var c = Physics2D.OverlapCircleAll(transform.position + pos, _Collider.radius * transform.lossyScale.x);
        if (c != null
             && c.Length > 1
            )
        {
            Debug.Log("c.name=" + c[0].name);
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
