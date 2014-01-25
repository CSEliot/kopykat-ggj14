using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Civilian : MonoBehaviour {
	
	//public GameObject Actor = null;
	//public GameObject Target = null;
	//public float MoveForce = 3;
	//public float Speed = 1.0f;
	
	private ActorController actorCtrl;
	private AnimationProcessor processor;

	//private float stateChangeTimer = 0
	//private const float stateChangeTimeLimit = 5.0f;
	
	public enum State {Walk, Idle, Panic, Dead, HandsUp};
	public FirstPersonController masterPlayer; // mirror player's behavior
	private float DeathTimer = 10f; // must be >> death animation time
	private float Tick = GameSystem.Tick; // TENTATIVE VALUE, SHOULD BE SET ACCORDING TO FRAMERATE
	private float RotationAngle = Random.Range(0.0f,Mathf.PI*2);
	private float RotateDelta = 0.2f;
	private float RotateTimer = 0.0f;
	private AIhandler aihandler;
	CharacterController characterController;
	private float jumpSpeed = GameSystem.JumpSpeed;
	private float verticalVelocity = 0.0f; 
	private float gravity = GameSystem.Gravity;

	public void Start(){
		// Add to aihandler
		// set position to position
		characterController = GetComponent<CharacterController>();
	}

	// Called on signal change
	public void SetMasterPlayer(PlayerInput P){
		masterPlayer = P;
	}

	// Not sure if we will use this
	public aihandler Getaihandler()
	{
		return aihandler;
	}

	/*
	 * Called by AIhandler when a signal occurs.
	 * When called the AI handler, the NPC determines what
	 * its next state should be.
	 * Also, for the NPC, Jump is an action, not a state.
	 */
	public void NextState(bool IsPanic)
	{
		if (aihandler.ShouldPanic)
		{
			State = State.Panic;
			movementspeed = GameSystem.PanicSpeed;
		}
		else if (aihandler.ShouldHandsUp)
		{
			// masterplayer stabbed or did handsup
			State = State.HandsUp;
		}
		else if (masterPlayer.IsWalking())
		{
			State = State.Walk;
			movementspeed = GameSystem.WalkSpeed;
		}
		else
		{
			State = State.Idle;
		}
		//
		if (masterPlayer.IsJumping())
		{
			Jump();
		}
	}
	
	private void Jump()
	{
		if (characterController.isGrounded){
			verticalVelocity = jumpSpeed;
		}
	}

	// Signals AI handler
	// Called by player
	public void Kill()
	{
		aihandler.ModBodycount(1);
		State = State.Dead;
		//Start death animation
	}

	/* OLD CODE
	// Use this for initialization
	void Start () {
		//add us to the global AI manager so we can be enabled/disabled
		agentID = aihandler.GetInstance().AddAgent(this);
		//try to target a player if no other target's set
		if(Target == null)
		{
			Target = GameObject.FindWithTag("Player").GetComponent<PlayerLogic>().Actor;
		}
		setActor(Actor);
		//healthInfo = actorCtrl.GetComponent<HealthInfo>();
		//to avoid surprising the player in debug mode, match our enabled status to the AI manager's agent status
		enabled = aihandler.GetInstance().AgentsEnabled;
		RotationAngle = Random.Range(0.0f, 2*Mathf.PI); //initial

	}*/

	// Collisions
	void OnTriggerEnter(Civilian c)
	{
		RotationAngle += Random.Range(-1, 1)*Mathf.PI*.75;
	}
	void OnTriggerEnter(FirstPersonController p)
	{
		RotationAngle += Random.Range(-1, 1)*Mathf.PI*.75;
	}


	/* This is called once per frame! Keep it simple!
	 * Generally should not be checking for changes in game states
	 * */

	void Update (){
		switch (State){
		case State.Dead:
			if (DeathTimer > 0)
			{
				// Countdown until animation is over
				DeathTimer -= (Tick * Time.deltaTime);
			}
			else {
				// animation and fade is over = delete me
				// signal AI controller
				aihandler.RemoveCivilian(this);
				aihandler.ModBodycount(-1);
				//Destroy(gameObject);
				Destroy(this);
			}
			break;
		case State.HandsUp:
			// Civilians are paused during hands-up mode
			// Hmm... not sure if we actually need this part
			if (aihandler.ShouldPanic)
			{
				State = State.Panic;
			}
			else if (!aihandler.ShouldHandsUp)
			{
				//wait for panic mode to end
				State = State.Idle;
			}
			break;
		case State.Idle:
			// it is possible to jump without moving
			speed = new Vector3(0, verticalVelocity*gravity, 0);
			break;
		default: // walk or panic
			RotationAngle += Random.Range (-RotateDelta, RotateDelta);
			speed = new Vector3( Mathf.Cos (RotationAngle)*movementspeed, verticalVelocity*gravity, Mathf.sin (RotationAngle)*momentspeed);
			break;
		}
	}
	
	
}
