using UnityEngine;
using UnityEditor;
using System.Reflection;

[CustomPropertyDrawer(typeof(InspectorButton))]
public class InspectorButtonDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
	{
		InspectorButton inspectorButtonAttribute = (InspectorButton)attribute;

		Rect buttonRect = new Rect(position.x + position.width * 0.1f, position.y, position.width * 0.8f, position.height);

		if (GUI.Button(buttonRect, label.text))
		{
			System.Type eventOwnerType = prop.serializedObject.targetObject.GetType();
			string eventName = inspectorButtonAttribute.MethodName;

			MethodInfo eventMethodInfo = eventOwnerType.GetMethod(eventName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

			if (eventMethodInfo != null)
				eventMethodInfo.Invoke(prop.serializedObject.targetObject, null);
			else
				Debug.LogWarning(string.Format("InspectorButton: Unable to find method {0} in {1}", eventName, eventOwnerType));
		}
	}
}
