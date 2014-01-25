using UnityEngine;
using System.Collections;

public static class ParentSearches {

	public static Transform FindParentWithTag(Transform child, string tag)
	{
		if(child.CompareTag(tag))
		{
			return child;
		}
		Transform parent = child.parent;
		if(parent != null)
		{
			return FindParentWithTag(parent, tag);
		}
		return null;
	}
	
	/// <summary>
	/// Gets the component of type T in the object or in its parents.
	/// </summary>
	public static T GetComponentInParents<T>(Transform child) where T : Component
	{
		T component = child.gameObject.GetComponent<T>();
		if(component != null)
		{
			return component;
		}
		Transform parent = child.parent;
		if(parent != null)
		{
			return GetComponentInParents<T>(parent);
		}
		return default(T);
	}
	
	public static T GetComponentInParents<T>(GameObject child) where T : Component
	{
		return GetComponentInParents<T>(child.transform);
	}
}
