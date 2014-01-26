using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KopyKat
{
    public interface IActorControllerListener
    {
        void OnActorMove(Vector3 forceVec);
    }

    public class ActorController : MonoBehaviour
    {
        //Movement members
        public float SpeedMax;
        public float ForceMax;
        public float MoveForce;
	public float JumpForceFactor = 1.0f;
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
        private bool isAlive = true;

        //collision members
        private Vector3 groundNormal;
        private const float GROUNDED_NORMAL_MAG_MIN = 0.01f;

        //effect managers
        private List<IActorControllerListener> effectListeners;

        //networking managers
        private PhotonView photonView;

        //spawning members
        public Vector2 DistributionRange = Vector2.one;
        
        public Vector3 Orientation
        {
            get { return orientation; }
        }

        public Vector3 Forward
        {
            get { return transform.forward; }
        }

        public Vector3 Right
        {
            get { return transform.right; }
        }

        public Vector3 Up
        {
            get { return transform.up; }
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
            get { return isAlive; }
        }

        public bool IsPlayingAnimation
        {
            get { return false; }//(processor != null) ? processor.IsPlaying : false; }
        }

        public void Kill(GameObject attacker)
        {
            isAlive = false;
            ActorKilledEvent actKilled = new ActorKilledEvent(gameObject, attacker);
            EventManager.TriggerEvent(actKilled);
        }

        public bool IsGrounded
        {
            get { return checkIsGrounded(); }
        }

        public float SqrVelocity
        {
            get { return rigidbody.velocity.sqrMagnitude; }
        }

        public bool IsNetworked
        {
            get { return photonView != null; }
        }

        public bool IsClientControlled
        {
            get
            {
                if (IsNetworked)
                {
                    return photonView.isMine;
                }
                //no network implies everything's client-side
                return true;
            }
        }

        public float HorizSpeed()
	{
		Vector2 v = new Vector2(rigidbody.velocity.x,rigidbody.velocity.z);
		return v.magnitude;
	}

	public float VertSpeed()
	{
		return rigidbody.velocity.y;
	}
        
        public void AttachListener(IActorControllerListener listener)
        {
            if (!effectListeners.Contains(listener))
            {
                effectListeners.Add(listener);
            }
        }

        public void RemoveListener(IActorControllerListener listener)
        {
            effectListeners.Remove(listener);
        }

        // Use this for initialization
        void Start()
        {
            //randomize our position
            Vector3 newPos = new Vector3(Random.Range(-DistributionRange.x, DistributionRange.x),
                                        transform.position.y,
                                        Random.Range(-DistributionRange.y, DistributionRange.y));
            transform.position = newPos;
            //also randomize heading
            transform.rotation = Quaternion.Euler(0.0f, Random.Range(0.0f, 360.0f), 0.0f);
            photonView = GetComponent<PhotonView>();
            //init listener list
            effectListeners = new List<IActorControllerListener>();
            //init movement
            forceVec = new Vector3();
            //init orientation properties
            orientation = transform.localRotation.eulerAngles;
            rotMask = new Vector3(AllowPitch ? 1 : 0,
                                    AllowYaw ? 1 : 0,
                                    AllowRoll ? 1 : 0);
            AIManager.GetInstance().AddActor(this);
        }

        // Update is called once per frame
        void Update()
        {
        }

        void FixedUpdate()
        {
            if (IsClientControlled)
            {
                //handle movement
                //if we're moving really slowly, just halt so we don't appear to be slipping
                if (speedSqr < Mathf.Pow(SpeedFloor, 2))
                {
                    rigidbody.velocity = new Vector3(0.0f, rigidbody.velocity.y, 0.0f);
                }
                foreach (IActorControllerListener listener in effectListeners)
                {
                    listener.OnActorMove(forceVec);
                }
                //update model's bones as needed
                //if (processor != null)
                //{
                //    processor.UpdateModelBones(orientation);
                //}
                //if we're dead, play a death animation
                if (!IsAlive)
                {
                    //processor.PlayDeathAnimation();
                    //Once that's done, fade out or something
                    //NOTE TO SELF: totally need an event system for this
                    if (!IsPlayingAnimation)
                    {
                        EventManager.TriggerEvent(new CorpseGoneEvent(this.gameObject));
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
        {
            //do not run if this client can't control this actor
            if (!IsClientControlled)
            {
                return;
            }
        }

        //Puts your hands up.
        public void HandsUp()
        {
            //do not run if this client can't control this actor
            if (!IsClientControlled)
            {
                return;
            }
        }

        public void Jump()
        {
            //do not run if this client can't control this actor
            if (!IsClientControlled)
            {
                return;
            }
            Vector3 jumpForce = (Vector3.up * MoveForce * JumpForceFactor);
            rigidbody.AddRelativeForce(jumpForce);
        }

        //orientation modifying functions.
        public void Move(Vector3 direction)
        {
            //do not run if this client can't control this actor
            if (!IsClientControlled)
            {
                return;
            }
            if (IsAlive && SpeedMax > 0)
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

        //Moves to a point in the world, if possible.
        public void MoveTo(Vector3 worldPos)
        {
            //do not run if this client can't control this actor
            if (!IsClientControlled)
            {
                return;
            }
            Vector3 moveDir = (worldPos - transform.position).normalized;
            Move(moveDir);
        }

        public void Rotate(Vector3 eulerAngles)
        {
            //do not run if this client can't control this actor
            if (!IsClientControlled)
            {
                return;
            }
            if (IsAlive)
            {
			Vector3 rotation = new Vector3(eulerAngles.x * rotMask.x, eulerAngles.y * rotMask.y, eulerAngles.z * rotMask.z);
			if (rotation.sqrMagnitude <= 0.1f || float.IsNaN(rotation.sqrMagnitude))
			{
				return;
			}
			transform.Rotate(rotation);
                orientation += eulerAngles;
            }
        }

        public void RotateY(float anglesDeg)
        {
            //do not run if this client can't control this actor
            if (!IsClientControlled)
            {
                return;
            }
            Rotate(new Vector3(0.0f, anglesDeg, 0.0f));
        }

        public void RotateToOrientation(Quaternion newHeading)
        {
            //do not run if this client can't control this actor
            if (!IsClientControlled)
            {
                return;
            }
            if (IsAlive)
            {
                Vector3 newHedEuler = newHeading.eulerAngles;
                Rotate(newHedEuler - orientation);
                //transform.rotation = newHeading;
                //orientation = newHeading.eulerAngles;
            }
        }

        //add a MoveTo method?

        private void addForce(Vector3 force)
        {
            //do not run if this client can't control this actor
            if (!IsClientControlled)
            {
                return;
            }
            Vector3 finalForce = force;

            //can we still move this frame?
            if (ForceMax >= 0.0f)
            {
                if (availableForceSqr > 0.0f)
                {
                    if (availableForceSqr < force.sqrMagnitude)
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
            if (SpeedMax >= 0.0f && speedSqr >= Mathf.Pow(SpeedMax, 2))
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
}