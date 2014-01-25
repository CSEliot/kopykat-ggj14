using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[RequireComponent (typeof(characterController))]
//this requires the character controller or compiler errors occur.
public class FirstPersonController : MonoBehaviour {
	
	private float movementSpeed = 3.001f; 
	private float mouseSensetivity = 5.0f;
	private float rotYSpeed = 1.01f;
	private float upDownRange = 69.0f;
	private float rotUpDown = 0.0f;
	private float jumpSpeed = 1.7f;
	private float verticalVelocity = 0.0f; 
	public string state;
	CharacterController characterController;
	public bool newState;
	public string oldState;
	public List<string> states = new List<string>();
	states.Add
			
	// Use this for initialization
	void Start () {
		Screen.lockCursor = true;
		characterController = GetComponent<CharacterController>();
		if (!characterController) {
			//freak out
		}
		state = "standing";
		newState = false;
	}
	
	// Update is called once per frame
	void Update () {

		//test for change in state
		if (oldState != state) 
		{
			newState = true;
		}
		//player rotation
		//left and right
		float rotLeftRight = Input.GetAxis("Mouse X")*mouseSensetivity;
		transform.Rotate(0, rotLeftRight, 0);
		//record old state and clear state for change
		oldState = state;
		state.Clear ();

		//Movement
		float forwardSpeed = Input.GetAxis("Vertical");
		float sideSpeed = Input.GetAxis("Horizontal");
		//Debug.Log ("FORWARD SPEED: "+forwardSpeed ); Debug.Log ("SIDE SPEED"+sideSpeed);
		//add the new states
		state.Add (forwardSpeed);
		state.Add (sideSpeed);


		verticalVelocity += Physics.gravity.y * Time.deltaTime;

		if (characterController.isGrounded && Input.GetButtonDown("Jump")){
			verticalVelocity = jumpSpeed;		
		}

		Vector3 speed = new Vector3( sideSpeed*movementSpeed, verticalVelocity*.50f, forwardSpeed*movementSpeed);
		
		speed = transform.rotation * speed;

		characterController.Move( speed * Time.deltaTime);
	}

	public ArrayList getState(){
		//Debug.Log (state[0] + ", " + state[1] + "TEST1");

		return state;
	}

	public bool getStateBool(){
		//Debug.Log (state[0] + ", " + state[1] + "TEST1");
		return newState;
	}
}
