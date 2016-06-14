using UnityEngine;
using System.Collections;

public class AnimationDelayStart : MonoBehaviour
{
	IEnumerator Start()
	{
		yield return new WaitForSeconds(1f);

		this.GetComponent<Animation>().Play();
	}
}
