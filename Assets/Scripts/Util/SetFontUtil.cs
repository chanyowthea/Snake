using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class SetFontUtil : MonoBehaviour
{
	void Start()
	{
		var font = Resources.Load<Font>("Font/方正粗活意简体"); 
		var ts = transform.GetAllComponentsInChildren<Text>(); 
		foreach (var item in ts)
		{
			item.font = font; 
		}
		DestroyImmediate(this); 
	}
}

public static class TransformUtil
{
	public static List<T> GetAllComponentsInChildren<T>(this Transform trf) where T : Component
	{
		List<T> list = null;
		T t = trf.GetComponent<T>();
		if (t != null)
		{
			if (list == null)
			{
				list = new List<T>();
			}
			list.Add(t);
		}
		for (int i = 0; i < trf.childCount; i++)
		{
			List<T> childrenList = trf.GetChild(i).GetAllComponentsInChildren<T>();
			if (childrenList == null)
			{
				continue;
			}

			if (list == null)
			{
				list = new List<T>();
			}
			list.AddRange(childrenList);
		}
		return list;
	}
}
