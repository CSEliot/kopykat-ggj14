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
	
	//Weapon input members
	private ActorController actorCtrl;
	public float AlignmentDistance = 10000; //the distance from the screen that dumbfire weapons should be aligned for
	private float defaultAlignDist;
	private HealthInfo health;
	
	//Ejection system members
	private PlayerLogic logic;
	
	private Rigidbody actorRB;
	
	//debug command members
	public SpawnPoint EnemySpawner = null;
	
	#region Properties
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
		actorCamera = logic.CameraRig;
		audioDev = actorCamera.GetComponentInChildren<AudioListener>();
		ReloadActor();
		defaultAlignDist = AlignmentDistance;
		//we don't want the friggin' mouse moving around!
		Screen.lockCursor = true;
	}
	
	// Update is called once per frame
	void Update () {
		//next, handle debug mode commands and menu if necessary
		//must be called outside FixedUpdate to run while the game is paused
		updateDebugInput();
		updateMenuInput();
	}
	
	void FixedUpdate()
	{
		updateMouseLook();
		
		updateRaycast();
		//we can't do movement or weapon control if we're dead
		health = actor.GetComponent<HealthInfo>();
		if(health.IsAlive)
		{
			updateMovementInput();
			updateTargetingInput();
			updateWeaponInput();
			//handle ejections and mech boarding
			if(Input.GetButtonDown("Eject"))
			{
				logic.Eject();
				ResetLockOnTarget();
			}
			if(Input.GetButtonDown("BoardMech") && lockOnTarget != null)
			{
				logic.SetActorAndSeat(lockOnTarget.gameObject, 0);
				ResetLockOnTarget();
			}
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
	
	protected void updateRaycast () {
		//always do a raycast
		if(actorCamera != null)
		{
			Ray cast = actorCamera.ControlledCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
			RaycastHit hit = new RaycastHit(); //Raycast test will store an hit information here
			if(Physics.Raycast(cast, out hit) && hit.distance <= actorCtrl.MaxLockOnDistance)
			{
				//tags are on parents of collider objects, so search up the hit's parent tree for a tagged object
				lockOnTarget = ParentSearches.FindParentWithTag(hit.transform, "LockOnTarget");
				if(lockOnTarget != logic.Actor.transform)
				{
					AlignmentDistance = hit.distance;
				}
				else
				{
					AlignmentDistance = defaultAlignDist;
					lockOnTarget = null;
				}
				if(lockOnTarget != null)
				{
					missileTarget = lockOnTarget;
				}
				//Debug.Log("TESTING lockONTarget: "+lockOnTarget.position);
				//Debug.Log ("Have a target at distance " + hit.distance);
			}
			else
			{
				//Debug.Log ("No target");
			}
		}
		else
		{
			Debug.Log ("No CameraController attached!");
		}
	}
	
	protected void updateMouseLook()
	{
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
			actorCtrl.Rotate(rotVec);//new Vector3(0.0f, rotVec.y, 0.0f));//rotVec);
		}
		else
		{
			actor.transform.Rotate(0.0f, rotVec.y, 0.0f);
		}
		//Debug.Log ("new pitch: " + actorCtrl.Pitch.ToString());
	}
	
	protected void updateMovementInput()
	{
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
		
		//and then give the movement order to the actor
		actorCtrl.Move(actorCamera.transform.localRotation * moveVector);
		
		//actorCtrl.Rotate(new Vector3(moveVector.x, moveVector.y, 0.0f));
	}
	
	protected void updateTargetingInput()
	{
		//Debug.Log ("locking!");
		//Debug.Log("locked on to " + lockOnTarget);
		if(Input.GetButtonDown("LockTarget"))
		{
			//Debug.Log ("locking!");
			if(lockOnTarget != null)
			{
				//Debug.Log("locked on to " + lockOnTarget);
				missileTarget = lockOnTarget;
			}
		}
		if(Input.GetButtonDown("GlobalRetarget"))
		{
			Transform target = missileTarget != null ? missileTarget : lockOnTarget;
			if(target != null)
			{
				List<GameObject> mssls = OrdinanceManager.GetMissilesOfLauncher(actorCtrl.gameObject);
				//Debug.Log("player has " + mssls.Count + " missiles");
				foreach(GameObject mssl in mssls)
				{
					OrdinanceManager.ChangeMissileTarget(mssl, target.gameObject);
				}
			}
		}
	}
	
	protected void updateWeaponInput()
	{	
		Vector3 alignmentPoint = actorCamera.ControlledCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, AlignmentDistance));
		//missiles can only be fired the moment the trigger is pressed
		if(Input.GetButtonDown("FireMissile"))
		{
			Transform target = missileTarget != null ? missileTarget : lockOnTarget;
			actorCtrl.FireMissile(target, alignmentPoint);
		}
		
		//guns can fire continuously, so fire whenever the trigger's held down
		if(Input.GetButton("FireBullet"))
		{
			actorCtrl.FireGuns(alignmentPoint);
		}
	}
	
	void updateMenuInput()
	{
		//handle pausing the game
		if(Input.GetButtonDown("PauseGame"))
		{
			GameSystem.ToggleGamePaused();
		}
	}
	
	void updateDebugInput ()
	{
		if(Input.GetButtonDown("ToggleAI"))
		{
			AIManager.GetInstance().ToggleAgentsEnabled();
		}
		
		if(Input.GetButtonDown("SpawnEnemy"))
		{
			//have we been passed an enemy spawner?
			if(EnemySpawner != null)
			{
				EnemySpawner.Spawn();
			}
		}
	}
}
