using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// -- BUG -- 
// 在原点重生很多次【在原点被攻击立刻重生】
// AI停在一个地方不动
// AI头无法攻击玩家的头了
// AI追逐无法攻击的玩家，比如同样是总长为3
// AI没有逃跑
// 如果在墙壁周围增长，那么尾巴为伸到墙外去
// AI始终在墙壁周围打转
// 超出视野不能追逐
// 追逐有时间，并且超过追逐时间
// 被攻击立即攻击该对象【判断是否】
// 随机移动要判断当前移动的位置，随机到另外一边

// 死亡之后的Body层级要改变
// 重生的保护时间，10秒
// 尾巴增长应该在原地添加尾巴
// 重生要检测降生位置是否有障碍物
// 墙壁应该放到外面去
// 寻路，在某个区域检测
// 为什么不优先攻击少于自己的敌方的头部
// 吃红色的食物加分有问题
// AI偶尔会出现没有目标的情况


// logic of artificial intelligence


//添加障碍物

public class GameManager : MonoBehaviour
{
    public static GameManager instance { private set; get; }
    [SerializeField] GameData _GameData;
    [SerializeField] Camera _MainCamera;

    public GameObject FoodRoot { private set; get; }
    public GameObject BarrierRoot { private set; get; }
    List<uint> _DelayCallIds = new List<uint>();
    Queue<Action> _ActionQueue = new Queue<Action>();
    List<BaseCharacter> _Characters = new List<BaseCharacter>();
    public CharacterPool _EnemyPool = new CharacterPool();
    public CharacterPool _PlayerPool = new CharacterPool();
    public BodyPool _BodyPool = new BodyPool();
    public BodyPool _HeadPool = new BodyPool();
    public FoodPool _FoodPool = new FoodPool();

    UniqueIDGenerator _IDGenerator = new UniqueIDGenerator();
    List<int> _RandomNameIDs = new List<int>();

    private void Awake()
    {
        instance = this;
        Singleton.Init();
    }

    void OnDestroy()
    {
        _Characters.Clear();
        _RandomNameIDs.Clear();
        for (int i = 0, length = _DelayCallIds.Count; i < length; i++)
        {
            Singleton._DelayUtil.CancelDelayCall(_DelayCallIds[i]);
        }
        _DelayCallIds.Clear();
        Singleton.Clear();
    }

    private void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;

        FoodRoot = new GameObject("FoodRoot");
        BarrierRoot = new GameObject("BarrierRoot");
        Singleton._DelayUtil = gameObject.AddComponent<DelayCallUtil>();
        PrepareBarriers();
        Singleton._PathUtil = gameObject.AddComponent<PathFindingUtil>();

