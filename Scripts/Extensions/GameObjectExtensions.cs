using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class GameObjectExtensions
{
	public static void DeleteEditorSafe(this GameObject go, bool detach = false)
	{
#if UNITY_EDITOR
		if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
		{
			GameObject.Destroy(go);

			if (detach)
				go.transform.SetParent(null);
		}
		else if (UnityEngine.Application.isEditor)
		{
			GameObject.DestroyImmediate(go);
		}
#else
		GameObject.Destroy(go);

		if(detach)
			go.transform.SetParent(null);
#endif
	}
}
