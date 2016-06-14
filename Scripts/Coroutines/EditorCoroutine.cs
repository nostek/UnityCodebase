using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/*
 * Forked from Benoit FOULETIER
 * https://gist.github.com/benblo/10732554
*/

public class EditorCoroutine
{
	public static EditorCoroutine Start(IEnumerator routine)
	{
		EditorCoroutine coroutine = new EditorCoroutine(routine);
		coroutine.Start();
		return coroutine;
	}

	readonly IEnumerator routine;

	bool running;

	EditorCoroutine(IEnumerator routine)
	{
		this.routine = routine;
	}

	void Start()
	{
		running = true;

		#if UNITY_EDITOR
		EditorApplication.update += Update;
		#endif
	}

	public void Stop()
	{
		running = false;

		#if UNITY_EDITOR
		EditorApplication.update -= Update;
		#endif
	}

	void Update()
	{
		/* 
		 * NOTE: no need to try/catch MoveNext,
		 * if an IEnumerator throws its next iteration returns false.
		 * Also, Unity probably catches when calling EditorApplication.update.
		 */

		if (routine.Current != null && routine.Current is CustomYieldInstruction)
		{
			CustomYieldInstruction customYield = (CustomYieldInstruction)routine.Current;

			if (customYield.keepWaiting)
				return;
		}

		if (!routine.MoveNext())
		{
			Stop();
		}
	}

	public bool Running
	{
		get
		{
			return running;
		}
	}

	public CustomYieldInstruction WaitUntilComplete()
	{
		return new WaitUntil(() => this.running == false);
	}
}
