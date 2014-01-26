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
	PlayerInput masterPlayer;
    private AIManager aiManager;
    public ActorController ActorCtrl;
    public enum State { Wander, Walk, Idle, Panic, Dead, HandsUp };
    private State aiState;
    
	float walkSpeed = 2.0f;
	float runSpeed = 5.0f;
	float tick = 0.01f;
	float HandsUpMax = 8f;

    //wander behavior
    //wandering works by having the actor move to a point on
    //a circle placed ahead of the actor.
    public float WanderRadius = 1.0f;
    public float WanderDistance = 1.0f;
    public float MaxWanderJitter = 2.0f;
    private float currWanderAngle = 0;

	// Use this for initialization
	void Start () {
        aiManager = AIManager.GetInstance();
		TimerDelay = -1f;
		TimerDeath = 10f; // duration of animation + extra time
		TimerHandsUp = HandsUpMax;
		bDying = false;
		bInAir = false;
		bHandsUp = false;
		speed = walkSpeed;
		angle = Random.Range(0.0f,Mathf.PI*2);
	}

    private Vector3 getWanderPosition()
    {
        //displace in front of the civilian
        Vector3 result = ActorCtrl.Forward * WanderDistance;
        //now displace in a circle around the forward point
        result += new Vector3(  Mathf.Cos(currWanderAngle) * WanderRadius,
                                0,
                                Mathf.Sin(currWanderAngle) * WanderRadius);
        return result;
    }

	private void updatePosition()
	{
		// if above ground -> y position -= gravity * dt ??? (physics handles this)
		// move
        Vector3 wanderPos = getWanderPosition();
        angle = Mathf.Acos(Vector3.Dot((wanderPos - ActorCtrl.transform.position).normalized, ActorCtrl.Forward.normalized));
        //angle = ActorCtrl.transform.rotation.eulerAngles.y;
        ActorCtrl.RotateY(angle);
        Debug.Log(angle);
        ActorCtrl.Move(getWanderPosition());
	}

    public void SetMasterPlayer(PlayerInput P)
    {
        masterPlayer = P;
    }

    private void imitateMaster()
    {
        if (aiState == State.Walk || aiState == State.Idle)
        {
            if (masterPlayer.StartedMove)
            {
                aiState = State.Walk;
            }
            else
            {
                aiState = State.Idle;
            }
            if (masterPlayer.StartedJump)
            {
                
            }
            if (masterPlayer.StartedHandsUp || masterPlayer.StartedShiv)
            {
                aiState = State.HandsUp;
            }
        }
    }

	// Update is called once per frame
	void Update () {
        currWanderAngle += Time.deltaTime;//Random.Range(-MaxWanderJitter, MaxWanderJitter);
		float playerdist;

		if (TimerHandsUp < 0)
		{
			bHandsUp = false;
			TimerHandsUp = HandsUpMax;
		}
		// Flowchart...
		else if (TimerDelay >0)
		{
			TimerDelay -= tick;
			updatePosition();
			return;
		}
		else if (aiManager.ShouldPanic)
		{
			speed = runSpeed;
			updatePosition();
			return;
		}
		else if (bHandsUp)
		{
			speed = 0;
			TimerHandsUp -= tick;
			return;
		}
		// pmaster = Closest player
        SetMasterPlayer(aiManager.GetNearestMaster(transform.position));
		playerdist = (this.transform.position - masterPlayer.transform.position).magnitude;
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
		updatePosition();
	}
}
