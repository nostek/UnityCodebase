using UnityEngine;
using System;
using System.Text;

public static class Log
{
	public static void D(params object[] vars)
	{
		StringBuilder sb = new StringBuilder();

		for (int i = 0; i < vars.Length; i++)
		{
			sb.Append(vars[i] == null ? "(null)" : vars[i]);
			sb.Append(" ");
		}

		Debug.Log(sb.ToString());
	}

	public static void W(params object[] vars)
	{
		StringBuilder sb = new StringBuilder();

		for (int i = 0; i < vars.Length; i++)
		{
			sb.Append(vars[i] == null ? "(null)" : vars[i]);
			sb.Append(" ");
		}

		Debug.LogWarning(sb.ToString());
	}
}

public static class LogExtensions
{
	public static void Trace(this System.Object obj, params object[] vars)
	{
		StringBuilder sb = new StringBuilder();

		for (int i = 0; i < vars.Length; i++)
		{
			sb.Append(vars[i] == null ? "(null)" : vars[i]);
			sb.Append(" ");
		}

		Debug.Log(sb.ToString());
	}

	public static void Trace(this UnityEngine.Object mono, params object[] vars)
	{
		StringBuilder sb = new StringBuilder();

		for (int i = 0; i < vars.Length; i++)
		{
			sb.Append(vars[i] == null ? "(null)" : vars[i]);
			sb.Append(" ");
		}

		Debug.Log(sb.ToString(), mono);
	}
}