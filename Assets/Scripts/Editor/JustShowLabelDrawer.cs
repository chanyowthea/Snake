//using UnityEngine;
//using System.Collections;
//using UnityEditor;

//[CustomPropertyDrawer(typeof(JustShowLabelAttribute))] 
//public class JustShowLabelDrawer : PropertyDrawer
//{
//	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//	{
//		Rect rect = position; 
//		position = rect; 
//		EditorGUI.LabelField(rect, label); 
//		rect.x += EditorGUIUtility.labelWidth; 
//		EditorGUI.SelectableLabel(rect, GetDisplayValue(property)); 
//	}

//	string GetDisplayValue(SerializedProperty property)
//	{
////		Debug.LogError("type=" + property.type); 
//		switch(property.type)
//		{
//			case "int": 
//				return property.intValue.ToString(); 
//			case "long": 
//				return property.longValue.ToString(); 
//			case "string": 
//				return property.stringValue.ToString(); 
//			case "bool": 
//				return property.boolValue.ToString(); 
//			default:
//				{
//					if (property.type.Contains("PPtr"))
//					{
//						string s = property.objectReferenceValue == null ? "null" : property.objectReferenceValue.ToString();
////						Debug.LogError(s);
//						return s; 
//					}
//					return "null"; 
//				}
//		}
//	}
//}