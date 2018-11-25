using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ESpawnType
{
    None,
    Food,
    Character,
}

public class MapManager : MonoBehaviour
{
    public static MapManager instance { private set; get; }
    public Vector2 CurMapSize { private set; get; }

    private void Awake()
    {
        instance = this;
        CurMapSize = new Vector2(50, 50);
    }

    [SerializeField] SpriteRenderer _MapGround;
    [SerializeField] SpriteRenderer _MapBG;
    [SerializeField] GameObject[] _Walls;

    private void Start()
    {
        SetSize(CurMapSize);
    }

    public Vector3 GetRandPosInCurMap(int offsetX = 0, int offsetY = 0)
    {
        int x = RandomUtil.instance.Next(
            (int)(-MapManager.instance.CurMapSize.x / 2f) + offsetX,
            (int)(MapManager.instance.CurMapSize.x / 2f) - offsetX);
        int y = RandomUtil.instance.Next(
            (int)(-MapManager.instance.CurMapSize.y / 2f) + offsetX,
            (int)(MapManager.instance.CurMapSize.y / 2f) - offsetX);
        return new Vector3(x, y, 0);
    }

    public Vector3 GetRandPosInCurMap(ESpawnType type)
    {
        if (type == ESpawnType.Character)
        {
            return GetRandPosInCurMap(GameManager.instance.InitBodyLength + 1,
                GameManager.instance.InitBodyLength + 1);
        }
        else if (type == ESpawnType.Food)
        {
            return GetRandPosInCurMap(1, 1);
        }
        return GetRandPosInCurMap();
    }

    void SetSize(Vector2 size)
    {
        _MapGround.size = size;
        _MapBG.size = size * 2;
        _Walls[0].transform.position = new Vector3(-size.x / 2f, 0, 0);
        _Walls[0].transform.localScale = new Vector3(1, size.y, 1);
        _Walls[1].transform.position = new Vector3(size.x / 2f, 0, 0);
        _Walls[1].transform.localScale = new Vector3(1, size.y, 1);
        _Walls[2].transform.position = new Vector3(0, -size.y / 2f, 0);
        _Walls[2].transform.localScale = new Vector3(size.x, 1, 1);
        _Walls[3].transform.position = new Vector3(0, size.y / 2f, 0);
        _Walls[3].transform.localScale = new Vector3(size.x, 1, 1);
    }
}
