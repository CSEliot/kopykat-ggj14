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
	
	private float sqrGoToNormalDist = Mathf.Pow(15.0f, 2);
	private int agentID;

	public enum State {Walk, Idle, Panic, Dead, HandsUp};
	public FirstPersonController masterPlayer; // mirror player's behavior
	private float DeathTimer = 10f; // must be >> death animation time
	private float Tick = 0.1f; // TENTATIVE VALUE, SHOULD BE SET ACCORDING TO FRAMERATE
	private float RotationAngle = 0.0f;
	private float RotateMax = 0.2f;
	private float RotateTimer = 0f;
	private AIhandler aihandler;

	private float walkspeed = 1.0;
	private float panicspeed = walkspeed*2;
	
	//missile tracking/statistics members
	//Dictionary<int, GameObject> missiles;
	//negative value indicates no missiles locked onto agent
	//float sqrAvgMissileDist = -1.0f;
	//int numMissilesToIntercept = 0;
	
	// ADDED BY MIKE D
	public void SetMasterPlayer(PlayerInput P){
		masterPlayer = P;
	}

	/*
	 * When called the AI handler, the NPC determines what
	 * its next state should be.
	 * Also, for the NPC, Jump is an action, not a state.
	 */
	public void NextState()
	{
		if (aihandler.Bodycount > 0)
		{
			State = State.Panic;
		}
		else
		{
			if (masterPlayer.IsScary())
			{
				State = State.HandsUp;
			}
			else {
				if (masterPlayer.IsWalking())
				{
					State = State.Walk;
				}
				else {
					State = State.Idle;
				}
				if (masterPlayer.IsJumping())
				{
					Jump();
				}
			}
		}
	}
	
	public void Jump()
	{
		if (characterController.isGrounded){
			verticalVelocity = jumpSpeed;
		}
	}
	
	public void Kill()
	{
		aihandler.ModBodycount(1);
		State = State.Dead;
		//Start death animation
	}

	
	// Use this for initialization
	void Start () {
		//add us to the global AI manager so we can be enabled/disabled
		agentID = AIhandler.GetInstance().AddAgent(this);
		//try to target a player if no other target's set
		if(Target == null)
		{
			Target = GameObject.FindWithTag("Player").GetComponent<PlayerLogic>().Actor;
		}
		setActor(Actor);
		//healthInfo = actorCtrl.GetComponent<HealthInfo>();
		//to avoid surprising the player in debug mode, match our enabled status to the AI manager's agent status
		enabled = AIhandler.GetInstance().AgentsEnabled;
		RotationAngle = Random.Range(0.0f, 2*Mathf.PI); //initial

	}

	void OnTriggerEnter(Civilian c)
	{
		RotationAngle += Random.Range(-1, 1)*Mathf.PI*.75;
	}
	void OnTriggerEnter(FirstPersonController p)
	{
		RotationAngle += Random.Range(-1, 1)*Mathf.PI*.75;
	}

	void Update (){
		switch (State){
		case State.Dead:
			if (DeathTimer > 0)
			{
				// Countdown until animation is over
				DeathTimer -= (Tick * Time.deltaTime);
			}
			else {
				// animation is over = delete me
				// signal AI controller
				AIhandler.GetInstance().RemoveAgent(agentID);
				Destroy(gameObject);
				Destroy(this);
				AIhandler.ModBodycount(-1);
				AIhandler.Signal();
			}
			break;
		case State.HandsUp:
			break;
		case State.Idle:
			break;
		case State.Walk:
			RotationAngle += Random.Range (-RotateMax, RotateMax);
			Vector3 speed = new Vector3( Mathf.Cos (RotationAngle)*walkspeed, verticalVelocity*.50f, Mathf.sin (RotationAngle)*walkspeed);
			break;
		default: // panic
			// panic or walk
			RotationAngle += Random.Range (-RotateMax, RotateMax);
			Vector3 speed = new Vector3( Mathf.Cos (RotationAngle)*panicspeed, verticalVelocity*.50f, Mathf.sin (RotationAngle)*panicspeed);
			break;
		}
	}
	
	
}
