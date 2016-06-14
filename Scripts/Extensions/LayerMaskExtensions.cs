using UnityEngine;

public static class LayerMaskExtensions
{
	public static int MaskToLayer(this LayerMask mask)
	{
		int l = mask.value;
		int c = -1;
		while (l > 0)
		{
			c++;
			l = l >> 1;
		}
		return c;
	}
}