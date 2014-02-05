using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(CharacterController))]
public class playerController : MonoBehaviour {
	
	private float movementSpeed = 1.3f;
	private float mouseSensitivity = 5.0f;
	private float jumpSpeed = 3.0f;
	private List<string> states = new List<string>();
	float verticalRotation = 0;
	private float upDownRange = 60.0f;
    private string state;
    private bool newState;
    private Animator animator;
	//these already_ booleans prevent the new states from repeating
    private bool alreadyMoving = false;
    private bool alreadyStanding = true;
    private bool alreadyJumping = false;
    private bool alreadyStabbing = false;
    private GameObject[] tomList; //number of AI NPC
    private int talkedToCount = 0;
    private int numWeNeedToTalkTo;
	float verticalVelocity = 0;
    private Vector3 centerMapLocation = new Vector3(0, 0, 0);
	float forwardSpeed;
	float sideSpeed;
    //in order to detect speed changes, we use directional booleans
    bool movForward=false; bool movLeft=false; bool movRight=false; bool movBack=false;
    bool oldmovForward=false; bool oldmovLeft=false; bool oldmovRight=false; bool oldmovBack=false;
	float rotLeftRight;
	int playerNum;
	string jumpString;
    string stabString;
    float groundStandingHeight = 0.0654f;
    bool makeKillDead = false;
    
	
	CharacterController characterController;
    private int killTimer;
	// Use this for initialization
	void Start () {
        if (this.name == "PlayerA")
        {
			playerNum = 1;
			//set certain non-movement string pings for player
			jumpString = "p1_Jump";
            stabString = "p1_Stab";
            //Debug.Log(jumpString);

		}
		else if(this.name == "PlayerB")
		{
			playerNum = 2;
			jumpString = "p2_Jump";
            stabString = "p2_Stab";
            //Debug.Log(jumpString);
		}

       // Debug.Log(this.name);
        states.Add("standing");states.Add("walking");states.Add("jumping");states.Add("hands up");
		Screen.lockCursor = true;
		characterController = GetComponent<CharacterController>();
		if (!characterController) {
			//freak out
		}
		state = "standing";
		newState = false;
        animator = GetComponent<Animator>();
        tomList = GameObject.FindGameObjectsWithTag("Tom");
        numWeNeedToTalkTo = tomList.Length;
        //Debug.Log(numWeNeedToTalkTo);
	}

