using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public abstract class AssetMap<T_Enum, T_Class> : AssetMapBase
	where T_Enum : IConvertible
	where T_Class : new()
{
	[SerializeField]
	[HideInInspector]
	List<int> keys = new List<int>();

	[SerializeField]
	[HideInInspector]
	List<T_Class> values = new List<T_Class>();

	int GetKeyIndex(int key)
	{
		for (int i = 0; i < keys.Count; i++)
			if (keys[i] == key)
				return i;

		return -1;		
	}

	System.Object GetDefault()
	{
		return default(T_Class);
	}

	System.Object NewDefault()
	{
		return new T_Class();
	}

	public override Type GetMapEnum()
	{
		return typeof(T_Enum);
	}

	public override Type GetMapType()
	{
		return typeof(T_Class);
	}

	public List<KeyValuePair<T_Enum, T_Class>> KeyValues()
	{
		List<KeyValuePair<T_Enum, T_Class>> ret = new List<KeyValuePair<T_Enum, T_Class>>(keys.Count);

		for (int i = 0; i < keys.Count; i++)
			ret.Add(new KeyValuePair<T_Enum, T_Class>(
					(T_Enum)Enum.ToObject(GetMapEnum(), keys[i]),
					values[i]
				));

		return ret;
	}

	public override System.Object GetMap(int key)
	{
		int index = GetKeyIndex(key);

		if (index >= 0)
			return values[index];

		if (GetMapType().IsClass && GetMapType().BaseType != typeof(UnityEngine.Object))
		{
			return NewDefault();
		}

		return GetDefault();
	}

	public override void SetMap(int key, System.Object obj)
	{
		int index = GetKeyIndex(key);

		if (index >= 0)
		{
			if (values[index] != null && values[index].Equals(obj))
				return;
			
			values[index] = (T_Class)obj;

			return;
		}

		if (obj == GetDefault())
			return;

		keys.Add(key);
		values.Add((T_Class)obj);
	}

	public T_Class GetByType(T_Enum key)
	{
		int index = (int)Enum.ToObject(GetMapEnum(), key);

		if (index >= 0)
			return (T_Class)GetMap(index);

		return (T_Class)GetDefault();
	}
}
