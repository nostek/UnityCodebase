using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;
using System;

public static class EditorCustomDrawer
{
	public static System.Object Draw(string label, Type clsType, System.Object obj)
	{
		return DrawElement(label, clsType, obj, null, null);
	}

	static System.Object DrawElement(string label, Type clsType, System.Object obj, FieldInfo field, System.Object fieldObject)
	{
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

		if (clsType.IsArray)
		{
			return DrawArray(label, clsType, obj, field, fieldObject);
		}

		if (clsType.IsClass)
		{
			return DrawClass(label, clsType, obj, field, fieldObject);
		}

		Debug.LogWarning("Unsupported type: " + clsType);

		return null;
	}

	static System.Object DrawArray(string label, Type clsType, System.Object obj, FieldInfo field, System.Object fieldObject)
	{
		Event ev = Event.current;

		int objHash = obj.GetHashCode();

		Rect rectArray = EditorGUILayout.BeginVertical();

		bool show = false;

		if (obj != null && ((Array)obj).Length > 0)
			show = SetFoldout(objHash, EditorGUILayout.Foldout(GetFoldout(objHash), label));
		else
			EditorGUILayout.LabelField(label, "(Empty)");

		if (show && obj != null)
		{
			Array array = (Array)obj;

			EditorGUI.indentLevel++;

			for (int i = 0; i < array.Length; i++)
			{
				Rect rectArrayElement = EditorGUILayout.BeginVertical();

				array.SetValue(
					DrawElement(i.ToString(), clsType.GetElementType(), array.GetValue(i), null, null),
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

	static System.Object DrawClass(string label, Type clsType, System.Object obj, FieldInfo field, System.Object fieldObject)
	{
		int objHash = obj.GetHashCode();

		bool show = SetFoldout(objHash, EditorGUILayout.Foldout(GetFoldout(objHash), label));

		if (!show)
			return obj;

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

	#region FOLDOUT

	static Dictionary<int, bool> foldout = new Dictionary<int, bool>();

	static bool GetFoldout(int hash)
	{
		if (!foldout.ContainsKey(hash))
			foldout.Add(hash, true);

		return foldout[hash];
	}

	static bool SetFoldout(int hash, bool value)
	{
		foldout[hash] = value;

		return value;
	}

	#endregion
}
