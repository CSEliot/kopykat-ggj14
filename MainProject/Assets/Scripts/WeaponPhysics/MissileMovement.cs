using UnityEngine;
using System.Collections;

public class MissileMovement : MonoBehaviour {
	
	public GameObject Target = null;
	public float MaxSpeed;
	public float Thrust;
	public float SeekSpeed;
	
	// Use this for initialization
	void Start () {
		//for now, we'll emulate the Blender build's missile behavior.
		//Lockon will be handled in a player script, and will provide the
		//target for this script.
		
		//missiles in the old build had a constant forward velocity
		//rigidbody.velocity = (transform.forward * Speed);// + (transform.up * 0.5f * Speed);
		rigidbody.constantForce.relativeForce = Vector3.forward * Thrust;
		rigidbody.AddRelativeForce(Vector3.forward * Thrust * 35);
		rigidbody.AddRelativeForce(Vector3.up * Thrust * 4);
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	void FixedUpdate()
	{
		//for now, we'll emulate the Blender build's missile behavior.
		//Lockon will be handled in a player script, and will provide the
		//target for this script.
		
		//if there's a target, seek towards it by adding a force leading
		//towards the target.
		if(Target != null)
		{
			Vector3 seekForce = (Target.transform.position - rigidbody.position) * SeekSpeed;
			//we also want to turn the missile, so we need to get the perpendicular component of the force
			//Vector3 seekTorque = seekForce;//Vector3.Dot(seekForce, Vector3.up) * Vector3.up;
			Quaternion newHeading = Quaternion.LookRotation(seekForce);
			rigidbody.rotation = Quaternion.Slerp(rigidbody.rotation, newHeading, SeekSpeed);
			//rigidbody.velocity = transform.forward * Speed;
			//rigidbody.AddRelativeForce(seekForce);
		}
		//rescale our velocity if the added force exceeded our speed cap
		if(MaxSpeed > 0.0f && rigidbody.velocity.sqrMagnitude > Mathf.Pow(MaxSpeed, 2))
		{
			rigidbody.velocity = rigidbody.velocity.normalized * MaxSpeed;
		}
	}
}
