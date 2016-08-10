using System;
using System.Collections;

public static class EnumeratorExtensions
{
	public static void RunNow(this IEnumerator enumerator)
	{
		while (enumerator.MoveNext())
		{
			
		}
	}
}
