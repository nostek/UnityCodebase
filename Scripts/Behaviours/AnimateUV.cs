using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class AnimateUV : MonoBehaviour
{
	[SerializeField]
	bool animateU = true;

	[SerializeField]
	bool animateV = true;

	[SerializeField]
	float speed = 1;

	new Renderer renderer;

	Vector3 offset;

	void Awake()
	{
		renderer = GetComponent<Renderer>();

		offset = new Vector3();
	}

	void Update()
	{
		offset.x = animateU ? Time.time * speed : 0f;
		offset.y = animateV ? Time.time * speed : 0f;

		renderer.material.SetTextureOffset("_MainTex", offset);
	}
}
