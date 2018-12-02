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
    [SerializeField] BoxCollider2D[] _Walls;

    private void Start()
    {
        SetSize(CurMapSize);
    }

    public bool IsInMap(Vector3 pos, float headSize = ConstValue._BodyUnitSize)
    {
        return pos.x < CurMapSize.x / 2f - headSize / 2f && pos.x > -CurMapSize.x / 2f + headSize / 2f
            && pos.y < CurMapSize.y / 2f - headSize / 2f && pos.y > -CurMapSize.y / 2f + headSize / 2f;
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

    public Vector3 GetRandPosInRect(Rect rect, float headSize = ConstValue._BodyUnitSize)
    {
        // check valid
        Vector3 pos = Vector3.zero;
        bool findValidPos = false;
        int xMin = Mathf.CeilToInt(Mathf.Max(-MapManager.instance.CurMapSize.x / 2f + headSize / 2f, rect.x - rect.width / 2f));
        int xMax = Mathf.FloorToInt(Mathf.Min(MapManager.instance.CurMapSize.x / 2f - headSize / 2f, rect.x + rect.width / 2f));
        int yMin = Mathf.CeilToInt(Mathf.Max(-MapManager.instance.CurMapSize.y / 2f + headSize / 2f, rect.y - rect.height / 2f));
        int yMax = Mathf.FloorToInt(Mathf.Min(MapManager.instance.CurMapSize.y / 2f - headSize / 2f, rect.y + rect.height / 2f));
        for (int i = 0; i < ConstValue._MaxLoopTime; i++)
        {
            pos.x = RandomUtil.instance.Next(xMin,xMax);
            pos.y = RandomUtil.instance.Next(yMin, yMax);
            var c = Physics2D.OverlapCircle(pos, ConstValue._BodyUnitSize, 0xffff ^ LayerMask.GetMask("Food"));
            if (c == null)
            {
                findValidPos = true;
                break;
            }
        }
        if (!findValidPos)
        {
            Debugger.LogError("cannot find a valid position! ");
        }
        return pos;
    }

    public Vector3 GetRandPosInCurMap(ESpawnType type)
    {
        if (type == ESpawnType.Character)
        {
            Vector3 pos = Vector3.zero;
            bool findValidPos = false;
            for (int i = 0; i < ConstValue._MaxLoopTime; i++)
            {
                pos = GetRandPosInCurMap(GameManager.instance.InitBodyLength + 1,
                    GameManager.instance.InitBodyLength + 1);
                var c = Physics2D.OverlapCircle(pos, ConstValue._BodyUnitSize, 0xffff ^ LayerMask.GetMask("Food"));
                if (c == null)
                {
                    findValidPos = true;
                    break;
                }
            }
            if (!findValidPos)
            {
                Debugger.LogError("cannot find a valid position! ");
            }
            return pos;
        }
        else if (type == ESpawnType.Food)
        {
            return GetRandPosInCurMap(1, 1);
        }
        return GetRandPosInCurMap();
    }

    void SetSize(Vector2 size)
    {
        float factor = 2;
        _MapGround.size = size / factor;
        _MapGround.transform.localScale = Vector3.one * factor;
        _MapBG.size = size * 2;
        int defaultWidth = 1;
        _Walls[0].transform.position = new Vector3(-size.x / 2f - defaultWidth / 2f, 0, 0);
        _Walls[0].transform.localScale = new Vector3(defaultWidth, size.y, defaultWidth);
        _Walls[1].transform.position = new Vector3(size.x / 2f + defaultWidth / 2f, 0, 0);
        _Walls[1].transform.localScale = new Vector3(defaultWidth, size.y, defaultWidth);
        _Walls[2].transform.position = new Vector3(0, -size.y / 2f - defaultWidth / 2f, 0);
        _Walls[2].transform.localScale = new Vector3(size.x, defaultWidth, defaultWidth);
        _Walls[3].transform.position = new Vector3(0, size.y / 2f + defaultWidth / 2f, 0);
        _Walls[3].transform.localScale = new Vector3(size.x, defaultWidth, defaultWidth);
    }
}
