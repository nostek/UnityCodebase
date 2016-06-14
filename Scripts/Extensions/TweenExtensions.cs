using System;
using DG.Tweening;

public static class TweenExtensions
{
	public static Tweener ApplySettings(this Tweener tween, TweenSettings settings)
	{
		return settings.Apply(tween);
	}
}
