using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImageAlphaSetter : MonoBehaviour
{
	public float Alpha;

	Image image;

	float alphaSet;

	void Awake()
	{
		image = GetComponent<Image>();

		Alpha = alphaSet = image.color.a;
	}

	void Update()
	{
		if (Alpha != alphaSet)
		{
			alphaSet = Alpha;

			Color c = image.color;
			c.a = Alpha;
			image.color = c;
		}
	}
}
