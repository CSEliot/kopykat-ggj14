using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[RequireComponent (typeof(characterController))]
//this requires the character controller or compiler errors occur.
public class FirstPersonControllerAI : MonoBehaviour {
	
	private float movementSpeed = .3001f; 
	private float mouseSensetivity = 10.0f;
	private float rotYSpeed = 1.01f;
	private float upDownRange = 69.0f;
	private float rotUpDown = 0.0f;
	private float jumpSpeed = 1.7f;
	private float verticalVelocity = 0.0f;
    private List<string> states = new List<string>();
	public float playerDist;
	CharacterController characterController;
	float waitTick = 0f;
	float reqTick;
	float modTick = 300f;
	UnityEngine.GameObject playerA;
	UnityEngine.GameObject playerB;

	// Use this for initialization
	void Start () {
        states.Add("standing"); states.Add("walking"); states.Add("jumping"); states.Add("hands up");
		Screen.lockCursor = true;
		characterController = GetComponent<CharacterController>();
		if (!characterController) {
			//freak out
		}
		playerA = GameObject.Find ("PlayerA");
	}
	
	// Update is called once per frame
	void Update () {

		//Debug.Log(playerA.GetComponent<FirstPersonController> ().getState()[0] + ", " + playerA.GetComponent<FirstPersonController> ().getState()[1] + "TEST2");

		playerDist = Vector3.Distance(GameObject.Find("PlayerA").transform.position, this.transform.position);
		reqTick = playerDist * modTick;
		Debug.Log(reqTick);
		if (playerA.GetComponent<FirstPersonController> ().getStateBool ()) {
			states.Add(playerA.GetComponent<FirstPersonController> ().getState());

		}
		if(waitTick >= reqTick){
			waitTick = 0f;
			float forwardSpeed = Random.Range(-1, 1);
			float sideSpeed = Random.Range(-1,1);
			states.RemoveAt(0);
		}
		else{
			waitTick++;
			//Movement
		}


		//player rotation
		//left and right
		float rotLeftRight = Input.GetAxis("Mouse X")*mouseSensetivity;
		transform.Rotate(0, rotLeftRight, 0);
						

		verticalVelocity += Physics.gravity.y * Time.deltaTime;

		if (characterController.isGrounded && Input.GetButtonDown("Jump")){
			verticalVelocity = jumpSpeed;		
		}

		Vector3 speed = new Vector3( sideSpeed*movementSpeed, verticalVelocity*.50f, forwardSpeed*movementSpeed);
		
		speed = transform.rotation * speed;
		

		characterController.Move( speed * Time.deltaTime);
	}
}
