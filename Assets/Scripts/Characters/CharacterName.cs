using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterName : MonoBehaviour
{
    [SerializeField] Text _NameText; 
    [SerializeField] Head _Head;
    Vector3 _Offset = new Vector3(0, 0.4f, 0);

    void Update()
    {
        if (!this.gameObject.activeSelf)
        {
            return;
        }
        transform.position = _Head.transform.position + _Offset;
    }

    public void SetData(string name_)
    {
        _NameText.text = name_; 
    }
}
