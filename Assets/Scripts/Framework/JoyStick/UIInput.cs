using UnityEngine;
using System.Collections;
using System;
using UIFramework;

public class UIInput : BaseUI
{
    public UIInput()
    {
        _NaviData._Type = EUIType.Independent;
        _NaviData._Layer = EUILayer.Resident;
        _NaviData._IsCloseCoexistingUI = false;
    }

    public static UIInput instance { private set; get; }

    private void Awake()
    {
        instance = this;
    }

    public event Action<Vector2> onValueChanged;

    public void OnValueChanged(Vector2 value)
    {
        if (onValueChanged != null)
        {
            onValueChanged(value.normalized);
        }
        PlayerController.instance.OnMove(value.normalized); 
    }
}
