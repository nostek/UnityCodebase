using UnityEngine;
using System;

[AttributeUsage(AttributeTargets.Field)]
public class EnumFlags : PropertyAttribute
{
	public EnumFlags()
	{
	}
}
