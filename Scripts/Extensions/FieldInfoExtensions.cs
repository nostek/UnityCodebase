using System;
using System.Reflection;

public static class FieldInfoExtensions
{
	public static T GetFirstCustomAttribute<T>(this FieldInfo info)
	{
		object[] attr = info.GetCustomAttributes(typeof(T), true);

		if (attr.Length > 0)
			return (T)attr[0];

		return default(T);
	}

	public static int GetNumCustomAttributes<T>(this FieldInfo info)
	{
		object[] attr = info.GetCustomAttributes(typeof(T), true);

		return attr.Length;
	}
}
