//using UnityEngine;
//using System.Collections;
//using UnityEditor;

//// The property drawer class should be placed in an editor script, inside a folder called Editor.
//// Tell the RangeDrawer that it is a drawer for properties with the RangeAttribute.
//[CustomPropertyDrawer(typeof(ArrayAttribute))]
//public class ArrayDrawer : PropertyDrawer
//{
//	public const float widthBt = 18;

//	public static void AddArrayTools(Rect position, SerializedProperty property)
//	{
//		string path = property.propertyPath;
//		int arrayInd = path.LastIndexOf(".Array");
//		bool bIsArray = arrayInd >= 0;

//		if (bIsArray)
//		{
//			SerializedObject so = property.serializedObject;
//			string arrayPath = path.Substring(0, arrayInd);
//			SerializedProperty arrayProp = so.FindProperty(arrayPath);

//			//Next we need to grab the index from the path string
//			int indStart = path.IndexOf("[") + 1;
//			int indEnd = path.IndexOf("]");

//			string indString = path.Substring(indStart, indEnd - indStart);

//			int myIndex = int.Parse(indString);
//			Rect rcButton = position;
//			rcButton.height = EditorGUIUtility.singleLineHeight;
//			rcButton.x = position.xMax - widthBt * 4;
//			rcButton.width = widthBt;

//			bool lastEnabled = GUI.enabled;

//			if (myIndex == 0)
//				GUI.enabled = false;

//			if (GUI.Button(rcButton, new GUIContent("↑", "move up")))
//			{
//				arrayProp.MoveArrayElement(myIndex, myIndex - 1);
//				so.ApplyModifiedProperties();

//			}

//			rcButton.x += widthBt;
//			GUI.enabled = lastEnabled;
//			if (myIndex >= arrayProp.arraySize - 1)
//				GUI.enabled = false;

//			if (GUI.Button(rcButton, new GUIContent("↓", "move down"))) // \u21b4
//			{
//				arrayProp.MoveArrayElement(myIndex, myIndex + 1);
//				so.ApplyModifiedProperties();
//			}

//			GUI.enabled = lastEnabled;

//			rcButton.x += widthBt;
//			if (GUI.Button(rcButton, new GUIContent("-", "delete")))
//			{
//				var p = arrayProp.GetArrayElementAtIndex(myIndex);
//				if (p.objectReferenceValue != null)
//				{
//					arrayProp.DeleteArrayElementAtIndex(myIndex);
//				}
//				arrayProp.DeleteArrayElementAtIndex(myIndex);
//				so.ApplyModifiedProperties();
//			}

//			rcButton.x += widthBt;
//			if (GUI.Button(rcButton, new GUIContent("+", "add")))
//			{
//				var p = arrayProp.GetArrayElementAtIndex(myIndex);
//				arrayProp.InsertArrayElementAtIndex(myIndex);
//				if (p.objectReferenceValue != null)
//				{
//					arrayProp.DeleteArrayElementAtIndex(myIndex + 1);
//				}
//				so.ApplyModifiedProperties();
//			}
//		}
//	}

//	public static SerializedProperty GetArrayProperty(SerializedProperty property)
//	{
//		string path = property.propertyPath; 
//		int arrayInd = path.LastIndexOf(".Array");
//		bool bIsArray = arrayInd >= 0;
//		if (bIsArray)
//		{
//			SerializedObject so = property.serializedObject;
//			string arrayPath = path.Substring(0, arrayInd);
//			return so.FindProperty(arrayPath);
//		}
//		return null;
//	}

//	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//	{
//		AddArrayTools(position, property);
//		Rect rc = position;
//		if (!property.isExpanded)
//			rc.width -= widthBt * 4;
//		EditorGUI.PropertyField(rc, property, label, true);
//	}

//	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//	{
//		return EditorGUI.GetPropertyHeight(property);
//	}
//}