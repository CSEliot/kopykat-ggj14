using UnityEngine;
using System.Collections;

//[RequireComponent (typeof(characterController))]
//this requires the character controller or compiler errors occur.
public class FirstPersonController : MonoBehaviour {
	
	private float movementSpeed = .3001f; 
	private float mouseSensetivity = 10.0f;
	private float rotYSpeed = 1.01f;
	private float upDownRange = 69.0f;
	private float rotUpDown = 0.0f;
	private float jumpSpeed = 1.7f;
	private float verticalVelocity = 0.0f; 

	CharacterController characterController;

	// Use this for initialization
	void Start () {
		Screen.lockCursor = true;
		characterController = GetComponent<CharacterController>();
		if (!characterController) {
			//freak out
		}


	}
	
	// Update is called once per frame
	void Update () {

		//player rotation
		//left and right
		float rotLeftRight = Input.GetAxis("Mouse X")*mouseSensetivity;
		transform.Rotate(0, rotLeftRight, 0);
		//up and down (with camera)
		rotUpDown -= Input.GetAxis("Mouse Y")*mouseSensetivity;
		rotUpDown= Mathf.Clamp(rotUpDown, -upDownRange, upDownRange);
		
		Camera.main.transform.localRotation = Quaternion.Euler(rotUpDown,0,0);

				
		//Movement
		float forwardSpeed = Input.GetAxis("Vertical");
		float sideSpeed = Input.GetAxis("Horizontal");

		verticalVelocity += Physics.gravity.y * Time.deltaTime;

		if (characterController.isGrounded && Input.GetButtonDown("Jump")){
			verticalVelocity = jumpSpeed;		
		}

		Vector3 speed = new Vector3( sideSpeed*movementSpeed, verticalVelocity*.50f, forwardSpeed*movementSpeed);
		
		speed = transform.rotation * speed;
		

		characterController.Move( speed * Time.deltaTime);
	}
}
