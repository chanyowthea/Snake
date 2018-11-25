using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotMove : MonoBehaviour
{
    //[SerializeField] float _MoveSpeed = 0.05f;
    [SerializeField] Vector3 _TargetPos;
    Vector3 _MoveDir;
    BaseCharacter _Character;

    private void Start()
    {
        _Character = GetComponent<BaseCharacter>();
        GenerateTargetPos();
    }

    void Update()
    {
        // redirect
        if (Vector3.Distance(_Character.Head.transform.position, _TargetPos) < 0.1f)
        {
            GenerateTargetPos();
        }
        // move to target position. 
        else
        {
            var rs = _Character.Move(_MoveDir * _Character.MoveSpeed);
            if (!rs)
            {
                GenerateTargetPos();
            }
        }
    }

    void GenerateTargetPos()
    {
        _TargetPos = MapManager.instance.GetRandPosInCurMap(ESpawnType.Character);
        _MoveDir = (_TargetPos - _Character.Head.transform.position).normalized;
    }
}
