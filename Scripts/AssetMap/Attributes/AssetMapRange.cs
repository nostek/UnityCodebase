using System;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class AssetMapRange : Attribute
{
	public int from = 0;
	public int to = 0;

	public AssetMapRange(int from, int to = -1, bool exclude = true)
	{
		this.from = from;
		this.to = (to == -1) ? int.MaxValue : to;

		if (exclude)
		{
			this.from++;
			this.to--;
		}
	}
}
