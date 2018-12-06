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
    [SerializeField] CountDownText _CountDownText;
    [SerializeField] Text _KillText;
    [SerializeField] Text _DieText;
    List<RankItem> _RankItems = new List<RankItem>();
    string _PromptContentFormat = "你的排名：{0}";
    string _KillCountFormat = "击杀：{0}";
    string _DieTimesFormat = "死亡：{0}";
    uint _HUDCallID;
    uint _RankCallID;

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
        for (int i = 0, length = cs.Count; i < length; i++)
        {
            var item = GameObject.Instantiate(_RankItemPrefab);
            item.gameObject.SetActive(true);
            item.transform.SetParent(_RankContent);
            item.transform.localScale = Vector3.one;
            _RankItems.Add(item);
        }
        UpdateRankInfo();
        _RankCallID = Singleton._DelayUtil.DelayCall(1, UpdateRankInfo, true);
        _HUDCallID = Singleton._DelayUtil.DelayCall(1, UpdateHUDInfo, true);
        _CountDownText.SetCountDownEndTime(GameManager.instance.GetRaceEndTime(), "", "", true, null, OnRaceEnd);
    }

    void UpdateRankInfo()
    {
        var cs = GameManager.instance.GetCharacters();
        CharacterUtil.InsertionSort<BaseCharacter>(cs, CharacterUtil.Compare); // .Sort((BaseCharacter cx, BaseCharacter cy) => cy.TotalLength - cx.TotalLength); 
        for (int i = 0, length = Mathf.Min(cs.Count, _RankItems.Count); i < length; i++)
        {
            var character = cs[i];
            if (character == null)
            {
                continue;
            }
            var item = _RankItems[i];
            item.SetData(character.Name, (int)character.Scores, character.CharacterID == PlayerController.instance.CharacterID);
        }
    }

    void UpdateHUDInfo()
    {
        _KillText.text = string.Format(_KillCountFormat, PlayerController.instance.PlayerInfo_._KillCount);
        _DieText.text = string.Format(_DieTimesFormat, PlayerController.instance.PlayerInfo_._DieTimes);
    }

    void OnRaceEnd()
    {
        var ui = UIManager.Instance.Open<UIPrompt>();
        var cs = GameManager.instance.GetCharacters();
        var index = cs.FindIndex((BaseCharacter temp) => temp.CharacterID == 0);
        ui.SetData(string.Format(_PromptContentFormat, index + 1), "再来一局", OnClickOK);
        UIManager.Instance.Close<UIInput>();
        Singleton._DelayUtil.Timer._TimeScale = 0;
    }

    void OnClickOK()
    {
        UIManager.Instance.ChangeScene();
        SceneManager.LoadScene("Play");
    }

    internal override void Close()
    {
        if (_HUDCallID != 0)
        {
            Singleton._DelayUtil.CancelDelayCall(_HUDCallID);
        }
        if (_RankCallID != 0)
        {
            Singleton._DelayUtil.CancelDelayCall(_RankCallID);
        }
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
