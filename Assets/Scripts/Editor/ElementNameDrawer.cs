//using UnityEngine;
//using System.Collections;
//using UnityEditor;

//[CustomPropertyDrawer(typeof(ElementNameAttribute))] // ItemConf
//public class ElementNameDrawer : PropertyDrawer
//{
//	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//	{
//		ItemConf data = property.objectReferenceValue as ItemConf;
//		if (data != null)
//		{
//			label = new GUIContent(data._id + "_" + (data._prefab == null ? "" : data._prefab.name));
//		}

//		ArrayDrawer.AddArrayTools(position, property);
//		Rect rc = position;
//		if (!property.isExpanded)
//		{
//			rc.width -= ArrayDrawer.widthBt * 4;
//		}
//		EditorGUI.PropertyField(rc, property, label, true);
//	}
//}

//[CustomPropertyDrawer(typeof(ViewConfAttribute))] // ViewConf
//public class ViewConfDrawer : PropertyDrawer
//{
//	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//	{
//		//ViewConf data = property.objectReferenceValue as ViewConf;
//		//if (data != null)
//		//{
//		//	label = new GUIContent(data._viewType + "_" + (data._prefab == null ? "" : data._prefab.name));
//		//}
//		//ArrayDrawer.AddArrayTools(position, property);
//		//Rect rc = position;
//		//if (!property.isExpanded)
//		//{
//		//	rc.width -= ArrayDrawer.widthBt * 4;
//		//}
//		//EditorGUI.PropertyField(rc, property, label, true);
//	}
//}