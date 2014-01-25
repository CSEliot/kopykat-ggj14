using UnityEngine;
using System.Collections;

public class Ordinance : MonoBehaviour {
	
	public enum OrdinanceType { Seeker, Dumbfire };
	public OrdinanceType Type;
	public GameObject Target = null;
	private GameObject launcher = null;
	public float MaxSpeed = 0.0f;
	public float Thrust = 0.0f;
	public float SeekSpeed = 0.0f;
	
	//time in seconds this instance can remain in the game
	//-1 = infinite
	public float Lifetime = -1;
	
	private float timeAlive = 0;
	
	public float Damage = 0.0f;
	
	public GameObject ExplosionEmitter;
	
	public GameObject Launcher
	{
		get { return launcher; }
		set { launcher = value; }
	}
	
	// Use this for initialization
	void Start () {
		//for now, we'll emulate the Blender build's missile behavior.
		//Lockon will be handled in a player script, and will provide the
		//target for this script.
		
		//missiles in the old build had a constant forward velocity
		//rigidbody.velocity = (transform.forward * Speed);// + (transform.up * 0.5f * Speed);
		switch(Type)
		{
			case OrdinanceType.Seeker:
			{
				rigidbody.constantForce.relativeForce = Vector3.forward * Thrust;
				rigidbody.AddRelativeForce(Vector3.forward * Thrust * 35);
				rigidbody.AddRelativeForce(Vector3.up * Thrust * 4);
				break;
			}
			case OrdinanceType.Dumbfire:
			{
				rigidbody.velocity = transform.forward * MaxSpeed;
				if(rigidbody.constantForce != null)
				{
					rigidbody.constantForce.relativeForce = Vector3.forward * Thrust;
				}
				break;
			}
			default:
			{
				Debug.Log ("Ordinance: " + this.name + "'s OrdinanceType isn't set!");
				break;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	void FixedUpdate()
	{
		//for now, we'll emulate the Blender build's missile behavior.
		//Lockon will be handled in a player script, and will provide the
		//target for this script.
		
		//if there's a target, seek towards it by adding a force leading
		//towards the target.
		switch(Type)
		{
			case OrdinanceType.Seeker:
			{
				if(Target != null)
				{
					Vector3 seekForce = (Target.transform.position - rigidbody.position) * SeekSpeed;
					//we also want to turn the missile, so we need to get the perpendicular component of the force
					//Vector3 seekTorque = seekForce;//Vector3.Dot(seekForce, Vector3.up) * Vector3.up;
					Quaternion newHeading = Quaternion.LookRotation(seekForce);
					rigidbody.rotation = Quaternion.Slerp(rigidbody.rotation, newHeading, SeekSpeed);
					//rigidbody.velocity = transform.forward * Speed;
					//rigidbody.AddRelativeForce(seekForce);
				}
				//rescale our velocity if the added force exceeded our speed cap
				if(MaxSpeed > 0.0f && rigidbody.velocity.sqrMagnitude > Mathf.Pow(MaxSpeed, 2))
				{
					rigidbody.velocity = rigidbody.velocity.normalized * MaxSpeed;
				}
				break;
			}
			case OrdinanceType.Dumbfire:
			{
				//dumbfire weapons don't have changing forces, do nothing
				break;
			}
			default:
			{
				break;
			}
		}
		
		timeAlive += Time.deltaTime;
		if(timeAlive >= Lifetime)
		{
			OnDeath();
		}
	}
	
	void OnTriggerEnter(Collider other)
	{
		//TODO: spawn dust flare & explosion
		//inflict damage if possible
		HealthInfo hitHealth = ParentSearches.GetComponentInParents<HealthInfo>(other.gameObject);
		if(hitHealth != null)
		{
			hitHealth.ApplyDamage(Damage, launcher);
		}
		OnDeath();
	}
	
	//mostly just generating the ordinance's explosion effect
	void OnDeath()
	{
		if(ExplosionEmitter != null)
		{
			Instantiate(ExplosionEmitter, transform.position, Quaternion.identity);
		}
		OrdinanceManager.DestroyRound(gameObject);
	}
}
