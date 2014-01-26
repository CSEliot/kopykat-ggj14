using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CivilianInput : MonoBehaviour {
	
	//AI controller for enemies. Blender build's AI seems to be a state machine
	//could do some crazy interface where each state has a BehaviorUpdate(), but that's probably not worth the work
	public float MoveForce = 3;
	public float TrackingSpeed = 1.0f;
	public float Speed = 1.0f;
	
	public ActorController ActorCtrl;
	private AIManager aiManager;
	private AnimationProcessor processor;
	private HealthInfo healthInfo;
	private GameObject interceptTarget = null; //temporary target assigned when something must be intercepted (i.e., missiles)
	private float stateChangeTimer = 0; //measured in seconds

	private int agentID;
	
	// Added by Mike D. 12:44 am sat
	public enum State {Walk, Idle, Panic, Dead, HandsUp};
	public PlayerInput masterPlayer;
	private State aiState;
	private float DeathTimer = -100;
	
	
	// ADDED BY MIKE D
	public void SetMasterPlayer(PlayerInput P){
		masterPlayer = P;
	}
	
	/*
	 * Mike D: This probably isn't how signals will work,
	 * but this is how the bots should react.
	 * Also, I don't know where this goes, maybe in the
	 * update section?
	 * if (IsHandsUpState())
	 * {
	 * 		if (GlobalCorpses > 0)
	 * 			State = State.Panic;
	 * 		else
	 * 			State = State.Normal;
	 * }
	 * 
	 * if (DeathTimer >= 0)
	 * {
	 * 		DeathTimer -= somevalue
	 * }
	 * else if (DeathTimer > -99)
	 * {
	 * 		GLOBAL BODYCOUNT --;
	 * 		Delete or become invisible
	 * }
	 * 
	 */
	
	public void Jump()
	{
	}
	
	public void Kill()
	{
		//GLOBAL_BODYCOUNT ++;
		//PlayDeathAnimation()
		DeathTimer = 10; // or something
	}
	
	// Use this for initialization
	void Start () {
		//add us to the global AI manager so we can be enabled/disabled
		healthInfo = ActorCtrl.GetComponent<HealthInfo>();
		aiManager = AIManager.GetInstance();
		//TODO: Freak out if aiManager is null
	}
	
	void Update () {
		if(ActorCtrl.IsAlive)
		{
			stateChangeTimer += Time.deltaTime;
			//TODO
			//Ask AIManager: closest player?
			//They're now commander
			//Execute all commands in queue, add execution delay
		}
		else
		{
			//we must be dead; the actor's controller should play an animation. When the animation's done, drop loot and then remove the actor
			if(!ActorCtrl.IsPlayingAnimation)
			{	
				//gameObject.SetActive(false);
				
				//Destroy(Actor);
				this.enabled = false;
				Destroy(gameObject);
				Destroy(this);
			}
		}
	}

	private void imitateMaster()
	{
		SetMasterPlayer (aiManager.GetNearestMaster (transform.position));
		if (aiState==State.Walk || aiState==State.Idle) 
		{
			if(masterPlayer.StartedMove)
			{
				aiState = State.Walk;
			}
			else
			{
				aiState = State.Idle;
			}
			if(masterPlayer.StartedJump)
			{
				Jump();
			}
			if(masterPlayer.StartedHandsUp || masterPlayer.StartedShiv)
			{
				aiState = State.HandsUp;
			}
		}
	}

	private void updateState()
	{
		switch (aiState)
		{
			default:
				break;
		}
		//cap our velocity if needed
		ActorCtrl.rigidbody.velocity = Vector3.ClampMagnitude(ActorCtrl.rigidbody.velocity, Speed);
	}

	void setActor (ActorController actor)
	{
		if(actor != null)
		{
			processor = ActorCtrl.GetComponentInChildren<AnimationProcessor>();
			ActorCtrl.transform.parent = transform;
		}
	}
}
