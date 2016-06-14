using UnityEngine;
using System;
using System.Threading;

public class WatchDog : MonoBehaviour
{
	#if UNITY_EDITOR
	
	volatile bool destroyed = false;
	volatile bool focused = true;
	volatile bool paused = false;

	volatile float lastSeen = 0f;

	Thread main;
	Thread thread;

	long firstSeconds = 0;

	void Awake()
	{
		firstSeconds = DateTime.Now.Ticks / TimeSpan.TicksPerSecond;

		lastSeen = (DateTime.Now.Ticks / TimeSpan.TicksPerSecond) - firstSeconds;

		main = Thread.CurrentThread;

		thread = new Thread(ThreadUpdate);
		thread.Start();
	}

	void OnDestroy()
	{
		lastSeen = 0f;
		destroyed = true;
	}

	void OnApplicationFocus(bool focusStatus)
	{
		lastSeen = 0f;
		focused = focusStatus;
	}

	void OnApplicationPause(bool pauseStatus)
	{
		lastSeen = 0f;
		paused = pauseStatus;
	}

	void Update()
	{
		float sec = (float)((DateTime.Now.Ticks / TimeSpan.TicksPerSecond) - firstSeconds);

		lastSeen = sec;

		if (!thread.IsAlive)
		{
			Debug.Break();
		}
	}

	void ThreadUpdate()
	{
		UnityEngine.Debug.Log("[Watchdog] Ready");

		long first = DateTime.Now.Ticks / TimeSpan.TicksPerSecond;

		while (!destroyed)
		{
			Thread.Sleep(1000);

			if (paused)
				continue;

			if (!focused)
				continue;

			float sec = (float)((DateTime.Now.Ticks / TimeSpan.TicksPerSecond) - first);

			if (lastSeen != 0f && sec > lastSeen + 3f)
			{
				UnityEngine.Debug.Log("[Watchdog] Alert");

				main.Abort();

				Thread.ResetAbort();
			}
		}
	}

	#endif
}
