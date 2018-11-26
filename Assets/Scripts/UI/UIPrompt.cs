using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFramework;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;

class UIPrompt : BaseUI
{
    [SerializeField] Text _ContentText;
    [SerializeField] Text _OKBtnText;
    Action _OnOK;

    public UIPrompt()
    {
        _NaviData._Type = EUIType.Coexisting;
        _NaviData._Layer = EUILayer.Popup;
    }

    public void SetData(string content, string OKString, Action onOK)
    {
        _ContentText.text = content;
        _OKBtnText.text = OKString;
        _OnOK = onOK; 
    }

    public void OnClickOK()
    {
        if (_OnOK != null)
        {
            _OnOK();
        }
    }
}
