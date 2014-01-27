﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[RequireComponent (typeof(characterController))]
//this requires the character controller or compiler errors occur.
public class FirstPersonControllerAI : MonoBehaviour {
	
	private float movementSpeed = 2.001f;
    private Quaternion targetRotation;
    private float jumpSpeed = 3f;
    private float rotationSpeed = 5;
	private float verticalVelocity = 0.0f;
    private float forwardSpeed;
    private float sideSpeed;
    private List<string> states = new List<string>();
    private List<string> stateQueue = new List<string>();
	private float playerDist;
    private bool toJump = false;
    private string newState;
    private Vector3 tempVectorDir;
    private Vector3 speed = new Vector3(0,0,0);
	CharacterController characterController;
	float waitTick = 0f;
	float reqTick;
	float modTick = 20f;
    bool caseState;
    string myState;
	UnityEngine.GameObject playerA;
	UnityEngine.GameObject playerB;
    private Animator animator;
    private int[] moveList = new int[3];
    //THIS IS FOR MIKE - SET TO ROOM CENTER, IS UNIQUE
    private Vector3 centerMapLocation = new Vector3(0, 0, 0);
    private bool speedAltered = false;


	// Use this for initialization
	void Start () {
        states.Add("standing"); states.Add("walking"); states.Add("jumping"); states.Add("hands up");
		Screen.lockCursor = true;
		characterController = GetComponent<CharacterController>();
		if (!characterController) {
			//freak out
		}
		playerA = GameObject.Find ("PlayerA");
        if (playerA == null)
        {
            Debug.Log("AM I NULL??");
        }
        animator = GetComponent<Animator>();
        moveList[0] = -1; moveList[1] = 0; moveList[2] = 1;
	}
	
	// Update is called once per frame
	void Update () {
        //myState = playerA.GetComponent<FirstPersonController>().getState();
           // Debug.Log("AI: I am adding the state: " + myState + " to my QUEUE!");
        
        //Debug.Log("AI one");
        //Debug.Log("AI: The new state was found to be: " + caseState);
        /*if (transform.position.y < 0.3f && Input.GetButton("Jump")){
            toJump = true;
        }*/
        caseState = playerA.GetComponent<FirstPersonController>().getStateBool();
        if (caseState == true)
        {
            myState = playerA.GetComponent<FirstPersonController>().getState();
            Debug.Log("AI: I am adding the state: " + myState + " to my QUEUE!");
            stateQueue.Add(myState);
        }
        //Debug.Log("AI two");
        if (stateQueue.Count > 0)
        {
            waitTick++;
        }
        
		//Debug.Log(playerA.GetComponent<FirstPersonController> ().getState()[0] + ", " + playerA.GetComponent<FirstPersonController> ().getState()[1] + "TEST2");

		playerDist = Vector3.Distance(GameObject.Find("PlayerA").transform.position, this.transform.position);
		reqTick = playerDist * modTick;

		if(waitTick >= reqTick){
            //Debug.Log("Setting state, current size left: " + stateQueue.Count);
			waitTick = 0f;
            if (stateQueue.Count > 0)
            {
                newState = stateQueue[0];
                stateQueue.RemoveAt(0);
                switch (newState)
                {
                    case "standing":
                        forwardSpeed = 0;
                        sideSpeed = 0;
                        animator.SetBool("isJumping", false);
                        animator.SetBool("isWalking", false);
                        Debug.Log("AI: I am standing!!");
                        return;
                    case "walking":
                        while((forwardSpeed == 0 && sideSpeed == 0 )){
                            forwardSpeed = moveList[Random.Range(0, 3)];
                            sideSpeed = moveList[Random.Range(0, 3)];
                        }
                        Debug.Log("AI: I am walking towards: ( " + forwardSpeed + "," + sideSpeed + " )");
                        animator.SetBool("isWalking", true);
                        animator.SetBool("isJumping", false);
                        return;
                    case "jumping":
                        toJump = true;
                        Debug.Log("AI: I will jump!!");
                        verticalVelocity = jumpSpeed;
                        return;
                    case "hands up":
                        return;
                    default:
                        return;
                }
            }

		}


		//player rotation
		//left and right
		/*float rotLeftRight = Input.GetAxis("Mouse X")*mouseSensetivity;
		transform.Rotate(0, rotLeftRight, 0);*/
        
        if (forwardSpeed!= 0 || sideSpeed != 0)
        {   
            tempVectorDir = new Vector3(sideSpeed, 0, forwardSpeed);
            //Debug.Log("About to rotate!, to: " + tempVectorDir);
            //targetRotation = Quaternion.LookRotation(tempVectorDir);
            //transform.eulerAngles = Vector3.up * Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetRotation.eulerAngles.y, rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(tempVectorDir),
                    Time.deltaTime * rotationSpeed
                    );
            
        }
		verticalVelocity += Physics.gravity.y * Time.deltaTime;
        //Debug.Log("IS HE GRUNDED? " + characterController.isGrounded);
		if (this.transform.position.y < 0.3f && toJump){//Input.GetButtonDown("Jump")){
            Debug.Log("AI: I JUMP!!");
			verticalVelocity = jumpSpeed;
            //animator.SetBool("isJumping", true);
            animator.SetBool("isWalking", false);
            //animator.SetBool("isStanding", false);
            toJump = false;
		}


        speed.Set(sideSpeed * movementSpeed, verticalVelocity * .50f, forwardSpeed * movementSpeed);
        speed = transform.rotation * speed;
        speedAltered = false;
        //ALSO FOR MIKE
        /*//Debug.Log(Vector3.Distance(centerMapLocation,this.transform.position));
        if (Vector3.Distance(centerMapLocation, this.transform.position) > 4.4f)
        {
            speedAltered = true;
            //Debug.Log("SPEEEEEEEEEEED: " + speed);
            this.transform.LookAt(centerMapLocation);
            speed.Set(transform.forward.x * movementSpeed, verticalVelocity * .50f, transform.forward.z * movementSpeed);
        }
        else if (Vector3.Distance(centerMapLocation, this.transform.position) < 1f)
        {
            speed.Set(sideSpeed * movementSpeed, verticalVelocity * .50f, forwardSpeed * movementSpeed);
            speed = transform.rotation * speed;
            speedAltered = false;
        }*/



		characterController.Move( speed * Time.deltaTime);
	}
}
