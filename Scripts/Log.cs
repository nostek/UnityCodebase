using System;
using System.Text;
using System.Diagnostics;
using System.Runtime.CompilerServices;

public static class Log
{
	[Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
	public static void D(params object[] vars)
	{
		StringBuilder sb = new StringBuilder();

		for (int i = 0; i < vars.Length; i++)
		{
			sb.Append(vars[i] == null ? "(null)" : vars[i]);
			sb.Append(" ");
		}

		UnityEngine.Debug.Log(sb.ToString());
	}

	[Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
	public static void W(params object[] vars)
	{
		StringBuilder sb = new StringBuilder();

		for (int i = 0; i < vars.Length; i++)
		{
			sb.Append(vars[i] == null ? "(null)" : vars[i]);
			sb.Append(" ");
		}

		UnityEngine.Debug.LogWarning(sb.ToString());
	}

	[Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
	public static void E(params object[] vars)
	{
		StringBuilder sb = new StringBuilder();

		for (int i = 0; i < vars.Length; i++)
		{
			sb.Append(vars[i] == null ? "(null)" : vars[i]);
			sb.Append(" ");
		}

		UnityEngine.Debug.LogError(sb.ToString());
	}

	[Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static void F(params object[] vars)
	{
		StackTrace st = new StackTrace();
		StackFrame sf = st.GetFrame(1);

		StringBuilder sb = new StringBuilder();
		sb.Append(sf.GetMethod().DeclaringType);
		sb.Append(".");
		sb.Append(sf.GetMethod().Name);
		sb.Append("(");

		for (int i = 0; i < vars.Length; i++)
		{
			sb.Append(vars[i] == null ? "(null)" : vars[i]);

			if (i < vars.Length - 1)
				sb.Append(", ");
		}
		sb.Append(")");

		UnityEngine.Debug.Log(sb.ToString());
	}
}
