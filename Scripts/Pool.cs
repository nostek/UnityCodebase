using System;
using System.Collections.Generic;

public class Pool<T>
{
	//TODO: Array later with resize.
	List<T> elements;

	Func<T> onCreate;
	Action<T> onPush;
	Action<T> onPop;

	public Pool(Func<T> onCreate, Action<T> onPush, Action<T> onPop)
	{
		elements = new List<T>();

		this.onCreate = onCreate;
		this.onPush = onPush;
		this.onPop = onPop;
	}

	public T Pop()
	{
		T ret;

		int count = elements.Count;

		if (count == 0)
		{
			ret = onCreate();
		}
		else
		{
			ret = elements[count - 1];	

			elements.RemoveAt(count - 1);
		}

		if (onPop != null)
			onPop(ret);

		return ret;
	}

	public void Push(T item)
	{
		if (elements.IndexOf(item) < 0)
		{
			if (onPush != null)
				onPush(item);
			
			elements.Add(item);
		}
	}

	public List<T> Elements
	{
		get
		{
			return elements;
		}
	}
}
