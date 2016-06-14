using System;
using UnityEngine;

public static class TransformExtensions
{
	public static void DeleteAllChildren(this Transform transform, bool detach = false)
	{
		for (int i = transform.childCount - 1; i >= 0; i--)
			transform.GetChild(i).gameObject.DeleteEditorSafe(detach);
	}
}
