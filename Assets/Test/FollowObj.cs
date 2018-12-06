using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class FollowObj : MonoBehaviour
{
    [SerializeField]
    Camera UI_Camera;//UI相机
    [SerializeField]
    RectTransform image;//UI元素
    [SerializeField]
    GameObject obj;//3D物体
    [SerializeField]
    Canvas ui_Canvas;
    // Update is called once per frame
    void Update()
    {
        UpdateNamePosition();
    }
    /// <summary>
    /// 更新image位置
    /// </summary>
    void UpdateNamePosition()
    {
        Vector2 mouseDown = Camera.main.WorldToScreenPoint(obj.transform.position);
        Vector2 mouseUGUIPos = new Vector2();
        bool isRect = RectTransformUtility.ScreenPointToLocalPointInRectangle(ui_Canvas.transform as RectTransform, mouseDown, UI_Camera, out mouseUGUIPos);
        if (isRect)
        {
            image.anchoredPosition = mouseUGUIPos;
        }
    }
}