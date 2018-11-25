using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// strong body的不能被小的咬断，也不能被大的咬断
// 小的头可以被咬，咬了就挂了【咬了就会掉血？】

// -- BUG -- 


// 速度要一致
// AI

public class GameManager : MonoBehaviour
{
    public static GameManager instance { private set; get; }
    [SerializeField] GameData _GameData;
    [SerializeField] Camera _MainCamera;

    public GameObject FoodRoot { private set; get; }
    public int InitBodyLength
    {
        get
        {
            return _GameData._InitBodyLength;
        }
    }
    DelayCallUtil _DelayCallUtil;
    List<uint> _DelayCallIds = new List<uint>();
    Queue<Action> _ActionQueue = new Queue<Action>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        _MainCamera.gameObject.SetActive(false);
        FoodRoot = new GameObject("FoodRoot");
        _DelayCallUtil = gameObject.AddComponent<DelayCallUtil>();
        var id = DelayCall(0, DelayLoad);
        _DelayCallIds.Add(id);
    }

    private void OnDestroy()
    {
        for (int i = 0, length = _DelayCallIds.Count; i < length; i++)
        {
            CancelDelayCall(_DelayCallIds[i]);
        }
        _DelayCallIds.Clear(); 
    }

    public uint DelayCall(float delayTime, Action action, bool isRepeated = false)
    {
        return _DelayCallUtil.DelayCall(delayTime, action, isRepeated);
    }

    public void CancelDelayCall(uint id)
    {
        _DelayCallUtil.CancelDelayCall(id);
    }

    private void Update()
    {
        if (_ActionQueue.Count > 0)
        {
            // dequeue one action per frame. 
            var queue = _ActionQueue.Dequeue();
            if (queue != null)
            {
                queue();
            }
        }
        _DelayCallUtil.RunOneFrame();
    }

    void FixedUpdate()
    {
        _DelayCallUtil.FixedRunOneFrame();
    }

    void DelayLoad()
    {
        _ActionQueue.Enqueue(PrepareCharacters);
        for (int i = 0; i < 10; i++)
        {
            _ActionQueue.Enqueue(PrepareFoods);
        }
    }

    void PrepareCharacters()
    {
        RespawnCharacter(0);
        RespawnCharacter(-1);
        RespawnCharacter(1);
        RespawnCharacter(2);
        RespawnCharacter(3);
    }

    void PrepareFoods()
    {
        for (int i = 0; i < 10; i++)
        {
            RespawnFood(1);
        }
    }

    public PlayerData GetPlayerData(int characterId)
    {
        var ps = _GameData._Players;
        PlayerData playerData = null;
        for (int i = 0, length = ps.Length; i < length; i++)
        {
            if (ps[i]._ID == characterId)
            {
                playerData = ps[i];
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
            player.SetData(GameManager.instance.GetPlayerData(characterId), _GameData._InitBodyLength);
            var pos = MapManager.instance.GetRandPosInCurMap(ESpawnType.Character);
            player.transform.position = pos;
            return;
        }

        if (characterId != 0)
        {
            var enemy = GameObject.Instantiate(_GameData._EnemyPrefab);
            enemy.SetData(GameManager.instance.GetPlayerData(characterId), _GameData._InitBodyLength);
            var pos = MapManager.instance.GetRandPosInCurMap(ESpawnType.Character);
            enemy.transform.position = pos;
        }
        else
        {
            var player = GameObject.Instantiate(_GameData._PlayerPrefab);
            player.SetData(GameManager.instance.GetPlayerData(characterId), _GameData._InitBodyLength);
            player.transform.position = Vector3.zero;
        }
    }

    public FoodData GetFoodData(int foodId)
    {
        var fs = _GameData._Foods;
        FoodData foodData = null;
        for (int i = 0, length = fs.Length; i < length; i++)
        {
            if (fs[i]._ID == foodId)
            {
                foodData = fs[i];
                break;
            }
        }
        return foodData;
    }

    public void RespawnFood(int foodId)
    {
        var food = GameObject.Instantiate(_GameData._FoodPrefab);
        food.SetData(GameManager.instance.GetFoodData(foodId));
        var pos = MapManager.instance.GetRandPosInCurMap(ESpawnType.Food);
        food.transform.position = pos;
        food.transform.SetParent(FoodRoot.transform);
    }

    public Body RespawnBody()
    {
        return GameObject.Instantiate(_GameData._BodyPrefab);
    }
}
