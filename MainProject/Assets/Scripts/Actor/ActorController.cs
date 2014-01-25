using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IActorControllerListener
{
	void OnActorMove(Vector3 forceVec);
}

public class ActorController : MonoBehaviour {
	//Movement members
	public float SpeedMax;
	public float ForceMax;
	public float MoveForce;
	public float JumpForceFactor;
	public float JumpMaxAltitude;	
	public float SpeedFloor; //minimum speed the actor can be moving before the actor stops
	private Vector3 forceVec;
	private float speedSqr; //speed of actor at current frame squared; see docs for why we're storing this value
	private float availableForceSqr; //force available for actor to use this frame
	
	//Orientation members
	public bool AllowPitch = true;
	public bool AllowYaw = true;
	public bool AllowRoll = true;
	private Vector3 orientation;
	private Vector3 rotMask;
	
	//sensor members
	public float MaxLockOnDistance = 1000.0f;
	private HealthInfo health;
	private AnimationProcessor processor;
	
	//collision members
	private bool grounded = true;
	private Vector3 groundNormal;
	private const float GROUNDED_NORMAL_MAG_MIN = 0.01f;
	
	//effect managers
	private List<IActorControllerListener> effectListeners;
	
	//animation members
	private AnimationProcessor modelAnims;
	
	public Vector3 Orientation
	{
		get { return orientation; }
	}
	
	public float Pitch
	{
		get { return orientation.x; }
	}
	
	public float Yaw
	{
		get { return orientation.y; }
	}
	
	public float Roll
	{
		get { return orientation.z; }
	}
	
	public bool IsAlive
	{
		get { return health.IsAlive; }
	}
	
	public bool IsPlayingAnimation
	{
		get { return (processor != null) ? processor.IsPlaying : false; }
	}
	
	public float Health
	{
		get { return health.Health; }
	}
	
	public HealthInfo HealthInfo
	{
		get { return (health != null) ? health : GetComponent<HealthInfo>(); }
	}
	
	public bool IsGrounded
	{
		get { return grounded; }
	}
	
	public void AttachListener(IActorControllerListener listener)
	{
		if(!effectListeners.Contains(listener))
		{
			effectListeners.Add(listener);
		}
	}
	
	public void RemoveListener(IActorControllerListener listener)
	{
		effectListeners.Remove(listener);
	}
	
