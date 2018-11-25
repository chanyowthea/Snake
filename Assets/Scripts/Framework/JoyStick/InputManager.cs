using UnityEngine;
using System.Collections;
using System;

public struct Pos
{
    public int _x;
    public int _y;
    public Pos(int x, int y)
    {
        _x = x;
        _y = y;
    }

    public override string ToString()
    {
        return string.Format("[Pos: x: {0}, y: {1}]", _x, _y);
    }

    public static bool operator ==(Pos pos1, Pos pos2)
    {
        if (pos1._x == pos2._x && pos1._y == pos2._y)
        {
            return true;
        }
        return false;
    }

    public static bool operator !=(Pos pos1, Pos pos2)
    {
        if (pos1._x == pos2._x && pos1._y == pos2._y)
        {
            return false;
        }
        return true;
    }

    public static Pos operator /(Pos pos, float scale)
    {
        return new Pos((int)(pos._x / scale), (int)(pos._y / scale));
    }

    public static Pos operator *(Pos pos, float scale)
    {
        return new Pos((int)(pos._x * scale), (int)(pos._y * scale));
    }

    public int Length { get { return (int)Math.Sqrt(Math.Pow(_x, 2) + Math.Pow(_y, 2)); } }

    public Vector2 ToVector2()
    {
        return new Vector2(_x, _y);
    }

    public static bool IsParalell(Pos x, Pos y)
    {
        x /= x.Length;
        y /= y.Length;
        return x._x == y._x && x._y == y._y;
    }
}

public class InputManager : MonoBehaviour
{
    public static InputManager instance { private set; get; }

    private void Awake()
    {
        instance = this;
    }

    //	public event Action<float> onXChanged; 
    //	public event Action<float> onYChanged; 
    public event Action<Vector2> onValueChanged; 

	[SerializeField] float _coolDownTime = 0.5f; 
	[SerializeField] float _curTime;

//	public void OnHorizontal(float value)
//	{
//		if (_curTime > 0)
//		{
//			return; 
//		}
//		_curTime = _coolDownTime; 
//		if (onXChanged != null)
//		{
//			onXChanged(value); 
//		}
//	}
//
//	public void OnVertical(float value)
//	{
//		if (_curTime > 0)
//		{
//			return; 
//		}
//		_curTime = _coolDownTime; 
//		if (onYChanged != null)
//		{
//			onYChanged(value); 
//		}
//	}

	public void OnValueChanged(Vector2 value)
	{
		//if (_curTime > 0)
		//{
		//	return; 
		//}
		_curTime = _coolDownTime;
        Debug.Log("before onValueChanged != null");
        if (onValueChanged != null)
		{
            Debug.Log("onValueChanged != null"); 
			//Pos pos; 
			if (Math.Abs(value.x) > Math.Abs(value.y))
			{
				//pos = new Pos(1, 0) * Mathf.Sign(value.x); 
			}
			else
			{
				//pos = new Pos(0, 1) * Mathf.Sign(value.y); 
			}
			onValueChanged(value.normalized); 
		}
	}

	void Update()
	{
		if (_curTime <= 0)
		{
			return; 
		}
		_curTime -= Time.deltaTime; 
	}

//	void Awake()
//	{
////		onXChanged += OnXChanged;
////		onYChanged += OnYChanged;

////		onValueChanged += OnValueChang_Out; 
//	}

//	void OnXChanged(float value)
//	{
//		Debug.LogError("OnXChanged=" + value); 
//	}
//
//	void OnYChanged(float value)
//	{
//		Debug.LogError("OnYChanged=" + value); 
//	}

//	void OnValueChang_Out(Vector2 value)
//	{
////		Debug.LogError("OnValueChanged, x=" + value.x + ", y=" + value.y); 
//		Pos pos; 
//		if (Math.Abs(value.x) > Math.Abs(value.y))
//		{
//			pos = new Pos(1, 0) * Mathf.Sign(value.x); 
//		}
//		else
//		{
//			pos = new Pos(0, 1) * Mathf.Sign(value.y); 
//		}
//		Debug.LogError("OnValueChanged, pos=" + pos); 
//	}
}
