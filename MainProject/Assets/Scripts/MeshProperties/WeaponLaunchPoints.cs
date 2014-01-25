using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// List of weapon launch points on a model. 
/// Apply this script to a mesh asset if there's any actors using the mesh that fires weapons.
/// </summary>
public class WeaponLaunchPoints : MonoBehaviour {
	
	public List<Transform> MissileLaunchPoints;
	public List<Transform> BulletLaunchPoints;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
