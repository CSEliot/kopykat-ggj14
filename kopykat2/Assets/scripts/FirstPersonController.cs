using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//[RequireComponent (typeof(characterController))]
//this requires the character controller or compiler errors occur.
public class FirstPersonController : MonoBehaviour {
	
<<<<<<< HEAD
	private float movementSpeed = 2.001f; 
	private float mouseSensetivity = 5.0f;
	private float rotYSpeed = 1.01f;
	private float upDownRange = 69.0f;
	private float rotUpDown = 0.0f;
	private float jumpSpeed = 2f;
	private float verticalVelocity = 0f; 
=======
	private float movementSpeed = GameSystem.WalkSpeed;
	private float mouseaihandlertivity = GameSystem.MouseSensitivity;
	private float rotYSpeed = 1.01f;
	private float upDownRange = 69.0f;
	private float rotUpDown = 0.0f;
	private float jumpSpeed = GameSystem.JumpSpeed;
	private float verticalVelocity = 0.0f; 
>>>>>>> origin/master
	public string state;
    private bool alreadyMoving = false;
	CharacterController characterController;
	public bool newState;
	public string oldState;
	private bool testy = true;
    private List<string> states = new List<string>();
<<<<<<< HEAD
    private Animator animator;


=======
	private bool panicMode = false;
	private AIhandler aihandler;
	private float gravity = GameSystem.Gravity;
	
	public bool IsJumping()
	{
		return true;
	}

	// stabbing or hands up
	public bool IsScary()
	{
		return true;
	}
>>>>>>> origin/master
			
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
        animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		//player rotation
		//left and right
<<<<<<< HEAD
		float rotLeftRight = Input.GetAxis("Mouse X")*mouseSensetivity;
=======
		if (aihandler.IsPanic)
		{
			movementSpeed = GameSystem.PanicSpeed;
		}
		else{
			movementSpeed = GameSystem.WalkSpeed;
		}

		float rotLeftRight = Input.GetAxis("Mouse X")*mouseSensitivity;
        Debug.Log("rotLeftRight = " + rotLeftRight);
>>>>>>> origin/master
		transform.Rotate(0, rotLeftRight, 0);

		//Movement
		float forwardSpeed = Input.GetAxis("Vertical");
		float sideSpeed = Input.GetAxis("Horizontal");
		Debug.Log ("FORWARD SPEED: "+forwardSpeed ); Debug.Log ("SIDE SPEED"+sideSpeed);



		verticalVelocity += Physics.gravity.y * Time.deltaTime;

        //Debug.Log("IS YOU GRUNDED? " + characterController.isGrounded);
		if (transform.position.y < 0.5f && Input.GetButtonDown("Jump")){
            Debug.Log("You jumped!");
            verticalVelocity = jumpSpeed;
            state = states[2];
            newState = true;
            alreadyMoving = false;
            animator.SetBool("isWalking", false);
            animator.SetBool("isJumping", true);
		}

<<<<<<< HEAD
		Vector3 speed = new Vector3( sideSpeed*movementSpeed, verticalVelocity*.50f, forwardSpeed*movementSpeed);
=======
		Vector3 speed = new Vector3( sideSpeed*movementSpeed, verticalVelocity*gravity, forwardSpeed*movementSpeed);
		
>>>>>>> origin/master
		speed = transform.rotation * speed;
        if ((forwardSpeed != 0 || sideSpeed != 0) && !newState && !alreadyMoving)//|| rotLeftRight != 0
        {
            Debug.Log("Movement detected!! I am " + states[1]);
            animator.SetBool("isWalking", true);
            animator.SetBool("isJumping", false);
            state = states[1];
            newState = true;
            alreadyMoving = true;
        }
        else if (forwardSpeed < 0.2f && sideSpeed < 0.2f)// && rotLeftRight == 0)
        {
            Debug.Log("No movement detected!! I am " + states[0]);
            animator.SetBool("isWalking", false);
            animator.SetBool("isJumping", false);
            state = states[0];
            newState = true;
            alreadyMoving = false;
        }
        Debug.Log(speed);
		characterController.Move( speed * Time.deltaTime);
        /*if (newState)
        {
            Debug.Log(state);
<<<<<<< HEAD
        }*/
=======
        }

		// VERY IMPORTANT
		/* if (state changed)
		 * {
		 * 		aihandler.Signal();
		 * }
		 */
>>>>>>> origin/master
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
