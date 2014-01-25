using UnityEngine;
using System.Collections;

public class BulletMovement : MonoBehaviour {
	
	public float BulletVelocity;
	
	// Use this for initialization
	void Start () {
		//unless this is some kind of smart bullet, the bullet should just move forward
		rigidbody.velocity = transform.forward * BulletVelocity;
	}
}