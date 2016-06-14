using System;
using UnityEngine;

public class WaitForRealtimeSeconds : CustomYieldInstruction
{
	float time;

	public WaitForRealtimeSeconds(float seconds)
	{
		time = Time.realtimeSinceStartup + seconds;
	}

	#region implemented abstract members of CustomYieldInstruction

	public override bool keepWaiting
	{
		get
		{
			return (Time.realtimeSinceStartup < time);
		}
	}

	#endregion
}
