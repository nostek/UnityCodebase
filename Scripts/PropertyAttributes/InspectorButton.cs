using UnityEngine;

// Made by zaikman -> https://www.reddit.com/r/Unity3D/comments/1s6czv/inspectorbutton_add_a_custom_button_to_your/

/// <summary>
/// This attribute can only be applied to fields because its
/// associated PropertyDrawer only operates on fields (either
/// public or tagged with the [SerializeField] attribute) in
/// the target MonoBehaviour.
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Field)]
public class InspectorButton : PropertyAttribute
{
	public readonly string MethodName;

	public InspectorButton(string MethodName)
	{
		this.MethodName = MethodName;
	}
}
