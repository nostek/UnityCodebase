using System;

public static class ObjectExtension
{
	public static void Inject(this System.Object obj)
	{
		DependencyManager.Instance.Apply(obj);
	}
}
