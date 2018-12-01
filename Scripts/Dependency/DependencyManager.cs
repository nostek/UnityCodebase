using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

public class DependencyManager
{
	static DependencyManager instance;

	Dictionary<Type, Object> binds;

	private DependencyManager()
	{
		instance = this;

		binds = new Dictionary<Type, Object>();
	}

	public T Get<T>() where T : class
	{
		Object dic = null;

		if (binds.TryGetValue(typeof(T), out dic))
		{
			return (T)dic;
		}

		UnityEngine.Debug.LogWarning("Could not find type: " + typeof(T));

		return default(T);
	}

	public DependencyManager Bind<T>(T obj) where T : class
	{
		if (obj == null)
			UnityEngine.Debug.LogError("Trying to bind null: " + typeof(T));

		binds.Add(typeof(T), obj);

		return this;
	}

	public DependencyManager Unbind<T>() where T : class
	{
		binds.Remove(typeof(T));
		return this;
	}

	public DependencyManager Unbind(Type t)
	{
		binds.Remove(t);
		return this;
	}

	public void Apply(Object obj)
	{
		FieldInfo[] fields = obj.GetType().GetFields(
								 BindingFlags.FlattenHierarchy |
								 BindingFlags.Instance |
								 BindingFlags.Public |
								 BindingFlags.NonPublic |
								 BindingFlags.Static
							 );

		foreach (FieldInfo info in fields)
		{
			if (info.IsDefined(typeof(Inject), true))
			{
				Object dic = null;
				if (binds.TryGetValue(info.FieldType, out dic))
				{
					info.SetValue(obj, dic);
				}
				else
				{
					UnityEngine.Debug.LogWarning("Could not inject type: " + info.FieldType);
				}
			}
		}
	}

	public static DependencyManager Instance
	{
		get
		{
			if (instance == null)
				instance = new DependencyManager();

			return instance;
		}
	}
}
