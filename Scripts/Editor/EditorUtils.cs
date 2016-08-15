using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.IO;

public static class EditorUtils
{
	static public void ClearDeveloperConsole()
	{
		var logEntries = System.Type.GetType("UnityEditorInternal.LogEntries, UnityEditor.dll");
		var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
		clearMethod.Invoke(null, null);
	}

	[MenuItem("Tools/Clear Empty Folders")]
	static void ClearEmptyFolders()
	{
		ClearDeveloperConsole();

		Debug.Log("Checking for empty folders...");

		ClearEmptyFoldersRec(UnityEngine.Application.dataPath);

		AssetDatabase.Refresh();
	}

	static bool ClearEmptyFoldersRec(string path)
	{
		string[] folders = Directory.GetDirectories(path);
		string[] files = Directory.GetFiles(path);

		bool hasFolders = (folders != null && folders.Length > 0);
		bool hasFiles = (files != null && files.Length > 0);

		if (!hasFolders && hasFiles && files.Length == 1 && files[0].ToLower().EndsWith(".ds_store"))
			hasFiles = false;

		if (!hasFolders && !hasFiles)
		{
			foreach (var f in files)
				File.Delete(f);
			
			return true;
		}

		if (hasFolders)
			for (int i = 0; i < folders.Length; i++)
			{
				if (ClearEmptyFoldersRec(folders[i]))
				{
					Debug.Log("Delete empty folder: " + folders[i]);

					Directory.Delete(folders[i]);
				}
			}

		return false;
	}

