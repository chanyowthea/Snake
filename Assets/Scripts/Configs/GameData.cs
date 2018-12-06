using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PlayerData
{
    public int _ID; 
    public Color _BodyColor = Color.blue;
}

[Serializable]
public class FoodData
{
    public int _ID;
    public Color _Color = Color.blue;
    public int _Scores;
    public bool _IsAddStrongBody; 
}

public class GameData : ScriptableObject
{
    public PlayerData[] _Players;
    public Enemy _EnemyPrefab;
    public PlayerController _PlayerPrefab;
    public FoodData[] _Foods;
    public Food _FoodPrefab;
    public Body _BodyPrefab;
    public Head _HeadPrefab;
    public Barrier _BarrierPrefab;
}
