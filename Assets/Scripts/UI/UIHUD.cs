using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFramework;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

class UIHUD : BaseUI
{
    [SerializeField] RectTransform _RankContent;
    [SerializeField] RankItem _RankItemPrefab;
    List<RankItem> _RankItems = new List<RankItem>();

    public UIHUD()
    {
        _NaviData._Type = EUIType.FullScreen;
        _NaviData._Layer = EUILayer.FullScreen;
        _NaviData._IsCloseCoexistingUI = false;
    }

    private void Start()
    {
        _RankItemPrefab.gameObject.SetActive(false);
    }

    public override void Open(NavigationData data = null)
    {
        base.Open(data);
        var cs = GameManager.instance.GetCharacters();
        Debugger.LogError("cs=" + cs.Count);
        for (int i = 0, length = cs.Count; i < length; i++)
        {
            var character = cs[i];
            var item = GameObject.Instantiate(_RankItemPrefab);
            item.gameObject.SetActive(true);
            item.transform.SetParent(_RankContent);
            item.transform.localScale = Vector3.one;

            // TODO Name
            item.SetData(character.name, character.TotalLength); 
            _RankItems.Add(item);
        }
    }

    internal override void Close()
    {
        for (int i = 0, length = _RankItems.Count; i < length; i++)
        {
            var item = _RankItems[i];
            GameObject.Destroy(item.gameObject);
        }
        _RankItems.Clear();
        base.Close();
    }

#if UNITY_EDITOR || UNITY_EDITOR_WIN
    private void Update()
    {

    }
#endif
}