	public static System.Object DrawElement(string label, Type clsType, System.Object obj, FieldInfo field = null, System.Object fieldObject = null)
	{
		Event ev = Event.current;

		//Nicify
		label = ObjectNames.NicifyVariableName(label);

		//Attributes
		if (field != null)
		{
			//Space
			for (int i = 0; i < field.GetNumCustomAttributes<SpaceAttribute>(); i++)
				EditorGUILayout.Space();				

			//Header
			HeaderAttribute header = field.GetFirstCustomAttribute<HeaderAttribute>();
			if (header != null)
				EditorGUILayout.LabelField(header.header, EditorStyles.boldLabel);
		}

		if (clsType.BaseType == typeof(UnityEngine.Object))
		{
			PrefabRequirement prefabReq = field != null ? field.GetFirstCustomAttribute<PrefabRequirement>() : null;

			return EditorGUILayout.ObjectField(label, (UnityEngine.Object)obj, clsType, (prefabReq == null));
		}

		if (clsType == typeof(int))
		{
			RangeAttribute rangeAttr = field != null ? field.GetFirstCustomAttribute<RangeAttribute>() : null;

			if (rangeAttr != null)
				return EditorGUILayout.IntSlider(label, (int)obj, (int)rangeAttr.min, (int)rangeAttr.max);

			return EditorGUILayout.IntField(label, (int)obj);
		}

		if (clsType == typeof(float))
		{
			RangeAttribute rangeAttr = field != null ? field.GetFirstCustomAttribute<RangeAttribute>() : null;

			if (rangeAttr != null)
				return EditorGUILayout.Slider(label, (float)obj, rangeAttr.min, rangeAttr.max);

			return EditorGUILayout.FloatField(label, (float)obj);
		}

		if (clsType == typeof(double))
		{
			RangeAttribute rangeAttr = field != null ? field.GetFirstCustomAttribute<RangeAttribute>() : null;

			if (rangeAttr != null)
				return (double)EditorGUILayout.Slider(label, (float)((double)obj), rangeAttr.min, rangeAttr.max);

			return EditorGUILayout.DoubleField(label, (double)obj);
		}

		if (clsType == typeof(bool))
		{
			return EditorGUILayout.Toggle(label, (bool)obj);
		}

		if (clsType == typeof(string))
		{
			return EditorGUILayout.TextField(label, (string)obj);
		}

		if (clsType.IsEnum)
		{
			return EditorGUILayout.EnumPopup(label, (Enum)obj);
		}

		if (clsType == typeof(LayerMask))
		{
			int[] values = LayersExtra.AllLayerValues();
			string[] names = LayersExtra.AllLayerNames();

			LayerMask mask = (LayerMask)obj;

			//Find the selected index from values.
			int index = 0;
			for (int i = 0; i < values.Length; i++)
				if (values[i] == (int)mask)
				{
					index = i;
					break;
				}
			
			int selected = EditorGUILayout.Popup(label, index, names);

			return (LayerMask)values[selected];
		}

		if (clsType.IsArray)
		{
			Rect rectArray = EditorGUILayout.BeginVertical();

			if (obj != null && ((Array)obj).Length > 0)
				EditorGUILayout.LabelField(label);
			else
				EditorGUILayout.LabelField(label, "(Empty)");

			if (obj != null)
			{
				Array array = (Array)obj;

				EditorGUI.indentLevel++;

				for (int i = 0; i < array.Length; i++)
				{
					Rect rectArrayElement = EditorGUILayout.BeginVertical();

					array.SetValue(
						DrawElement(i.ToString(), clsType.GetElementType(), array.GetValue(i), null),
						i
					);

					EditorGUILayout.EndVertical();

					if (ev.type == EventType.ContextClick && rectArrayElement.Contains(ev.mousePosition))
					{
						GenericMenu menu = new GenericMenu();
						menu.AddItem(new GUIContent("Remove Item"), false, (removeIndex) =>
							{
								Array oldArray = (Array)obj;
								Array newArray = Array.CreateInstance(clsType.GetElementType(), oldArray.Length - 1);

								int addIndex = 0;
								for (int y = 0; y < oldArray.Length; y++)
									if (y != (int)removeIndex)
										newArray.SetValue(oldArray.GetValue(y), addIndex++);

								field.SetValue(fieldObject, newArray);
							}, i);
						menu.ShowAsContext();

						ev.Use();
					}
				}

				EditorGUI.indentLevel--;
			}

			EditorGUILayout.EndVertical();

			if (ev.type == EventType.ContextClick && rectArray.Contains(ev.mousePosition))
			{
				GenericMenu menu = new GenericMenu();
				menu.AddItem(new GUIContent("Add Item"), false, () =>
					{
						Array oldArray = (Array)obj;
						Array newArray = Array.CreateInstance(clsType.GetElementType(), oldArray.Length + 1);

						oldArray.CopyTo(newArray, 0);

						System.Object newItem = Activator.CreateInstance(clsType.GetElementType());

						newArray.SetValue(newItem, newArray.Length - 1);

						field.SetValue(fieldObject, newArray);
					});
				menu.ShowAsContext();

				ev.Use();
			}

			if (ev.type == EventType.DragUpdated || ev.type == EventType.DragPerform)
			{
				if (rectArray.Contains(ev.mousePosition))
				{
					DragAndDrop.visualMode = DragAndDropVisualMode.Link;

					if (ev.type == EventType.DragPerform)
					{
						DragAndDrop.AcceptDrag();

						int len = DragAndDrop.objectReferences.Length;

						Array oldArray = (Array)obj;
						Array newArray = Array.CreateInstance(clsType.GetElementType(), oldArray.Length + len);

						oldArray.CopyTo(newArray, 0);

						for (int i = 0; i < len; i++)
							newArray.SetValue(DragAndDrop.objectReferences[i], oldArray.Length + i);

						field.SetValue(fieldObject, newArray);
					}
				}
			}

			return obj;
		}

		if (clsType.IsClass)
		{
			EditorGUILayout.LabelField(label);

			EditorGUI.indentLevel++;

			FieldInfo[] fields = clsType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.Instance);

			for (int i = 0; i < fields.Length; i++)
			{
				if (fields[i].IsNotSerialized)
					continue;

				System.Object objGet = fields[i].GetValue(obj);

				if (fields[i].IsInitOnly)
				{
					EditorGUILayout.LabelField(ObjectNames.NicifyVariableName(fields[i].Name), objGet.ToString());
				}
				else
				{
					SerializeField serializeField = fields[i].GetFirstCustomAttribute<SerializeField>();
					HideInInspector hideInInspector = fields[i].GetFirstCustomAttribute<HideInInspector>();

					if (hideInInspector != null)
						continue;

					if (fields[i].IsPrivate && serializeField == null)
						continue;

					if (fields[i].FieldType.IsArray && objGet == null)
						objGet = Array.CreateInstance(fields[i].FieldType.GetElementType(), 0);

					if (fields[i].FieldType.IsClass &&
					    fields[i].FieldType.IsSerializable &&
					    objGet == null)
					{
						if (fields[i].FieldType == typeof(string))
							objGet = "";
						else
							objGet = Activator.CreateInstance(fields[i].FieldType, true);
					}

					System.Object objSet = DrawElement(fields[i].Name, fields[i].FieldType, objGet, fields[i], obj);

					if (objGet != objSet)
						fields[i].SetValue(obj, objSet);
				}
			}

			EditorGUI.indentLevel--;

			return obj;
		}

		Log.W("Unsupported type:", clsType);

		return null;
	}
}