        _PlayerPool.SetData(_GameData._PlayerPrefab);
        _EnemyPool.SetData(_GameData._EnemyPrefab);
        _BodyPool.SetData(_GameData._BodyPrefab);
        _HeadPool.SetData(_GameData._HeadPrefab);
        _FoodPool.SetData(_GameData._FoodPrefab);
        var id = Singleton._DelayUtil.DelayCall(0, DelayLoad);
        _DelayCallIds.Add(id);
        Singleton._DelayUtil.DelayCall(3, PrepareFoods, true);
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
    }

    //void FixedUpdate()
    //{
    //    _DelayCallUtil.FixedRunOneFrame();
    //}

    void DelayLoad()
    {
        _ActionQueue.Enqueue(PrepareResource);
        _ActionQueue.Enqueue(PrepareCharacters);
        _ActionQueue.Enqueue(PrepareUI);
        for (int i = 0; i < 10; i++)
        {
            _ActionQueue.Enqueue(PrepareFoods);
        }
    }

    void PrepareCharacters()
    {
        _MainCamera.gameObject.SetActive(false);
        var list = new List<PlayerNameCSV>();
        var csvs = ConfigDataManager.instance.GetDataList<PlayerNameCSV>();
        for (int i = 0; i < csvs.Count; i++)
        {
            list.Add(csvs[i] as PlayerNameCSV);
        }
        for (int i = 0; list.Count > 0; i++)
        {
            var index = RandomUtil.instance.Next(0, list.Count);
            _RandomNameIDs.Add(list[index]._ID);
            list.RemoveAt(index);
        }
        RespawnCharacter(0);
#if UNITY_EDITOR
        //RespawnCharacter(-1);
#endif
        RespawnCharacter(1);
        RespawnCharacter(2);
        RespawnCharacter(3);
        RespawnCharacter(1);
        RespawnCharacter(2);
        RespawnCharacter(3);
    }

    void PrepareBarriers()
    {
        for (int i = 0; i < 30; i++)
        {
            RespawnBarrier(1);
        }
    }

    void PrepareFoods()
    {
        for (int i = 0; i < 10; i++)
        {
            RespawnFood(1);
        }
        for (int i = 0; i < 1; i++)
        {
            RespawnFood(2);
        }
    }

    void PrepareUI()
    {
        UIFramework.UIManager.Instance.Open<UIHUD>();
        UIFramework.UIManager.Instance.Open<UIInput>();
    }

    void PrepareResource()
    {
        ConfigDataManager.instance.LoadCSV<UICSV>("UI");
        ConfigDataManager.instance.LoadCSV<PlayerNameCSV>("PlayerName");
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

    public BaseCharacter RespawnCharacter(int characterId, uint uniqueID = 0)
    {
        if (characterId < 0)
        {
            var player = _PlayerPool.AllocObject();
            player.transform.position = Vector3.zero;
            player.SetData(GetPlayerInfo(characterId, uniqueID), ConstValue._DefaultBodyLength);
            _Characters.Add(player);
            return player;
        }

        if (characterId != 0)
        {
            var enemy = _EnemyPool.AllocObject();
            enemy.transform.position = Vector3.zero;
            enemy.SetData(GetPlayerInfo(characterId, uniqueID), ConstValue._DefaultBodyLength);
            _Characters.Add(enemy);
            return enemy;
        }
        else
        {
            var player = _PlayerPool.AllocObject();
            player.transform.position = Vector3.zero;
            player.SetData(GetPlayerInfo(characterId, uniqueID), ConstValue._DefaultBodyLength);
            _Characters.Add(player);
            return player;
        }
    }

    public PlayerInfo GetPlayerInfo(int characterId, uint uniqueID)
    {
        PlayerInfo playerInfo = null;
        if (uniqueID == 0)
        {
            uniqueID = _IDGenerator.GetUniqueID();
            playerInfo = RunTimeData.instance.GetPlayerInfo(uniqueID);
            playerInfo._UniqueID = uniqueID;
            playerInfo._Name = GetRandomName();
            var pos = MapManager.instance.GetRandPosInCurMap(ESpawnType.Character);
            playerInfo._BirthPos = pos;
            playerInfo._PlayerData = GameManager.instance.GetPlayerData(characterId);
        }
        else
        {
            playerInfo = RunTimeData.instance.GetPlayerInfo(uniqueID);
        }
        return playerInfo;
    }

    public string GetRandomName()
    {
        int id = _RandomNameIDs[0];
        _RandomNameIDs.RemoveAt(0);
        var csv = ConfigDataManager.instance.GetData<PlayerNameCSV>(id.ToString());
        return csv == null ? "" : csv._Name;
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
        var food = _FoodPool.AllocObject();
        food.SetData(GameManager.instance.GetFoodData(foodId));
        var pos = MapManager.instance.GetValidRandPosInCurMap();
        food.transform.position = pos;
        food.transform.SetParent(FoodRoot.transform);
    }

    public void RemoveFood(Food obj)
    {
        Assert.IsNotNull(obj);
        obj.ClearData();
        _FoodPool.CollectObject(obj);
    }

    public void RespawnBarrier(int id)
    {
        var go = GameObject.Instantiate(_GameData._BarrierPrefab);
        //food.SetData(GameManager.instance.GetFoodData(foodId));
        var pos = MapManager.instance.GetRandPosInCurMap(ESpawnType.Character);
        go.transform.position = pos;
        go.transform.localScale = new Vector3(RandomUtil.instance.Next(1, 5), RandomUtil.instance.Next(1, 5), 1);
        go.transform.SetParent(BarrierRoot.transform);
    }

    public Head RespawnHead()
    {
        return _HeadPool.AllocObject() as Head;
    }

    public void RemoveHead(Head obj)
    {
        Assert.IsNotNull(obj);
        obj.ClearData();
        _HeadPool.CollectObject(obj);
    }

    public Body RespawnBody()
    {
        return _BodyPool.AllocObject();
    }

    public void RemoveBody(Body obj)
    {
        Assert.IsNotNull(obj);
        obj.ClearData();
        _BodyPool.CollectObject(obj);
    }

    public void RemoveCharacter(Enemy character)
    {
        Assert.IsNotNull(character);
        if (_Characters.Contains(character))
        {
            _Characters.Remove(character);
        }
        character.ClearData();
        _EnemyPool.CollectObject(character);
    }

    public void RemoveCharacter(PlayerController character)
    {
        Assert.IsNotNull(character);
        if (_Characters.Contains(character))
        {
            _Characters.Remove(character);
        }
        character.ClearData();
        _PlayerPool.CollectObject(character);
    }

    public List<BaseCharacter> GetCharacters()
    {
        return _Characters;
    }

    public float GetRaceEndTime()
    {
        return Singleton._DelayUtil.GameTime + ConstValue._RaceTime0 * 60;
    }
}
