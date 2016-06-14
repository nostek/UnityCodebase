using UnityEngine;
using System;
using DG.Tweening;

[Serializable]
public class TweenSettings
{
	[SerializeField]
	float delay = 0f;

	[SerializeField]
	float duration = 0f;

	[SerializeField]
	Ease ease = Ease.Unset;

	[SerializeField]
	int loops = 1;

	[SerializeField]
	LoopType loopType = LoopType.Restart;

	public float Delay
	{
		get
		{
			return delay;
		}
	}

	public float Duration
	{
		get
		{
			return duration;
		}
	}

	public Ease Ease
	{
		get
		{
			return ease;
		}
	}

	public int Loops
	{
		get
		{
			return loops;
		}
	}

	public float Length
	{
		get
		{
			return delay + (duration * loops);
		}
	}

	public Tweener Apply(Tweener tween)
	{
		if (delay > 0f)
			tween.SetDelay(delay);

		if (ease != Ease.Unset)
			tween.SetEase(ease);

		if (loops > 1)
			tween.SetLoops(loops, loopType);

		return tween;
	}
}
