using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head : Body
{
    Vector3 lastPos;
    Vector2 lastSize;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(lastPos, Size); 
    }

    public bool Move(Vector3 pos)
    {
        Vector3 targetPos = this.transform.position + pos;
        var collider = Physics2D.OverlapBox(targetPos, Size, 0);
        lastPos = targetPos;
        lastSize = Size;
        if (collider == null)
        {
            if (collider != null)
            {
                Debug.Log("c.Name=" + collider.name + ", character=" + (collider.GetComponent<Body>()._Character.name));
            }
            this.transform.position += pos;
            this.transform.right = pos.normalized;
            return true;
        }
        else
        {
            var body = collider.GetComponent<Body>();
            if (body == null)
            {
                this.transform.position += pos;
                this.transform.right = pos.normalized;
                return true;
            }
            else
            {
                if (body._Character == this._Character)
                { 
                    this.transform.position += pos;
                    this.transform.right = pos.normalized;
                    return true;
                }
                else
                {
                    TryAttack(body);
                }
            }
        }
        return false;
    }

    void TryAttack(Body body)
    {
        if (body == null)
        {
            return;
        }
        if (body._Character == this._Character)
        {
            return;
        }
        if (body.IsStrong)
        {
            return;
        }
        if (body is Head)
        {
            // if the snake attack the head of the other snake whose length is less than this, 
            // then the other snake would die. 
            if (body._Character.TotalLength < this._Character.TotalLength)
            {
                body._Character.Die();
            }
        }
        else
        {
            body._Character.RemoveBody(body.Index);
        }
    }
}
