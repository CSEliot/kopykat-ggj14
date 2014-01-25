using UnityEngine;
using System.Collections;

/// <summary>
/// Base class for all launchers.
/// </summary>
public class Launcher : MonoBehaviour {
	
	public enum LauncherType { Missile, Bullet };
	
	public LauncherType Type;
	public AudioClip LaunchSound;
	//what the launcher fires
	public Ordinance LaunchObj;
	//which direction the launcher is angled, relative to the actor
	public Vector3 LaunchHeading = Vector3.zero;
	public Vector3 LaunchForce = Vector3.zero; //measured in newtons
	public int MaxAmmo;
	public float FireRate = 1.0f; //measured in seconds
	public float Precision;
	public int NumRoundsInBurst = 1; //how many rounds the launcher fires per firing
	//launcher modifiers
	public float VelocityMod = 1.0f;
	public float DamageMod = 1.0f;
	public int CurrentAmmo
	{
		get { return currAmmo; }
	}
	
	//protected Quaternion internalLaunchHeading;
	protected int currAmmo;
	protected float lastFiredTime = 0.0f;
	protected int roundsFired = 0;
	protected float internalPrecision;
	protected AnimationProcessor processor;
	
	// Use this for initialization
	void Start () {
		//internalLaunchHeading = Quaternion.Euler(LaunchHeading);
		//grab the parent's AnimationProcessor if possible
		processor = ParentSearches.GetComponentInParents<AnimationProcessor>(gameObject);
		internalPrecision = Mathf.Clamp(Precision, 0.0f, 1.0f);
		currAmmo = MaxAmmo;
		if(processor == null)
		{
			Debug.Log ("ERR: couldn't find AnimationProcessor in parent");
		}
		//Debug.Log("precision = " + internalPrecision.ToString());
	}
	
	// Update is called once per frame
	void Update () {
		lastFiredTime += Time.deltaTime;
	}
	
	/// <summary>
	/// Fires a round from the launcher, if the launcher still has ammo.
	/// </summary>
	/// <returns>
	/// An instance of the ordinance fired, or null if the launcher cannot fire.
	/// </returns>
	/// <param name='launchPoint'>
	/// Where ordinance will be launched.
	/// </param>
	/// <param name='actorHeading'>
	/// The actor's current heading.
	/// </param>
	/// <param name='target'>
	/// Launcher's target, if any.
	/// </param>
	public GameObject Fire(Vector3 launchPoint, Quaternion actorHeading, GameObject target, GameObject launcher)
	{
		//check for ammo and that the launcher actually fires anything (redundant?)
		if((currAmmo > 0 || MaxAmmo < 0) && (lastFiredTime > FireRate) && LaunchObj != null)
		{
			//play animation!
			processor.PlayFiringAnimation(Type);
			//reset the rounds fired count if we fired a full burst previously
			if(roundsFired >= NumRoundsInBurst)
			{
				roundsFired = 0;
			}
			//calculate jitter
			Vector2 jitterAngle = Random.insideUnitCircle * Mathf.PI * (1.0f - internalPrecision);
			Quaternion jitter = Quaternion.Euler(jitterAngle.x, jitterAngle.y, 0.0f);
			//generate a new round
			GameObject newRound = OrdinanceManager.AddRound(LaunchObj, launchPoint, actorHeading * jitter, target, launcher);//(GameObject)Instantiate(Ordinance, launchPoint, actorHeading * jitter);
			//Debug.Log ("Launcher: launcher pos = " + transform.position.ToString() + ", round pos = " + launchPoint.ToString());
			newRound.transform.Rotate(LaunchHeading);
			//apply weapon modifications (move this into init by copying prefab during init?)
			Ordinance roundProperties = newRound.GetComponent<Ordinance>();
			//WeaponDamage damageProperties = newRound.GetComponent<WeaponDamage>();
			roundProperties.MaxSpeed *= VelocityMod;
			roundProperties.Damage *= DamageMod;
			roundProperties.Launcher = launcher;
			//play the weapon's launching sound
			if(LaunchSound != null)
			{
				AudioSource.PlayClipAtPoint(LaunchSound, launchPoint);
			}
			//apply any launching forces
			newRound.rigidbody.AddRelativeForce(LaunchForce);
			//if this launcher fires missiles, assign the missile a target
			//Ordinance msslMov = newRound.GetComponent<Ordinance>();
			//if(roundProperties.Type == Ordinance.OrdinanceType.Seeker)
			//{
				roundProperties.Target = target;
			//}
			//reduce ammo
			if(MaxAmmo > 0)
			{
				currAmmo--;
			}
			//add a round to the burst fire counter
			roundsFired++;
			//if we've fired a full burst, reset the fire delay
			if(roundsFired >= NumRoundsInBurst)
			{
				lastFiredTime = 0.0f;
			}
			return newRound;
		}
		return null;
	}
	
	#region Fire Overloads
	public GameObject Fire(Vector3 launchPoint, GameObject launcher)
	{
		return Fire(launchPoint, Quaternion.identity, null, launcher);
	}
	public GameObject Fire(Vector3 launchPoint, Quaternion actorHeading, GameObject launcher)
	{
		return Fire(launchPoint, actorHeading, null, launcher);
	}
	
	public GameObject Fire(Vector3 launchPoint, Vector3 actorHeadingEuler, GameObject launcher)
	{
		return Fire(launchPoint, Quaternion.Euler(actorHeadingEuler), null, launcher);
	}
	
	public GameObject Fire(Vector3 launchPoint, Vector3 actorHeadingEuler, GameObject target, GameObject launcher)
	{
		return Fire(launchPoint, Quaternion.Euler(actorHeadingEuler), target, launcher);
	}
	#endregion
	
	/// <summary>
	/// Adds ammo to launcher.
	/// </summary>
	/// <returns>
	/// The number of rounds added to the launcher.
	/// </returns>
	public int AddAmmo(int count)
	{
		//don't think this should be able to *take* ammo...
		if(count > 0)
		{
			//we can add at most (MaxAmmo - currAmmo) rounds
			int ammoToAdd = Mathf.Min(MaxAmmo - currAmmo, count);
			currAmmo += ammoToAdd;
			return ammoToAdd;
		}
		return 0;
	}
}