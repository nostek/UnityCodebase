using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;

[CustomEditor(typeof(AssetMapBase), true)]
public class AssetMapBaseEditor : Editor
{
	Type enumType;
	Type classType;
	string[] keys;
	Array values;

	void OnEnable()
	{
		if (!(target is AssetMapBase))
			return;
		
		AssetMapBase map = (AssetMapBase)target;

		if (map == null)
			return;

		enumType = map.GetMapEnum();
		classType = map.GetMapType();

		keys = Enum.GetNames(enumType);
		values = Enum.GetValues(enumType);
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		DrawDefaultInspector();

		AssetMapBase map = (AssetMapBase)target;

		int fromId = int.MinValue;
		int toId = int.MaxValue;

		Type mapType = map.GetType();

		foreach (var attr in mapType.GetCustomAttributes(true))
		{
			if (attr is AssetMapRange)
			{
				fromId = ((AssetMapRange)attr).from;
				toId = ((AssetMapRange)attr).to;
			}
		}

		EditorGUILayout.Space();

		EditorGUI.BeginChangeCheck();

		for (int i = 0; i < keys.Length; i++)
		{
			int enumId = (int)values.GetValue(i);

			if (!(enumId >= fromId && enumId <= toId))
				continue;

			map.SetMap(enumId, EditorUtils.DrawElement(keys[i], classType, map.GetMap(enumId)));
		}

		bool changed = EditorGUI.EndChangeCheck();

		serializedObject.ApplyModifiedProperties();

		if (changed)
		{
			EditorUtility.SetDirty(map);

			EditorSceneManager.MarkAllScenesDirty();
		}
	}
}
