using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// strong body的不能被小的咬断，也不能被大的咬断
// 小的头可以被咬，咬了就挂了【咬了就会掉血？】

// 两个BUG
// 碰撞有问题
// 反复直走会导致缩成一个点

public class GameManager : MonoBehaviour
{
    [SerializeField] GameData _GameData;

    public static GameManager instance { private set; get; }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        RespawnCharacter(0);
        RespawnCharacter(-1);
        //RespawnCharacter(1);
        //RespawnCharacter(2);
        //RespawnCharacter(3);
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
        if (characterId < 0)
        {
            var player = GameObject.Instantiate(_GameData._PlayerPrefab);
            player.SetData(GameManager.instance.GetPlayerData(characterId), 12);
            var pos = new Vector3(RandomUtil.instance.Next(-7, 7), RandomUtil.instance.Next(-7, 7), 0);
            player.transform.position = pos;
            return;
        }

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
