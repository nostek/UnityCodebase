using UnityEngine;
using System;

public static class RectTransformExtensions
{
	public static void ScaleHorizontalTo(this RectTransform transform, Vector3 to)
	{
		Vector3 dir = to - transform.position;
		dir.Normalize();

		float rot = Mathf.Atan2(dir.y, dir.x);

		transform.localRotation = Quaternion.Euler(0f, 0f, rot * Mathf.Rad2Deg);

		Vector3 localPos = transform.worldToLocalMatrix.MultiplyPoint(to);

		transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, localPos.magnitude);
	}
}
