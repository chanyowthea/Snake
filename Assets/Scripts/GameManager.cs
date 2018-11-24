using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// strong body的不能被小的咬断，也不能被大的咬断
// 小的头可以被咬，咬了就挂了【咬了就会掉血？】

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] GameData _GameData;

    private void Start()
    {
        RespawnCharacter(0);
        RespawnCharacter(3);
        RespawnCharacter(2);
    }

    public PlayerData GetPlayerData(int characterId)
    {
        var ps = _GameData._Players;
        PlayerData playerData = null;
        for (int i = 0, length = ps.Length; i < length; i++)
        {
            playerData = ps[i];
            if (ps[i]._ID == characterId)
            {
                break;
            }
        }
        return playerData;
    }

    public void RespawnCharacter(int characterId)
    {
        if (characterId != 0)
        {
            var enemy = GameObject.Instantiate(_GameData._EnemyPrefab);
            enemy.SetData(GameManager.instance.GetPlayerData(characterId), 12);
            var pos = new Vector3(RandomUtil.instance.Next(-7, 7), RandomUtil.instance.Next(-7, 7), 0);
            enemy.transform.position = pos; 
        }
        else
        {
            var player = GameObject.Instantiate(_GameData._PlayerPrefab);
            player.SetData(GameManager.instance.GetPlayerData(characterId), 12);
        }
    }
}