    // Update is called once per frame
    void Update()
    {
        if (!makeKillDead)
        {
            animator.SetBool("isAss", false);
            newState = false;
            //don't set new state to false until EVERYONE has been told the new state
            //Debug.Log("Playetr 1");
            /*if (talkedToCount >= numWeNeedToTalkTo)
            {
                //Debug.Log("Setting newState to false!!");
                newState = false;
                talkedToCount = 0;
            }*/
            //Debug.Log("Player 2");

            oldmovBack = movBack;
            oldmovForward = movForward;
            oldmovLeft = movLeft;
            oldmovRight = movRight;
            ///MOVEMENT CODE**************/
            /// So, Player 1/A uses the keyboard/controller 2, while player 2/B uses controller 1. 
            if (playerNum == 1)
            {
                rotLeftRight = Input.GetAxis("p1_Mouse X") * mouseSensitivity;
                //Since PLayer 1 won't be using the keyboard and controller at the same time,
                // we can simply just do += on rotLeftRight because one of them will always be 0.
                //MAYBE NOT WORKING--rotLeftRight += Input.GetAxis("p1_Look Rotation");
                if (!alreadyStabbing)
                {
                    //Getting Input
                    forwardSpeed = Input.GetAxis("p1_Forward") * movementSpeed;
                    sideSpeed = Input.GetAxis("p1_Strafe") * movementSpeed;
                    transform.Rotate(0, rotLeftRight, 0);
                }
                else
                {
                    forwardSpeed = 0;
                    sideSpeed = 0;
                }


            }
            else if (playerNum == 2)
            {
                //player 2 can only use a controller, so here we go
                rotLeftRight = Input.GetAxis("p2_Look Rotation")*2;
                if (!alreadyStabbing)
                {
                    //Getting Input
                    forwardSpeed = Input.GetAxis("p2_Forward") * movementSpeed;
                    sideSpeed = Input.GetAxis("p2_Strafe") * movementSpeed;
                    transform.Rotate(0, rotLeftRight, 0);
                }
                else
                {
                    forwardSpeed = 0;
                    sideSpeed = 0;
                }
                
            }
            //Gravity
            verticalVelocity += Physics.gravity.y * Time.deltaTime;

            // --Action Detection--
            // Jumping
            if (transform.position.y < groundStandingHeight && Input.GetAxis(jumpString) > 0 && alreadyJumping == false && !newState)
            {
                //animator.SetBool("isJumping", true);
                animator.SetBool("isAss", false);
                animator.SetBool("isWalking", false);
                //Debug.Log("Player: You jumped!");
                verticalVelocity = jumpSpeed;
                state = "jumping";
                newState = true;
                alreadyMoving = false;
                alreadyStanding = false;
                alreadyJumping = true;
                alreadyStabbing = false;
                //animator.SetBool("isWalking", false);
                //animator.SetBool("isJumping", true);
            }
            //Walking
            else if (((forwardSpeed > 0.0f || sideSpeed > 0.0f) ||
                      (forwardSpeed < 0.0f || sideSpeed < 0.0f)) && !newState && alreadyMoving == false)//|| rotLeftRight != 0
            {
               // Debug.Log("Player: Movement detected!! I am " + states[1]);
                animator.SetBool("isJumping", false);
                animator.SetBool("isAss", false);
                animator.SetBool("isWalking", true);
                state = states[1];
                newState = true;
                alreadyMoving = true;
                alreadyStanding = false;
                alreadyJumping = false;
                alreadyStabbing = false;
                //booleans set inside walking phase as well so that detection of start moving doesn't occur.
                if (forwardSpeed != 0)
                {
                    if (forwardSpeed > 0)
                    {
                        movForward = true;
                        movBack = false;
                    }
                    else
                    {
                        movForward = false;
                        movBack = true;
                    }
                }
                else
                {
                    movBack = false;
                    movForward = false;
                }
                if (sideSpeed != 0)
                {
                    if (sideSpeed > 0)
                    {
                        movRight = true;
                        movLeft = false;
                    }
                    else
                    {
                        movRight = false;
                        movLeft = true;
                    }
                }
                else
                {
                    movRight = false;
                    movLeft = false;
                }

            }
            //Standing
            else if (alreadyStanding == false && (forwardSpeed == 0.0f && sideSpeed == 0.0f && !alreadyStabbing) && !newState)
            {
                //Debug.Log("Player: No movement detected!! I am " + states[0]);
                animator.SetBool("isWalking", false);
                animator.SetBool("isJumping", false);
                animator.SetBool("isAss", false);
                state = states[0];
                newState = true;
                alreadyMoving = false;
                alreadyStanding = true;
                alreadyStabbing = false;
                alreadyJumping = false;
            }
            if (alreadyStabbing == false && Input.GetButtonDown(stabString))
            {
                //Debug.Log("WE GON' STABBY STABBY NOW");
                //animator.SetBool("isWalking", false);
                //animator.SetBool("isJumping", false);
                animator.SetBool("isAss", true);
                

            }
            if (!alreadyStabbing && animator.GetCurrentAnimatorStateInfo(0).IsName("MrKat_Assasinate"))
            {
                
                alreadyStabbing = true;
                alreadyMoving = false;
                alreadyStanding = false;
                alreadyJumping = false;
            }
            else if (alreadyStabbing && !animator.GetCurrentAnimatorStateInfo(0).IsName("MrKat_Assasinate"))
            {
                alreadyStabbing = false;
               // Debug.Log("Player: YOU MAY STAB AGAIN!!");
            }
            //Debug.Log(Vector3.Distance(centerMapLocation,this.transform.position));

            //Speed Math
            Vector3 speed = new Vector3(sideSpeed, verticalVelocity, forwardSpeed);
            speed = transform.rotation * speed;
            if (!alreadyStabbing && !animator.GetCurrentAnimatorStateInfo(0).IsName("MrKat_Assassinate"))
            {
                characterController.Move(speed * Time.deltaTime);
            }
            else
            {
                //Debug.Log("Player: We can't move!! Stabbing!!");
            }

            // We want direction changes to ping the Walking check below, so we set up the following:
            //the following booleans are all used to detect directional change.
            if (forwardSpeed != 0 && alreadyMoving)
            {
                if (forwardSpeed > 0)
                {
                    movForward = true;
                    movBack = false;
                }
                else
                {
                    movForward = false;
                    movBack = true;
                }
            }
            else
            {
                movBack = false;
                movForward = false;
            }
            if (sideSpeed != 0 && alreadyMoving)
            {
                if (sideSpeed > 0)
                {
                    movRight = true;
                    movLeft = false;
                }
                else
                {
                    movRight = false;
                    movLeft = true;
                }
            }
            else
            {
                movRight = false;
                movLeft = false;
            }
            if ((oldmovRight != movRight || oldmovLeft != movLeft || oldmovForward != movForward || oldmovBack != movBack) && alreadyMoving)
            {
                newState = true;
                //Debug.Log("Player: New Move direction (" + forwardSpeed + ", " + sideSpeed + ") detected from: Player " + playerNum);
            }

        }
        else
        {
            animator.SetBool("isHandsUp", true);
            animator.SetBool("isDead", true);
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("MrKat_Death"))
            {
                killTimer++;
                //Debug.Log(killTimer);
                //some arbitrary number. . .
                if (killTimer > 400)
                {
                    Destroy(gameObject);
                }
            }
        }

    }

    public string getState(){
		//Debug.Log (state[0] + ", " + state[1] + "TEST1");
		return state;
	}

	public bool getStateBool(){
        return newState;
;
	}

    public bool getStabbing()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName("MrKat_Assassinate");
    }

    void OnTriggerStay(Collider colide)
    {
        //Debug.Log(" O HI MARK NNNPC");
        //bool thisOBJ = 
        //Debug.Log("CHECK: " + colide.gameObject.transform.parent.gameObject.GetComponent<playerController>().getStabbing());
        if (colide.gameObject.transform.parent.gameObject.GetComponent<playerController>().getStabbing())
        {
            makeKillDead = true;
        }
    }

    void OnTriggerExit(Collider colide)
    {
        (gameObject.transform.FindChild("Targetted").GetComponent("Halo") as Behaviour).enabled = false;
    }

    void OnTriggerEnter(Collider colide)
    {
        //Debug.Log("OH HI MARK");

        (gameObject.transform.FindChild("Targetted").GetComponent("Halo") as Behaviour).enabled = true;
        //Debug.Log("OH HI MARK222");
    }

    public override string ToString()
    {
        return "TEST";
    }
    public void kill()
    {
        //Debug.Log("WE GUN DIE NIGA");
        makeKillDead = true;
    }
    
}