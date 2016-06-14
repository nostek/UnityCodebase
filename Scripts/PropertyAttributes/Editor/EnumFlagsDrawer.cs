using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EnumFlags))]
public class EnumFlagsDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);

		property.intValue = EditorGUI.MaskField(position, label, property.intValue, property.enumNames);

		EditorGUI.EndProperty();
	}
}
