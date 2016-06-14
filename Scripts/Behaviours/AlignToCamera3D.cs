using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class AlignToCamera3D : MonoBehaviour
{
	void LateUpdate()
	{
		this.transform.LookAt(Camera.main.transform.position);
		this.transform.Rotate(Vector3.up, 180f);
	}
}
