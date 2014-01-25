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
	private bool testy = true;
    private List<string> states = new List<string>();


	public bool IsWalking()
	{
		return true;
	}

	public bool IsJumping()
	{
		return true;
	}

	// stabbing or hands up
	public bool IsScary()
	{
		return true;
	}
			
	// Use this for initialization
	void Start () {
        states.Add("standing");states.Add("walking");states.Add("jumping");states.Add("hands up");
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
		//player rotation
		//left and right
		float rotLeftRight = Input.GetAxis("Mouse X")*mouseSensetivity;
        Debug.Log("rotLeftRight = " + rotLeftRight);
		transform.Rotate(0, rotLeftRight, 0);
		//record old state and clear state for change
		oldState = state;

		//Movement
		float forwardSpeed = Input.GetAxis("Vertical");
		float sideSpeed = Input.GetAxis("Horizontal");
		//Debug.Log ("FORWARD SPEED: "+forwardSpeed ); Debug.Log ("SIDE SPEED"+sideSpeed);
		//add the new states


		verticalVelocity += Physics.gravity.y * Time.deltaTime;

		if (characterController.isGrounded && Input.GetButtonDown("Jump")){
			verticalVelocity = jumpSpeed;
            state = states[2];
            newState = true;
		}

		Vector3 speed = new Vector3( sideSpeed*movementSpeed, verticalVelocity*.50f, forwardSpeed*movementSpeed);
		
		speed = transform.rotation * speed;
        if (speed != Vector3.zero || rotLeftRight != 0)
        {
            state = states[1];
            newState = true;
        }
        else if (!newState && speed == Vector3.zero && rotLeftRight == 0)
        {
            state = states[0];
            newState = true;
        }
		characterController.Move( speed * Time.deltaTime);
        if (newState)
        {
            Debug.Log(state);
        }
	}

	public string getState(){
		//Debug.Log (state[0] + ", " + state[1] + "TEST1");
		return state;
	}

	public bool getStateBool(){
        bool tempState = newState;
        newState = false;
		return tempState;
	}
}
