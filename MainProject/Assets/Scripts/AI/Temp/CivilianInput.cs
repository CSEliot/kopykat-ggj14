using UnityEngine;
using System.Collections;

public class CivilianInput2 : MonoBehaviour {
	
	bool bDying;
	float speed;
	float angle;
	float TimerHandsUp;
	float TimerDelay;
	float TimerDeath;
	PlayerInput pmaster;
	enum Action {Walk, Stop, HandsUp, Panic, None};
	Action nextAction;
	bool willJump;

	float walkSpeed = 2.0f;
	float runSpeed = 5.0f;
	float tick = 0.01f;
	float HandsUpMax = 8f;


	// Use this for initialization
	void Start () {
		TimerDelay = 0f;
		TimerDeath = 10f; // duration of animation + extra time
		TimerHandsUp = HandsUpMax;
		bDying = false;
		speed = walkSpeed;
		nextAction = Action.Walk;
		angle = Random.Range(0.0f,Mathf.PI*2);
	}

	// still performing the most recent instruction
	void UpdateDelayed()
	{
		TimerDelay -= tick;
		//UpdatePosition(); // << maybe this should be at the end of the function?
		switch (nextAction) {
		case Action.Panic:
			speed = runSpeed;
			break;
		case Action.HandsUp:
			speed = 0;
			// Hands-up animation
			break;
		case Action.Stop:
			speed = 0;
			break;
		case Action.Walk:
			speed = walkSpeed;
			break;
		}
		if (willJump){
			//Jump();
			willJump=false;
		}
	}


	// Update is called once per frame
	void Update () {
		float playerdist;
		nextAction = Action.None;

		if (bDying)
		{
			if (TimerDeath > 0){
				//Animate();
				TimerDeath -= tick;
			}
			else
			{
				;; //delete
			}
			return;
		}
		else if (TimerHandsUp < 0)
		{
			TimerDelay= 0; // wait for next instruction
		}
		else
		{
			TimerHandsUp -= tick;
		}

		if (TimerDelay>0 || (!willJump && nextAction == Action.None))
		{
			UpdateDelayed();
			return;
		}
		else if (false/*AIManager.GetInstance().ShouldPanic*/)
		{
			nextAction = Action.Panic;
			return;
		}
		else if (pmaster.StartedHandsUp || pmaster.StartedShiv)
		{
			nextAction = Action.HandsUp;
			TimerHandsUp  = 2.0f;
		}
		else if (pmaster.EndedMove)
		{
			nextAction = Action.Stop;
		}
		else if (pmaster.StartedMove)
		{
			nextAction = Action.Walk;
		}
		// no ending "else" condition
		if (false /*pmaster.hasJumped*/) // or is in air?
		{
			willJump = true;
		}
		else
		{
			willJump = false;
		}

		// pmaster = Closest player
		playerdist = (this.transform.position-pmaster.transform.position).magnitude;
		TimerDelay = playerdist * 0.3f; // some constant
	}
}
