using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimationProcessor : MonoBehaviour {
	
	public AnimationClip StrafeLeftAnim;
	public AnimationClip StrafeRightAnim;
	public AnimationClip WalkForwardAnim;
	public AnimationClip WalkBackwardAnim;
	public AnimationClip IdleAnim;
	public AnimationClip JumpAnim;
	public AnimationClip FallAnim;
	public AnimationClip FlyLeftAnim;
	public AnimationClip FlyRightAnim;
	public AnimationClip FlyForwardAnim;
	public AnimationClip FlyBackwardAnim;
	public AnimationClip MissileFireAnim;
	public AnimationClip GunFireAnim;
	public AnimationClip DeathAnim;
	public AudioClip WalkSound;
	private Dictionary<Launcher.LauncherType, AnimationClip> FiringAnims = new Dictionary<Launcher.LauncherType, AnimationClip>();
	public List<Transform> PitchDependentBones;
	public List<Transform> YawDependentBones;
	public List<Transform> RollDependentBones;
	public float MinAnimTriggerVel;
	
	private AudioSource actorSFX;
	private Rigidbody modelRB;
	private ActorController actorCtrl;
	private bool DeathAnimPlayed = false;
	private float maxWalkAltitude = 6.0f;
	
	Vector3 newOrient = Vector3.zero;
	
	public bool IsPlaying
	{
		get { return ((animation != null) ? animation.isPlaying : false); }
	}
	
	void Start () {
		//build firing anim dictionary
		FiringAnims[Launcher.LauncherType.Missile] = MissileFireAnim;
		FiringAnims[Launcher.LauncherType.Bullet] = GunFireAnim;
		
		//make sure all our animations loop...
		animation.wrapMode = WrapMode.Loop;
		//except the death animation
		DeathAnim.wrapMode = WrapMode.Once;
		//freeze any animations that might be playing
		animation.playAutomatically = false;
		animation.Stop();
		modelRB = ParentSearches.GetComponentInParents<Rigidbody>(gameObject);
		actorSFX = ParentSearches.GetComponentInParents<AudioSource>(gameObject);
		actorCtrl = ParentSearches.GetComponentInParents<ActorController>(gameObject);
		if(modelRB == null)
		{
			//this is bad, the model should be attached to an actor w/ rigidbody
			Debug.Log ("ERR: AnimationProcessing script couldn't find a rigidbody in actor " + gameObject.name);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(modelRB != null && !DeathAnimPlayed)
		{
			//the model should be under a rigidbody, so we can use its velocity to blend in animations
			//strafe uses the RB's x velocity
			//walk uses RB's z vel
			//any jumping anim would use the y velocity
			
			//build velocity weights
			Vector3 vel = modelRB.velocity;
			//get relative velocities along axis
			Vector3 relVel = new Vector3(Vector3.Dot(vel, modelRB.transform.right),
										 Vector3.Dot(vel, modelRB.transform.up),
										 Vector3.Dot(vel, modelRB.transform.forward));
			float sqrMag = vel.sqrMagnitude;
			float strafeWeight = 1.0f * Mathf.Abs(relVel.x);//Mathf.Pow(relVel.x, 2) / sqrMag;
			float walkWeight = 1.0f * Mathf.Abs(relVel.y);//Mathf.Pow(relVel.z, 2) / sqrMag;
			float jumpWeight = 1.0f * Mathf.Abs(relVel.z);// * (vel.y != 0.0f ? Mathf.Sign(vel.y) : 0.0f);//*(Mathf.Pow(vel.y, 2) / sqrMag);
			
			if(sqrMag >= Mathf.Pow (MinAnimTriggerVel, 2))
			{
				//don't let the mech flail around if it's flying in the air!
				//we'll need to find the ground's height and compare the mech's altitude against that
				Ray mechRay = new Ray(transform.position, Vector3.down); //we want the global down direction in case the mech's not oriented straight up for whatever reason
				RaycastHit hitInfo;
				int terrainLayerMask = 1 << 8; //"Terrain" layer is layer 8, we have to build a mask with only bit 8 active
				//if we're on the ground, handle walking movement
				//this check is problematic on the city map, something about how the cast length isn't very representative
				//alternate check is how CharacterMotor does it - store the hit's normal (or Vec3.Zero if no collision) and count as on ground if the normal's less than a specified delta
				bool isGrounded = !Physics.Raycast(mechRay, out hitInfo, Mathf.Infinity, terrainLayerMask) || hitInfo.distance < maxWalkAltitude || Mathf.Abs(vel.y) < MinAnimTriggerVel;
				if(isGrounded)
				{
					//now blend in animations
					if(strafeWeight != 0.0f)
					{
						animation.Blend((Mathf.Sign(relVel.x) > 0 ? StrafeRightAnim : StrafeLeftAnim).name, strafeWeight, 0.1f);
						//AudioSource.PlayClipAtPoint(WalkSound, transform.position);
						//animation.CrossFade((Mathf.Sign(relVel.x) > 0 ? StrafeRightAnim : StrafeLeftAnim).name, 3.0f, PlayMode.StopAll);
					}
					if(walkWeight != 0.0f)
					{
						animation.Blend((Mathf.Sign(relVel.z) > 0 ? WalkForwardAnim : WalkBackwardAnim).name, walkWeight, 0.1f);
						//AudioSource.PlayClipAtPoint(WalkSound, transform.position);
						//animation.CrossFade((Mathf.Sign(relVel.z) > 0 ? WalkForwardAnim : WalkBackwardAnim).name, 3.0f, PlayMode.StopAll);
					}
					//if we're not already playing the walking sound, play it now
					if(!actorSFX.isPlaying)
					{
						actorSFX.clip = WalkSound;
						actorSFX.Play();
					}
				}
				//otherwise, we must be in the air; handle air movement
				else
				{
					//stop any walking sounds
					if(actorSFX.isPlaying)
					{
						actorSFX.Stop();
					}
					//now blend in animations
					if(strafeWeight != 0.0f)
					{
						animation.Blend((Mathf.Sign(relVel.x) > 0 ? FlyRightAnim : FlyLeftAnim).name, strafeWeight, 0.1f);
						//AudioSource.PlayClipAtPoint(WalkSound, transform.position);
						//animation.CrossFade((Mathf.Sign(relVel.x) > 0 ? StrafeRightAnim : StrafeLeftAnim).name, 3.0f, PlayMode.StopAll);
					}
					if(walkWeight != 0.0f)
					{
						animation.Blend((Mathf.Sign(relVel.z) > 0 ? FlyForwardAnim : FlyBackwardAnim).name, walkWeight, 0.1f);
						//AudioSource.PlayClipAtPoint(WalkSound, transform.position);
						//animation.CrossFade((Mathf.Sign(relVel.z) > 0 ? WalkForwardAnim : WalkBackwardAnim).name, 3.0f, PlayMode.StopAll);
					}
					else
					{
						animation.CrossFade(IdleAnim.name, 1.0f);
					}					
				}
			}
			//if the velocity's too low, stop any walking sound being played and run an idle anim
			else
			{
				if(actorSFX.isPlaying)
				{
					actorSFX.Stop();
				}
				animation.CrossFade(IdleAnim.name);
				//animation.Blend(IdleAnim.name, 10.0f, 0.01f);
			}
			//firing animations would need to be called manually!
		}
	}
	
	public void LateUpdate()
	{
		updateModelBones ();
	}
	
	public void PlayFiringAnimation(Launcher.LauncherType type)
	{
		if(modelRB != null && !DeathAnimPlayed)
		{
			if(FiringAnims.ContainsKey(type))
			{
				animation.Blend(FiringAnims[type].name, 10.0f, 0.0f);
			}
		}
	}
	
	public void PlayDeathAnimation()
	{
		animation[DeathAnim.name].wrapMode = WrapMode.Once;
		//animation.Stop();
		if(!animation.IsPlaying(DeathAnim.name) && !DeathAnimPlayed)
		{
			animation.Play(DeathAnim.name, PlayMode.StopAll);
		}
		DeathAnimPlayed = true;
		//animation.CrossFade(DeathAnim.name, 0.0f, PlayMode.StopAll);
	}
	
	public void UpdateModelBones(Vector3 newOrientationEuler)
	{
		newOrient = newOrientationEuler;
	}
	
	private void updateModelBones()
	{;
		foreach(Transform bone in PitchDependentBones)
		{
			//Debug.Log ("rotating bone");
			Vector3 newRotation = bone.localRotation.eulerAngles;
			newRotation.y = -newOrient.x + 90.0f;
			bone.localRotation = Quaternion.Euler(newRotation);
		}
		foreach(Transform bone in YawDependentBones)
		{
			Vector3 newRotation = bone.localRotation.eulerAngles;
			newRotation.x = -newOrient.y;
			bone.localRotation = Quaternion.Euler(newRotation);
			newRotation = bone.rotation.eulerAngles;
			//newRotation.x = 0.0f;
			//newRotation.y = 0.0f;
			//bone.rotation = Quaternion.Euler(newRotation);
		}
		foreach(Transform bone in RollDependentBones)
		{
			Vector3 newRotation = bone.localRotation.eulerAngles;
			newRotation.z = newOrient.z;
			bone.localRotation = Quaternion.Euler(newRotation);
		}
	}
}
