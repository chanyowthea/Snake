using UnityEngine;
using System.Collections;
using System;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public class AxisEvent : UnityEvent<float> { }
[Serializable]
public class AxisValueEvent : UnityEvent<Vector2> { }

public class VJoyStick : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	public enum ShowType
	{
		DontShow,
		DragOnly,
		Always,
	}
	public ShowType showType;


	private Vector2 Screen2Local(Vector2 screenPos, Camera evenCamera)
	{
		Vector2 pointLocalPos;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, screenPos, evenCamera, out pointLocalPos);
		return pointLocalPos;
	}

	private Vector2 Local2Screen(Vector3 pos, Camera evenCamera)
	{
		return RectTransformUtility.WorldToScreenPoint(evenCamera, pos);
	}

	private bool _isDragging;
	public void OnBeginDrag(PointerEventData eventData)
	{
		if (showType == ShowType.DragOnly)
		{
			ShowJoystick();
			SetJoystick(Screen2Local(eventData.pressPosition, eventData.pressEventCamera));
		}
		_eventOffset = Local2Screen(_rtf_Rocker.position, eventData.pressEventCamera) - eventData.pressPosition;
		this._isDragging = true;
		SetValue(eventData);
		isUpdate = true;
		this._velocity = Vector2.zero;
	}

	public void OnDrag(PointerEventData eventData)
	{
		SetValue(eventData);
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		this._isDragging = false;

		if (showType == ShowType.DragOnly)
		{
			HideJoystick();
		}
	}

	private bool isUpdate;
	void UpdateValue()
	{
		if (!isUpdate) { return; }
		// Debug.Log("开始更新Value");
		if (this._isDragging)
		{
			this.Value = Vector2.SmoothDamp(this.Value, this._targetValue, ref this._velocity, this._inertia, float.MaxValue, Time.deltaTime);
		}
		else
		{
			if (Value.magnitude > 0.01F)
			{
				this.Value = Vector2.SmoothDamp(this.Value, Vector2.zero, ref this._velocity, this._inertia, float.MaxValue, Time.deltaTime);
				// Debug.Log("归位Value" + Value);
			}
			else
			{
				this.Value = Vector2.zero;
				isUpdate = false;
			}
		}
	}

	[SerializeField]
	private RectTransform _rtf_Base;
	[SerializeField]
	private RectTransform _rtf_Rocker;
	[SerializeField]
	private float _inertia = 0.05f;
	[SerializeField, Range(0f, 1f)]
	private float offsetRange;

	[SerializeField] GameObject _unDragObj; 

	private void SetRockerX(float value)
	{
		Vector2 pos = this._rtf_Rocker.anchoredPosition;
		pos.x = value * this.MaxValue.x + this._rtf_Base.anchoredPosition.x;
		this._rtf_Rocker.anchoredPosition = pos;
	}
	private void SetRockerY(float value)
	{
		Vector2 pos = this._rtf_Rocker.anchoredPosition;
		pos.y = value * this.MaxValue.y + this._rtf_Base.anchoredPosition.y;
		this._rtf_Rocker.anchoredPosition = pos;
	}
	private void SetJoystick(Vector2 pos)
	{
		_rtf_Base.anchoredPosition = pos;
		_rtf_Rocker.anchoredPosition = pos;
	}
	private void ShowJoystick()
	{
		_rtf_Base.gameObject.SetActive(true);
		_rtf_Rocker.gameObject.SetActive(true);
		_unDragObj.SetActive(false); 

	}
	private void HideJoystick()
	{
		_rtf_Base.gameObject.SetActive(false);
		_rtf_Rocker.gameObject.SetActive(false);
		_unDragObj.SetActive(true); 
	}

	private Vector2 MaxValue = Vector2.zero;
	private Vector2 pivotOffset = Vector2.zero;
	void Awake()
	{
		HideJoystick(); 

		//移动最大范围
		this.MaxValue = new Vector2(_rtf_Base.rect.width, _rtf_Base.rect.height) * 0.5f * this.offsetRange;
		this.pivotOffset = Vector2.one / 2 - this._rtf_Base.pivot;
		this.pivotOffset.y *= this._rtf_Base.rect.height;
		this.pivotOffset.x *= this._rtf_Base.rect.width;
		if (showType == ShowType.DragOnly || showType == ShowType.Always)
		{
			OnXChange.AddListener(SetRockerX);
			OnYChange.AddListener(SetRockerY);
		}
		if (showType == ShowType.Always)
		{
			ShowJoystick();
		}
	}

	void Update()
	{
		#if UNITY_EDITOR
		this.MaxValue = new Vector2(_rtf_Base.rect.width, _rtf_Base.rect.height) * 0.5f * this.offsetRange;
		this.pivotOffset = Vector2.one / 2 - this._rtf_Base.pivot;
		this.pivotOffset.y *= this._rtf_Base.rect.height;
		this.pivotOffset.x *= this._rtf_Base.rect.width;
		#endif
		UpdateValue();
	}

	void OnEnable()
	{
		HideJoystick(); 
		this._isDragging = false;
		this._velocity = Vector2.zero;
		this._targetValue = Vector2.zero;
		this.Value = Vector2.zero;
		this._eventOffset = Vector2.zero;
	}

	void OnDisable()
	{
		HideJoystick(); 
		this.Value = Vector2.zero;
	}

	private Vector2 _velocity;
	private Vector2 _targetValue;
	private Vector2 _eventOffset;
	private void SetValue(PointerEventData eventData)
	{
		Vector2 pointLocalPos;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(this._rtf_Base, eventData.position + _eventOffset, eventData.pressEventCamera, out pointLocalPos);
		pointLocalPos -= this.pivotOffset;
		if (enableX)
		{
			this._targetValue.x = pointLocalPos.x / this.MaxValue.x;
		}
		if (enableY)
		{
			this._targetValue.y = pointLocalPos.y / this.MaxValue.y;
		}
		if (this._targetValue.magnitude > 1)
		{
			this._targetValue = this._targetValue.normalized;
		}
	}

	private Vector2 _value;
	private Vector2 Value
	{
		get { return _value; }
		set
		{
			this._value = value;
			if (enableX)
			{
				OnXChange.Invoke(value.x);
			}
			if (enableY)
			{
				OnYChange.Invoke(value.y);
			}

			// 为了2D特殊处理
			if (value.x != 0 && value.y != 0)
			{
				OnValueChange.Invoke(value);
			}
		}
	}
	public bool enableX;
	public AxisEvent OnXChange = new AxisEvent();
	public bool enableY;
	public AxisEvent OnYChange = new AxisEvent();
	public AxisValueEvent OnValueChange = new AxisValueEvent();
}
