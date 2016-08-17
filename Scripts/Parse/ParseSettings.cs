using UnityEngine;
using System;

public class ParseSettings : ScriptableObject
{
	[Multiline]
	public string AppId = "null";
	[Multiline]
	public string RESTKey = "null";
	[Multiline]
	public string Host = "null";
}
