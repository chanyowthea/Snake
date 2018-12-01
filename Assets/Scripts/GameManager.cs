using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // 碰到障碍物重新检测【提前一段距离】
    // 考虑身体大小
    // 小路无法通过
    // 死亡之后的Body层级要改变
    // 重生的保护时间，10秒
    // 尾巴增长应该在原地添加尾巴

// logic of artificial intelligence


//添加障碍物

public class GameManager : MonoBehaviour
{
    public static GameManager instance { private set; get; }
    [SerializeField] GameData _GameData;
    [SerializeField] Camera _MainCamera;

    public GameObject FoodRoot { private set; get; }
    public GameObject BarrierRoot { private set; get; }
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
    List<BaseCharacter> _Characters = new List<BaseCharacter>();

    public float GameTime
    {
        get
        {
            return _DelayCallUtil.GameTime;
        }
    }

    public float DeltaTime
    {
        get
        {
            return _DelayCallUtil.Timer.DeltaTime;
        }
    }

    public float TimeScale
    {
        get
        {
            return _DelayCallUtil.Timer._TimeScale;
        }
        set
        {
            _DelayCallUtil.Timer._TimeScale = value;
        }
    }
    UniqueIDGenerator _IDGenerator = new UniqueIDGenerator();
    List<int> _RandomNameIDs = new List<int>();

    private void Awake()
    {
        instance = this;
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
        _DelayCallUtil = gameObject.AddComponent<DelayCallUtil>();
        var id = DelayCall(0, DelayLoad);
        _DelayCallIds.Add(id);
    }

    private void OnDestroy()
    {
        _Characters.Clear();
        _RandomNameIDs.Clear();
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
        PrepareBarriers();

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
        //RespawnCharacter(2);
        //RespawnCharacter(3);
        //RespawnCharacter(1);
        //RespawnCharacter(2);
        //RespawnCharacter(3);
    }

    void PrepareBarriers()
    {
        for (int i = 0; i < 10; i++)
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
            var player = GameObject.Instantiate(_GameData._PlayerPrefab);
            player.SetData(GetPlayerInfo(characterId, uniqueID), ConstValue._DefaultBodyLength);
            var pos = MapManager.instance.GetRandPosInCurMap(ESpawnType.Character);
            player.transform.position = pos;
            _Characters.Add(player);
            return player;
        }

        if (characterId != 0)
        {
            var enemy = GameObject.Instantiate(_GameData._EnemyPrefab);
            enemy.SetData(GetPlayerInfo(characterId, uniqueID), ConstValue._DefaultBodyLength);
            var pos = MapManager.instance.GetRandPosInCurMap(ESpawnType.Character);
            enemy.transform.position = pos;
            _Characters.Add(enemy);
            return enemy;
        }
        else
        {
            var player = GameObject.Instantiate(_GameData._PlayerPrefab);
            player.SetData(GetPlayerInfo(characterId, uniqueID), ConstValue._DefaultBodyLength);
            player.transform.position = Vector3.zero;
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
            playerInfo._PlayerData = GameManager.instance.GetPlayerData(characterId);
        }
        else
        {
            playerInfo = RunTimeData.instance.GetPlayerInfo(uniqueID);
        }
        return playerInfo;
    }

    static int _Count;
    public string GetRandomName()
    {
        _Count += 1;
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
        var food = GameObject.Instantiate(_GameData._FoodPrefab);
        food.SetData(GameManager.instance.GetFoodData(foodId));
        var pos = MapManager.instance.GetRandPosInCurMap(ESpawnType.Food);
        food.transform.position = pos;
        food.transform.SetParent(FoodRoot.transform);
    }

    public void RespawnBarrier(int id)
    {
        var go = GameObject.Instantiate(_GameData._BarrierPrefab);
        //food.SetData(GameManager.instance.GetFoodData(foodId));
        var pos = MapManager.instance.GetRandPosInCurMap(ESpawnType.Character);
        go.transform.position = pos;
        go.transform.SetParent(BarrierRoot.transform);
    }

    public Body RespawnBody()
    {
        return GameObject.Instantiate(_GameData._BodyPrefab);
    }

    public void RemoveCharacter(BaseCharacter character)
    {
        if (_Characters.Contains(character))
        {
            _Characters.Remove(character);
        }
    }

    public List<BaseCharacter> GetCharacters()
    {
        return _Characters;
    }

    public float GetRaceEndTime()
    {
        return GameTime + _GameData._RaceTime * 60;
    }
}
