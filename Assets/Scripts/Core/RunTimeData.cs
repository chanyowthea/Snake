using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo
{
    public uint _UniqueID;
    public int _KillCount;
    public int _DieTimes;
    public string _Name;
    public int _Scores;
    public PlayerData _PlayerData;
    public Vector3 _BirthPos;
}

public class RunTimeData : TSingleton<RunTimeData>
{
    public static float _MinMoveDelta { get { return ConstValue._BodyUnitSize * 0.1f * _DefaultBaseMoveSpeed / 3f; } }
    public static float _DefaultBaseMoveSpeed = 5;

    Dictionary<uint, PlayerInfo> _ScoresDict = new Dictionary<uint, PlayerInfo>();

    public void UpdateScores(uint characterUniqueId, int scores)
    {
        if (!_ScoresDict.ContainsKey(characterUniqueId))
        {
            _ScoresDict[characterUniqueId] = new PlayerInfo();
        }
        _ScoresDict[characterUniqueId]._Scores = scores;
    }

    public void UpdatePlayerInfo(uint characterUniqueId, PlayerInfo info)
    {
        _ScoresDict[characterUniqueId] = info;
    }

    public PlayerInfo GetPlayerInfo(uint characterUniqueId)
    {
        if (!_ScoresDict.ContainsKey(characterUniqueId))
        {
            _ScoresDict[characterUniqueId] = new PlayerInfo();
        }
        return _ScoresDict[characterUniqueId];
    }
}
