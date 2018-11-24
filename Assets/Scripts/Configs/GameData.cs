using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PlayerData
{
    public int _ID; 
    public Color _HeadColor = Color.blue;
    public Color _StrongColor = Color.red; 
    public Color _BodyColor = Color.yellow; 
    public float _MoveSpeed = 0.1f; 
}

public class GameData : ScriptableObject
{
    public PlayerData[] _Players;
    public Enemy _EnemyPrefab;
    public PlayerController _PlayerPrefab;
}
