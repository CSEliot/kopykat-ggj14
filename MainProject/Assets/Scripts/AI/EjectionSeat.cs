using UnityEngine;
using System.Collections;

public class EjectionSeat : MonoBehaviour {
	
	public float ThrustDuration = 0.0f;
	public float Thrust = 0.0f;
	
	private float thrustTime = 0.0f;
	private Rigidbody rb;
	
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
		if (rb == null)
		{
			Debug.Log ("EjectionSeat: actor does not have a rigidbody!");
			Destroy(this);
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		thrustTime += Time.deltaTime;
		rb.AddForce(Vector3.up * Thrust);
		if(thrustTime > ThrustDuration)
		{
			Destroy(this);
		}
	}
}
