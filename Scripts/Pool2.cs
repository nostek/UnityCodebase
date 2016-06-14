using System;
using System.Collections.Generic;

public abstract class Pool2<T>
{
	//TODO: Array later with resize.
	List<T> elements;

	public Pool2()
	{
		elements = new List<T>();
	}

	public T Pop()
	{
		T ret;

		int count = elements.Count;

		if (count == 0)
		{
			ret = OnCreate();
		}
		else
		{
			ret = elements[count - 1];	

			elements.RemoveAt(count - 1);
		}

		OnPop(ret);

		return ret;
	}

	public void Push(T item)
	{
		if (elements.IndexOf(item) < 0)
		{
			OnPush(item);
			
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

	protected abstract T OnCreate();

	protected abstract void OnPush(T obj);

	protected abstract void OnPop(T obj);
}
