using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankItem : MonoBehaviour
{
    [SerializeField] Text _NameText;
    [SerializeField] Text _LengthText;

    public void SetData(string name, int totalLength)
    {
        _NameText.text = name;
        _LengthText.text = totalLength.ToString();
    }
}
