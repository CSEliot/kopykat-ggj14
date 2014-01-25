using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour {
	
	private float movementSpeed = 3.0f;
	private float mouseSensitivity = 5.0f;
	private float jumpSpeed = 3.0f;
	private List<string> states = new List<string>();
	float verticalRotation = 0;
	private float upDownRange = 60.0f;
    private string state;
    private bool newState;
    private Animator animator;
    private bool alreadyMoving = false;
    private bool alreadyStanding = true;
	
	float verticalVelocity = 0;
	
	CharacterController characterController;
	
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
        newState = false;
		// Rotation
		
        //Extra Movement
		float rotLeftRight = Input.GetAxis("Mouse X") * mouseSensitivity;
		transform.Rotate(0, rotLeftRight, 0);
		verticalRotation -= Input.GetAxis("Mouse Y") * mouseSensitivity;
		verticalRotation = Mathf.Clamp(verticalRotation, -upDownRange, upDownRange);
		
        //Getting Input
		float forwardSpeed = Input.GetAxis("Vertical") * movementSpeed;
		float sideSpeed = Input.GetAxis("Horizontal") * movementSpeed;
		
        //Gravity
		verticalVelocity += Physics.gravity.y * Time.deltaTime;
        
        // --Action Detection--
        // Jumping
		if( characterController.isGrounded && Input.GetButton("Jump") ) {
            Debug.Log("You jumped!");
            verticalVelocity = jumpSpeed;
            state = states[2];
            newState = true;
            alreadyMoving = false;
            alreadyStanding = false;
            //animator.SetBool("isWalking", false);
            //animator.SetBool("isJumping", true);
		}
        // Standing
        else if ((forwardSpeed > 0.0f || sideSpeed > 0.0f) && !newState && !alreadyMoving)//|| rotLeftRight != 0
        {
            Debug.Log("Movement detected!! I am " + states[1]);
            //animator.SetBool("isWalking", true);
           // animator.SetBool("isJumping", false);
            state = states[1];
            newState = true;
            alreadyMoving = true;
            alreadyStanding = false;
        }
        else if (alreadyStanding == false && (forwardSpeed < 0.2f && sideSpeed < 0.2f))
        {
            Debug.Log("No movement detected!! I am " + states[0]);
            //animator.SetBool("isWalking", false);
            //animator.SetBool("isJumping", false);
            state = states[0];
            newState = true;
            alreadyMoving = false;
            alreadyStanding = true;
        }



        //Speed Math
		Vector3 speed = new Vector3( sideSpeed, verticalVelocity, forwardSpeed );
		speed = transform.rotation * speed;
		characterController.Move( speed * Time.deltaTime );
	}

    public string getState(){
		//Debug.Log (state[0] + ", " + state[1] + "TEST1");
		return state;
	}

	public bool getStateBool(){
        bool tempState = newState;
		return tempState;
	}
    public override string ToString()
    {
        return "TEST";
    }
}



/*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//[RequireComponent (typeof(characterController))]
//this requires the character controller or compiler errors occur.
public class FirstPersonController : MonoBehaviour {
	
	private float movementSpeed = 2.001f; 
	private float mouseSensetivity = 5.0f;
	private float rotYSpeed = 1.01f;
	private float upDownRange = 69.0f;
	private float rotUpDown = 0.0f;
	private float jumpSpeed = 2f;
	private float verticalVelocity = 0f; 
	private float movementSpeed = GameSystem.WalkSpeed;
	private float mouseaihandlertivity = GameSystem.MouseSensitivity;
	private float rotYSpeed = 1.01f;
	private float upDownRange = 69.0f;
	private float rotUpDown = 0.0f;
	private float jumpSpeed = GameSystem.JumpSpeed;
	private float verticalVelocity = 0.0f; 
	public string state;
    private bool alreadyMoving = false;
	CharacterController characterController;
	public bool newState;
	public string oldState;
	private bool testy = true;
    private List<string> states = new List<string>();
    private Animator animator;
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
		float rotLeftRight = Input.GetAxis("Mouse X")*mouseSensetivity;

        Debug.Log("rotLeftRight = " + rotLeftRight);
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

		Vector3 speed = new Vector3( sideSpeed*movementSpeed, verticalVelocity*.50f, forwardSpeed*movementSpeed);   1`
		

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

        }*/

      //  }

		// VERY IMPORTANT
		/* if (state changed)
		 * {
		 * 		aihandler.Signal();
		 * }
		 */
/*
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
*/