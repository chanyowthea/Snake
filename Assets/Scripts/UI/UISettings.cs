using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFramework;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;

class UISettings : BaseUI
{
    [SerializeField] InputField _SpeedInput;
    public UISettings()
    {
        _NaviData._Type = EUIType.Coexisting;
        _NaviData._Layer = EUILayer.Popup;
    }

    public override void Open(NavigationData data = null)
    {
        base.Open(data);
        _SpeedInput.text = RunTimeData._DefaultBaseMoveSpeed.ToString();
    }

    public void OnClickClose()
    {
        UIManager.Instance.Close(this);
    }

    public void OnClickSet()
    {
        int value;
        if (int.TryParse(_SpeedInput.text, out value))
        {
            RunTimeData._DefaultBaseMoveSpeed = value;
        }
    }
}
