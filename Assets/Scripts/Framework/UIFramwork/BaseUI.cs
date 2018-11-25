using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIFramework
{
    public class BaseUI : MonoBehaviour
    {
        [SerializeField]
        internal NavigationData _NaviData = new NavigationData();

        Canvas _CanvasComp;
        public Canvas CanvasComp
        {
            get
            {
                if (_CanvasComp == null)
                {
                    _CanvasComp = GetComponent<Canvas>();
                }
                return _CanvasComp;
            }
        }
        public Action _OnOpen;
        public Action _OnClose;
        public Action _OnShow;
        public Action _OnHide;

        public virtual void Open(NavigationData data = null)
        {
            Show();
            if (_OnOpen != null)
            {
                _OnOpen();
            }

            if (data != null)
            {
                _NaviData = data;
            }

            SetData();
        }

        internal virtual void Close()
        {
            if (_OnClose != null)
            {
                _OnClose();
            }

            // 处理显示状态
            if (_NaviData._CloseByDestroy)
            {
                Destroy(this.gameObject);
            }
            else
            {
                ClearData();
                Hide();
            }
        }

        //public void CloseExternal()
        //{
        //    // 这里看看有什么优化方案，目前是Close调用UIManager.Close,然后UIManager调用CloseInternal
        //    UIManager.Instance.Close(this);
        //}

        internal virtual void Show()
        {
            gameObject.SetActive(true);
            if (_OnShow != null)
            {
                _OnShow();
            }
        }

        internal virtual void Hide()
        {
            if (_OnHide != null)
            {
                _OnHide();
            }
            gameObject.SetActive(false);
        }

        internal virtual void SetData()
        {

        }

        internal virtual void ClearData()
        {

        }
    }
}
