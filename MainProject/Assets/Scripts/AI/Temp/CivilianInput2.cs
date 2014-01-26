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
    enum Action { Walk, Stop, HandsUp, Panic, None };
    Action nextAction;
    bool willJump;
    
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
    public float HardTurnThreshold = 0.1f;
    public float HardTurnAngle = 90.0f;
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
        angle = 0;//Random.Range(0.0f,Mathf.PI*2);
        //initialize to a random initial heading
        ActorCtrl.RotateY(Random.Range(0.0f, 360.0f));
        currWanderAngle = Random.Range(0.0f, 8000.0f);
	}

    private float randomNormVal()
    {
        float result = (Mathf.PerlinNoise(currWanderAngle, currWanderAngle) * 2.0f) - 1.0f;
        return result;
    }

    private Vector3 getWanderPosition()
    {
        //displace in front of the civilian
        Vector3 result = new Vector3(   randomNormVal() * MaxWanderJitter,
                                        0.0f,
                                        randomNormVal() * MaxWanderJitter);
        result = result.normalized * WanderRadius;
        //(Vector3.forward * WanderDistance);
        //now displace in front of the civilian
        result += WanderDistance * (transform.rotation * Vector3.forward);
        return result;
    }

	private void updatePosition()
	{
		// if above ground -> y position -= gravity * dt ??? (physics handles this)
		// move
        Vector3 wanderPos = getWanderPosition();
        angle = ((Mathf.Acos(Vector3.Dot(wanderPos.normalized, ActorCtrl.Forward)) / (Mathf.PI)) - 0.5f) * 10.0f;
        //angle = Mathf.Clamp(angle, -180.0f, 180.0f);
        //get the current velocity; if it's too slow, make a sharp turn
        if (ActorCtrl.SqrVelocity < HardTurnThreshold)
        {
            angle = Mathf.Sign(angle) * HardTurnAngle; 
        }
        ActorCtrl.RotateY(angle);
        Vector3 strafeJitter = Vector3.right * Random.Range(-1.0f, 1.0f);
        Vector3 finalMove = (Vector3.forward + strafeJitter).normalized;
        ActorCtrl.Move(finalMove);//getWanderPosition());
	}

    public void SetMasterPlayer(PlayerInput P)
    {
        masterPlayer = P;
    }

    // still performing the most recent instruction
    void UpdateDelayed()
    {
        TimerDelay -= tick;
        switch (nextAction)
        {
            case Action.Panic:
                ActorCtrl.SpeedMax = runSpeed;
                break;
            case Action.HandsUp:
                ActorCtrl.SpeedMax = 0;
                // Hands-up animation
                break;
            case Action.Stop:
                ActorCtrl.SpeedMax = 0;
                break;
            case Action.Walk:
                ActorCtrl.SpeedMax = walkSpeed;
                break;
        }
        if (willJump)
        {
            ActorCtrl.Jump();
            willJump = false;
        }
        updatePosition();
    }


    // Update is called once per frame
    void Update()
    {
        currWanderAngle += Time.deltaTime;
        float playerdist;
        nextAction = Action.None;

        if (bDying)
        {
            if (TimerDeath > 0)
            {
                TimerDeath -= tick;
            }
            else
            {
                ; ; //delete
            }
            return;
        }
        else if (TimerHandsUp < 0)
        {
            TimerDelay = 0; // wait for next instruction
        }
        else
        {
            TimerHandsUp -= tick;
        }

        if (TimerDelay > 0 || (!willJump && nextAction == Action.None))
        {
            UpdateDelayed();
            return;
        }
        else if (AIManager.GetInstance().ShouldPanic)
        {
            nextAction = Action.Panic;
            return;
        }
        else if (masterPlayer.StartedHandsUp || masterPlayer.StartedShiv)
        {
            nextAction = Action.HandsUp;
            TimerHandsUp = 2.0f;
        }
        else if (masterPlayer.EndedMove)
        {
            nextAction = Action.Stop;
        }
        else if (masterPlayer.StartedMove)
        {
            nextAction = Action.Walk;
        }
        // no ending "else" condition
        if (masterPlayer.StartedJump) // or is in air?
        {
            willJump = true;
        }
        else
        {
            willJump = false;
        }

        SetMasterPlayer(aiManager.GetNearestMaster(ActorCtrl.transform.position));
        if (masterPlayer != null)
        {
            Debug.Log("Got master player!");
        }
        playerdist = (this.transform.position - masterPlayer.transform.position).magnitude;
        TimerDelay = playerdist * 0.3f; // some constant
    }
}
