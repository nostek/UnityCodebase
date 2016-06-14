using System;
using UnityEngine;

public static class Vector3Extensions
{
	public static Vector3 WithX(this Vector3 vec3, float x)
	{
		return new Vector3(x, vec3.y, vec3.z);
	}

	public static Vector3 WithY(this Vector3 vec3, float y)
	{
		return new Vector3(vec3.x, y, vec3.z);
	}

	public static Vector3 WithZ(this Vector3 vec3, float z)
	{
		return new Vector3(vec3.x, vec3.y, z);
	}

	public static Vector3 UpdateX(this Vector3 vec3, float x)
	{
		vec3.x = x;
		return vec3;
	}

	public static Vector3 UpdateY(this Vector3 vec3, float y)
	{
		vec3.y = y;
		return vec3;
	}

	public static Vector3 UpdateZ(this Vector3 vec3, float z)
	{
		vec3.z = z;
		return vec3;
	}

	public static Vector2 AsVector2XY(this Vector3 vec3)
	{
		return new Vector2(vec3.x, vec3.y);
	}

	public static Vector2 AsVector2XZ(this Vector3 vec3)
	{
		return new Vector2(vec3.x, vec3.z);
	}
}
