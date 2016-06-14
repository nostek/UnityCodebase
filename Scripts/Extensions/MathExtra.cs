using System;

public static class MathExtra
{
	public static float Saturate(float value)
	{
		return Math.Min(1f, Math.Max(0f, value));
	}
}
