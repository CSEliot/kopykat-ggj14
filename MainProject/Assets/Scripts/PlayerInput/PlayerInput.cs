using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInput : MonoBehaviour {
	
	//Mouselook members
	//this is a major member, and other vars are defined by it
	//should only be changed through SetActor, but don't know how to do that yet
	private GameObject actor = null;
	public float CameraSensitivity;
	public float MaxYawRate; //move this to TaurusActor?
	
	//audio members
	private AudioListener audioDev = null;
	
	//Lockon members
	private CameraController actorCamera = null;
	private Transform lockOnTarget = null;
	private Transform missileTarget = null;

	//connected subsystems
	private ActorController actorCtrl;
	private HealthInfo health;
	private PlayerLogic logic;
	
	private Rigidbody actorRB;
	
	//command members
	public bool movedCurr, movedPrev;
	public bool rotatedCurr, rotatedPrev;
	public bool shivCurr, shivPrev;
	public bool handsUpCurr, handsUpPrev;
	public List<ActorCommand> CommandsThisFrame { get; }
	
	#region Properties
	public bool StartedMove
	{
		get { return movedCurr && (!movedPrev); }
	}

	public bool EndedMove
	{
		get { return (!movedCurr) && movedPrev; }
	}

	public bool StartedRotate
	{
		get { return rotatedCurr && (!rotatedPrev); }
	}
	
	public bool EndedRotate
	{
		get { return (!rotatedCurr) && rotatedPrev; }
	}

	public bool StartedShiv
	{
		get { return shivCurr && (!shivPrev); }
	}
	
	public bool EndedShiv
	{
		get { return (!shivCurr) && shivPrev; }
	}

	public bool StartedHandsUp
	{
		get { return handsUpCurr && (!handsUpPrev); }
	}
	
	public bool EndedHandsUp
	{
		get { return (!handsUpCurr) && handsUpPrev; }
	}

	//[ExposeProperty]
	public Transform LockOnTarget
	{
		get { return lockOnTarget; }
	}
	//should really rename these two members
	public Transform MissileTarget
	{
		get { return missileTarget; }
	}
	#endregion
	
	public void ResetLockOnTarget()
	{
		lockOnTarget = null;
	}
	
	// Use this for initialization
	void Start () {
		//setup the camera
		//actorCamera = GetComponentInChildren<Camera>();
		logic = GetComponent<PlayerLogic>();
		//if we haven't been passed a rigidbody, lookup one in our children
		
		//actorRB = actorPhysics.rigidbody;
		//initWeapons();
		//setActorAndSeat(actor, seatNum);
		movedCurr = false;
		movedPrev = false;
		shivCurr = false;
		shivPrev = false;
		handsUpCurr = false;
		handsUpPrev = false;
		rotatedCurr = false;
		rotatedPrev = false;
		actorCamera = logic.CameraRig;
		audioDev = actorCamera.GetComponentInChildren<AudioListener>();
		ReloadActor();
		//add ourselves to the AI system
		AIManager.GetInstance().AddCommander(this);

		//we don't want the friggin' mouse moving around!
		Screen.lockCursor = true;
	}
	
	// Update is called once per frame
	void Update () {}
	
	void FixedUpdate()
	{
		//clear command queue for this frame
		CommandsThisFrame.Clear();
		updateMouseLook();

		//we can't do movement or weapon control if we're dead
		health = actor.GetComponent<HealthInfo>();
		if(health.IsAlive)
		{
			updateMovementInput();
			updateShivInput();
			updateHandsUpInput();
		}
		else
		{
			GameSystem.State = GameSystem.GameState.Dead;
		}
	}
	
	public void ReloadActor()
	{
		//if the logic script reports that we've got a new actor, continue
		if(logic.Actor != null)
		{
			actor = logic.Actor;
			//get the weapons on the actor
			actorCtrl = logic.Actor.GetComponent<ActorController>();
			//and get the actor's rigidbody
			actorRB = logic.Actor.rigidbody;
		}
		//if we didn't attach to a new actor, something's gone wrong
		Debug.Log("PlayerInput: can't find new actor!");
	}
	
	protected void updateMouseLook()
	{
		rotatedPrev = rotatedCurr;
		//follow our actor
		
		//build a rotation vector from the mouse axis.
		Vector3 rotVec = new Vector3(-Input.GetAxis("Mouse Y") * CameraSensitivity,
		                             Input.GetAxis("Mouse X") * CameraSensitivity * (1.0f - actorRB.angularDrag),
		                             0.0f);
		//The Y component applies to the physics object; X applies to the camera to create tilt
		actorCamera.PitchCamera(rotVec.x);
		if(false)//!actorCtrl.IsInfantry)
		{
			actorCamera.YawCamera(rotVec.y);
			if(MaxYawRate >= 0.0f)
			{
				//actor.transform.Rotate(0.0f, Mathf.Clamp(rotVec.y, -MaxYawRate, MaxYawRate), 0.0f);
				//actorCtrl.Rotate(
				rotVec.y = Mathf.Clamp(rotVec.y, -MaxYawRate, MaxYawRate);
			}
			//else
			//{
				//actor.transform.Rotate(0.0f, rotVec.y, 0.0f);
			//}
			//new Vector3(0.0f, rotVec.y, 0.0f));//rotVec);
		}
		else
		{
			rotVec = new Vector3(0.0f, rotVec.y, 0.0f);
		}
		//rotate the CAMERA, not the PLAYER.
		//actorCtrl.Rotate(rotVec);
		//add command to queue
		rotatedCurr = true;
	}
	
	protected void updateMovementInput()
	{
		movedPrev = movedCurr;
		//compose the velocity vector first
		//...a little easier than I thought it'd be
		Vector3 moveVector = new Vector3(Input.GetAxis("Horizontal"), 
		                     			Input.GetAxis("Jump"),
		                     			Input.GetAxis("Vertical"));
		//if needed, renormalize the movement vector
		if(moveVector.sqrMagnitude > 1.0f)
		{
			moveVector.Normalize();
		}

		moveVector = actorCamera.transform.localRotation * moveVector;

		//and then give the movement order to the actor
		actorCtrl.Move(moveVector);
		//also add movement command to command queue
		if (moveVector.sqrMagnitude > 0.0f)
		{
			movedCurr = true;
		}
		else
		{
			movedCurr = false;
		}
	}
	
	protected void updateShivInput()
	{
		shivPrev = shivCurr;
	}

	protected void updateHandsUpInput()
	{
		handsUpPrev = handsUpCurr;
	}
}
