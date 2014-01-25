using UnityEngine;
using System.Collections;

public class CivilianInput2 : MonoBehaviour {

	bool bInAir;
	bool bHandsUp;
	bool bDying;
	float speed;
	float angle;
	float TimerHandsUp;
	float TimerDelay;
	float TimerDeath;
	PlayerInput pmaster;

	float walkSpeed = 2.0f;
	float runSpeed = 5.0f;
	float tick = 0.01f;
	float HandsUpMax = 8f;


	// Use this for initialization
	void Start () {
		TimerDelay = -1f;
		TimerDeath = 10f; // duration of animation + extra time
		TimerHandsUp = HandsUpMax;
		bDying = false;
		bInAir = false;
		bHandsUp = false;
		speed = walkSpeed;
		angle = Random.Range(0.0f,Mathf.PI*2);
	}

	void UpdatePosition()
	{
		if (speed > 0)
		{
			angle += Random.Range (-0.02f,0.02f);
		}
		// if above ground -> y position -= gravity * dt ???
		// move
	}

	// Update is called once per frame
	void Update () {
		float playerdist;

		if (bDying)
		{
			if (TimerDeath > 0){
				Animate();
				TimerDeath -= tick;
			}
			else
			{
				;; //delete
			}
			return;
		}
		if (TimerHandsUp < 0)
		{
			bHandsUp = false;
			TimerHandsUp = HandsUpMax;
		}
		// Flowchart...
		if (bIsInAir){
			UpdatePosition();
			return;
		}
		else if (TimerDelay >0)
		{
			TimerDelay -= tick;
			UpdatePosition();
			return;
		}
		else if (AIManager.GetInstance().ShouldPanic)
		{
			speed = runSpeed;
			UpdatePosition();
			return;
		}
		else if (bHandsUp)
		{
			speed = 0;
			TimerHandsUp -= tick;
			return;
		}
		// pmaster = Closest player
		playerdist = new Vector3(this.transform.position-pmaster.transform.position).magnitude;
		TimerDelay = playerdist * 0.3f; // some constant
		imitateMaster();
		/*
		if (pmaster.StartedShiv || pmaster.StartedHandsUp)
		{
			speed = 0;
			TimerHandsUp = 1.5f;
			return;
		}
		else if (pmaster.EndedMove)
		{
			speed = 0;
		}
		else if (pmaster.StartedMove)
		{
			speed = walkSpeed;
		}
		if (pmaster.StartedJump)
		{
			Jump ();
		}
		*/
		UpdatePosition();
	}
}
