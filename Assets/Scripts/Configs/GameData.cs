using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PlayerData
{
    public int _ID; 
    public Color _BodyColor = Color.blue;
    public float _MoveSpeed = 0.1f;
}

[Serializable]
public class FoodData
{
    public int _ID;
    public Color _Color = Color.blue;
}

public class GameData : ScriptableObject
{
    public int _InitBodyLength = 3; 
    public PlayerData[] _Players;
    public Enemy _EnemyPrefab;
    public PlayerController _PlayerPrefab;
    public FoodData[] _Foods;
    public Food _FoodPrefab;
    public Body _BodyPrefab;
}
