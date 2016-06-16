using UnityEngine;

public static class LayersExtra
{
	public static string[] AllLayerNames()
	{
		int count = 0;
		int index = 0;

		for (int i = 0; i < 32; i++)
			if (!string.IsNullOrEmpty(LayerMask.LayerToName(i)))
				count++;

		string[] names = new string[count];

		for (int i = 0; i < 32; i++)
			if (!string.IsNullOrEmpty(LayerMask.LayerToName(i)))
				names[index++] = LayerMask.LayerToName(i);

		return names;
	}

	public static int[] AllLayerValues()
	{
		int count = 0;
		int index = 0;

		for (int i = 0; i < 32; i++)
			if (!string.IsNullOrEmpty(LayerMask.LayerToName(i)))
				count++;

		int[] values = new int[count];

		for (int i = 0; i < 32; i++)
			if (!string.IsNullOrEmpty(LayerMask.LayerToName(i)))
				values[index++] = 1 << i;

		return values;
	}
}
