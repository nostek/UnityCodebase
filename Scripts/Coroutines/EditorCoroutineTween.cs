using System;
using System.Collections;
using UnityEngine;

//using DG.Tweening;

public class EditorCoroutineTween
{
	/*
	public static EditorCoroutine Run(Tween t)
	{
		EditorCoroutineTween ct = new EditorCoroutineTween();

		return EditorCoroutine.Start(ct.UpdateTween(t));
	}

	IEnumerator UpdateTween(Tween t)
	{
		#if UNITY_EDITOR
		float time = Time.realtimeSinceStartup;
		while (!t.IsComplete())
		{
			t.fullPosition = Time.realtimeSinceStartup - time;	
			yield return 0;
		}
		#else
		yield return null;
		#endif
	}
	*/
}
