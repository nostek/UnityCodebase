using UnityEngine;
using System;
using System.Collections.Generic;

public abstract class AssetMapBase : MonoBehaviour
{
	public abstract Type GetMapEnum();

	public abstract Type GetMapType();

	public abstract void SetMap(int key, System.Object obj);

	public abstract System.Object GetMap(int key);
}