	// Use this for initialization
	void Start () {
		//init listener list
		effectListeners = new List<IActorControllerListener>();
		//init movement
		forceVec = new Vector3();
		health = GetComponent<HealthInfo>();
		//init orientation properties
		orientation = transform.localRotation.eulerAngles;
		rotMask = new Vector3(	AllowPitch ? 1 : 0,
								AllowYaw ? 1 : 0,
								AllowRoll ? 1 : 0);
		//init weapons
		//get our launch points for our weapons
		LaunchPointProperties = GetComponentInChildren<WeaponLaunchPoints>();
		if(LaunchPointProperties != null)
		{
			Debug.Log("loading launch properties");
			missileLaunchPoints = LaunchPointProperties.MissileLaunchPoints;
			bulletLaunchPoints = LaunchPointProperties.BulletLaunchPoints;
			missileAimingHeadings = new Quaternion[missileLaunchPoints.Count];
			bulletAimingHeadings = new Quaternion[bulletLaunchPoints.Count];
		}
		//have to instance the launchers, so that fire rate and ammo works
		if(MissileLauncher != null)
		{
			msslLaunchInstance = ((GameObject)Instantiate(MissileLauncher.gameObject)).GetComponent<Launcher>();
			MissileLauncher = msslLaunchInstance.GetComponent<Launcher>();
		}
		if(Cannon != null)
		{
			cannonInstance = ((GameObject)Instantiate(Cannon.gameObject)).GetComponent<Launcher>();
			Cannon = cannonInstance.GetComponent<Launcher>();
		}
		//init animation manager
		processor = GetComponentInChildren<AnimationProcessor>();
		if(processor != null)
		{
			//attach the weapons to the model so they can broadcast animation information
			Debug.Log("found animation processor");
			if(msslLaunchInstance != null)
			{
				msslLaunchInstance.transform.parent = processor.transform;
			}
			if(cannonInstance != null)
			{
				cannonInstance.transform.parent = processor.transform;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	void FixedUpdate()
	{
		//handle collision updates
		//reset collision normal
		groundNormal = Vector3.zero;
		if(grounded && !checkIsGrounded())
		{
			grounded = false;
		}
		if(!grounded && checkIsGrounded())
		{
			grounded = true;
		}
		//handle movement
		//if we're moving really slowly, just halt so we don't appear to be slipping
		if(speedSqr < Mathf.Pow(SpeedFloor, 2))
		{
			rigidbody.velocity = new Vector3(0.0f, rigidbody.velocity.y, 0.0f);
		}
		foreach(IActorControllerListener listener in effectListeners)
		{
			listener.OnActorMove(forceVec);
		}
		//update model's bones as needed
		processor.UpdateModelBones(orientation);
		//if we're dead, play a death animation
		if(!IsAlive)
		{
			processor.PlayDeathAnimation();
			//Once that's done, explode
			//NOTE TO SELF: totally need an event system for this
			if(!IsPlayingAnimation)
			{
				if(Explosion != null && !exploded)
				{
					Instantiate(Explosion, transform.position, Quaternion.identity);
					//Destroy (gameObject);
					exploded = true;
				}
			}
		}
	}
	
	//Following are the commands you can issue to an actor.
	//Honestly, I should change the design so these just add commands to some queue
	//which would then be executed by command handlers after a SINGLE (not per-call) test for actor being alive...
	//But then again, it's 5/3, a little late for that. Next time for sure.
	//Thrusts shiv directly in front of you
	public void Shiv()
	{}
	
	//orientation modifying functions.
	public void Move(Vector3 direction)
	{
		if(IsAlive)
		{
			//update physical properties
			forceVec = Vector3.zero;
			speedSqr = rigidbody.velocity.sqrMagnitude;
			availableForceSqr = Mathf.Pow(ForceMax, 2);
			float altitude = transform.position.y;
			
			//compose the velocity vector first
			//...a little easier than I thought it'd be
			addForce(new Vector3(direction.x * MoveForce, 
			                     (altitude < JumpMaxAltitude) ? (direction.y * MoveForce * JumpForceFactor) : 0.0f,
			                     direction.z * MoveForce));
			
			//and apply the force
			rigidbody.AddRelativeForce(forceVec);
		}
	}
	
	public void Rotate(Vector3 eulerAngles)
	{
		if(IsAlive)
		{
			transform.Rotate(eulerAngles.x * rotMask.x, eulerAngles.y * rotMask.y, eulerAngles.z * rotMask.z);
			orientation += eulerAngles;
		}
	}
	
	public void RotateToOrientation(Quaternion newHeading)
	{
		if(IsAlive)
		{
			Vector3 newHedEuler = newHeading.eulerAngles;
			Rotate(newHedEuler - orientation);
			//transform.rotation = newHeading;
			//orientation = newHeading.eulerAngles;
		}
	}
	
	public void OnCollisionEnter(Collision coll)
	{
		//eh, fuck it
		groundNormal = coll.contacts[0].normal;
		grounded = checkIsGrounded();
	}
	
	//add a MoveTo method?
	
	private void addForce(Vector3 force)
	{
		Vector3 finalForce = force;
		
		//can we still move this frame?
		if(ForceMax >= 0.0f)
		{
			if(availableForceSqr > 0.0f)
			{
				if(availableForceSqr < force.sqrMagnitude)
				{
					//rescale vector to remaining force
					finalForce = finalForce.normalized * Mathf.Sqrt(availableForceSqr);
				}
				availableForceSqr -= force.sqrMagnitude;
				forceVec += finalForce;
			}
		}
		else
		{
			forceVec += finalForce;
		}
		
		//if we're moving at our speed cap, rescale our force
		if(SpeedMax >= 0.0f && speedSqr >= Mathf.Pow(SpeedMax, 2))
		{
			rigidbody.velocity = rigidbody.velocity.normalized * SpeedMax;
			//return;
		}
	}
	
	private bool checkIsGrounded()
	{
		return groundNormal.y > GROUNDED_NORMAL_MAG_MIN;
	}
}
