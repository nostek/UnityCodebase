using System;
using System.Collections.Generic;
using System.Reflection;

public class DependencyManager
{
	static DependencyManager instance;

	Dictionary<Type, Object> binds;
	Dictionary<Type, Object> session;

	private DependencyManager()
	{
		instance = this;

		binds = new Dictionary<Type, Object>();
		session = new Dictionary<Type, Object>();
	}

	public void ResetSession()
	{
		session = new Dictionary<Type, Object>();
	}

	public T Get<T>()
	{
		Object dic = null;

		if (binds.TryGetValue(typeof(T), out dic))
		{
			return (T)dic;
		}

		if (session.TryGetValue(typeof(T), out dic))
		{
			return (T)dic;
		}

		UnityEngine.Debug.LogWarning("Could not find type: " + typeof(T));

		return default(T);
	}

	public void Bind<T>(T obj)
	{
		if (obj == null)
			UnityEngine.Debug.LogError("Trying to bind null: " + typeof(T));
		
		binds.Add(obj.GetType(), obj);
	}

	public void BindSession<T>(T obj)
	{
		if (obj == null)
			UnityEngine.Debug.LogError("Trying to bind null: " + typeof(T));

		session.Add(obj.GetType(), obj);
	}

	public void BindComplete()
	{
		foreach (KeyValuePair<Type, Object> kvp in binds)
			if (kvp.Value is IInjectable)
				((IInjectable)kvp.Value).OnBindComplete();

		foreach (KeyValuePair<Type, Object> kvp in session)
			if (kvp.Value is IInjectable)
				((IInjectable)kvp.Value).OnBindComplete();
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
				else if (session.TryGetValue(info.FieldType, out dic))
				{
					info.SetValue(obj, dic);
				}
				else
				{
					UnityEngine.Debug.LogWarning("Could not bind type: " + info.FieldType);
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
