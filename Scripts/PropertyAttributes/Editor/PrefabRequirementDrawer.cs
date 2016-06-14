using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(PrefabRequirement))]
public class PrefabRequirementDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);

		property.objectReferenceValue = EditorGUI.ObjectField(position, label, property.objectReferenceValue, typeof(GameObject), false);

		EditorGUI.EndProperty();
	}
}
