using UnityEngine;
using System;
using System.Collections.Generic;

public class MonoDependencyInstaller : MonoBehaviour
{
	List<object> dependencies = null;

	public MonoDependencyInstaller Create()
	{
		if (dependencies == null)
			dependencies = new List<object>();

		return this;
	}

	public MonoDependencyInstaller RegisterDependency<T>(T obj) where T : class
	{
		DependencyManager.Instance.Bind<T>(obj);

		dependencies.Add(obj);

		return this;
	}

	public void Build()
	{
		if (dependencies != null)
			for (int i = 0; i < dependencies.Count; i++)
				if (dependencies[i] is IInjectable)
					((IInjectable)dependencies[i]).OnBindComplete();
	}

	protected virtual void OnDestroy()
	{
		if (dependencies != null)
			for (int i = 0; i < dependencies.Count; i++)
				DependencyManager.Instance.Unbind(dependencies[i].GetType());
	}
}
