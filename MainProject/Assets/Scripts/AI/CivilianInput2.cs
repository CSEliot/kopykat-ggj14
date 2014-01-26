using UnityEngine;
using System.Collections;

namespace KopyKat
{
    public class CivilianInput2 : MonoBehaviour
    {

        bool bDying;
        float speed;
        float angle;
        float TimerDeath;
        PlayerInput masterPlayer;
        private AIManager aiManager;
        public ActorController ActorCtrl;

	private float speedCoefficient;
    

        public float ThresholdSpeed = 0.2f;

        float walkSpeed = 2.0f;
        float runSpeed = 5.0f;

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
        void Start()
        {
            aiManager = AIManager.GetInstance();
            TimerDeath = 10f; // duration of animation + extra time
            bDying = false;
            speed = walkSpeed;
            angle = 0;//Random.Range(0.0f,Mathf.PI*2);
            //initialize to a random initial heading
            ActorCtrl.RotateY(Random.Range(0.0f, 360.0f));
            currWanderAngle = Random.Range(0.0f, 8000.0f);
		speedCoefficient = Random.Range (0.9f,1.1f);
            float minJitter = 0.5f;
            float maxJitter = 3.75f;
            WanderRadius *= Random.Range(minJitter, maxJitter);
            WanderDistance *= Random.Range(minJitter, maxJitter);
            MaxWanderJitter *= Random.Range(minJitter, maxJitter);
            HardTurnThreshold *= Random.Range(minJitter, maxJitter);
            HardTurnAngle *= Random.Range(minJitter, maxJitter);
        }

        private float randomNormVal()
        {
            float result = (Mathf.PerlinNoise(currWanderAngle, currWanderAngle) * 2.0f) - 1.0f;
            return result;
        }

        private Vector3 getWanderPosition()
        {
            //displace in front of the civilian
            Vector3 result = new Vector3(randomNormVal() * MaxWanderJitter,
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
            Vector3 seekCenter = Vector3.zero - ActorCtrl.transform.position;
            if (ActorCtrl.SqrVelocity < HardTurnThreshold * HardTurnThreshold)
            {
                wanderPos = seekCenter;
            }
            else
            {
                wanderPos = seekCenter;
            }
            angle = (Vector3.Dot(wanderPos.normalized, ActorCtrl.Forward.normalized)) * 100.0f;
            //angle = Mathf.Clamp(angle, -180.0f, 180.0f);
            //get the current velocity; if it's too slow, make a sharp turn
            /*if (ActorCtrl.SqrVelocity < HardTurnThreshold)
            {
                angle = Mathf.Sign(angle) * HardTurnAngle; 
            }*/
            if (this.ActorCtrl.SpeedMax > ThresholdSpeed)
            {
                ActorCtrl.RotateY(angle);
            }
            Vector3 strafeJitter = Vector3.right * Random.Range(-1.0f, 1.0f);
            Vector3 finalMove = (Vector3.forward + strafeJitter).normalized;
            ActorCtrl.Move(finalMove);//getWanderPosition());
        }

        public void SetMasterPlayer(PlayerInput P)
        {
            masterPlayer = P;
        }

        public void Update()
        {
            currWanderAngle += Time.deltaTime;
            if (bDying)
            {
                if (TimerDeath > 0)
                {
                    TimerDeath -= Time.deltaTime;
                }
                else
                {
                    ; ; //delete
                }
                return;
            }
            SetMasterPlayer(aiManager.GetNearestMaster(this.transform.position));
		//Debug.Log ("finding nearest master");
            if (aiManager.ShouldPanic)
            {
                this.ActorCtrl.SpeedMax = runSpeed;
            }
            else if (aiManager.ShouldHandsUp)
            {
                ActorCtrl.SpeedMax = 0;
                this.ActorCtrl.HandsUp();
            }
            else
			if (masterPlayer.ActorCtrl.HorizSpeed()>ThresholdSpeed)
                {
				//Debug.Log("Should walk");
				ActorCtrl.SpeedMax = speedCoefficient*masterPlayer.ActorCtrl.HorizSpeed();

                }
                else
                {
				//Debug.Log("Should idle");
                    ActorCtrl.SpeedMax = 0;
                }
			if (Mathf.Abs(masterPlayer.ActorCtrl.VertSpeed())>0.9f)
			{
				if (Mathf.Abs(this.ActorCtrl.VertSpeed())<=2f)
				{
					// JUMP, DAMMIT!!!!!!!!!
					this.ActorCtrl.Jump();
//					if (masterPlayer.ActorCtrl.HorizSpeed()>ThresholdSpeed)
						//Debug.Log ("Should walk and jump");
//					else
						//Debug.Log ("Should jump.");
				}
			}
		}
		updatePosition();
	}
}