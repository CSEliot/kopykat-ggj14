using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KopyKat
{
    public class PlayerInput : MonoBehaviour
    {

        //Mouselook members
        //this is a major member, and other vars are defined by it
        //should only be changed through SetActor, but don't know how to do that yet
        public float CameraSensitivity;
        public float MaxYawRate; //move this to TaurusActor?
        private Vector3 rotVec = Vector3.zero;

        //audio members
        private AudioListener audioDev = null;

        //Lockon members
        public Camera ActorCamera = null;
        public Targeting tsystem;
        GameObject targetActor;

        //connected subsystems
        public ActorController ActorCtrl = null;
        private Rigidbody actorRB;
        private AIManager aiManager;

        //command members
        public bool movedCurr, movedPrev;
        public bool rotatedCurr, rotatedPrev;
        public bool shivCurr, shivPrev;
        public bool handsUpCurr, handsUpPrev;
        public bool jumpCurr, jumpPrev;

        private int networkID = -1;

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

        public bool StartedJump
        {
            get { return jumpCurr && (!jumpPrev); }
        }

        public bool EndedJump
        {
            get { return (!jumpCurr) && jumpPrev; }
        }
        #endregion

        // Use this for initialization
        void Start()
        {
            GameSystem.GamePaused = false;
            //setup the camera
            //if we haven't been passed a rigidbody, lookup one in our children
            movedCurr = false;
            movedPrev = false;
            shivCurr = false;
            shivPrev = false;
            handsUpCurr = false;
            handsUpPrev = false;
            rotatedCurr = false;
            rotatedPrev = false;
            jumpCurr = false;
            jumpPrev = false;
            if (ActorCamera != null)
            {
                audioDev = ActorCamera.GetComponentInChildren<AudioListener>();
            }
            //get info from the network if available.
            if (ActorCtrl.IsNetworked)
            {
                if (ActorCtrl.IsClientControlled)
                {
                    networkID = Matchmaker.GetPlayerNum();
                }
            }
            ReloadActor();
            //add ourselves to the AI system
            aiManager = AIManager.GetInstance();
            aiManager.AddMaster(this);

            //we don't want the friggin' mouse moving around!
            Screen.lockCursor = true;
        }

        // Update is called once per frame
        void Update() { }

        void FixedUpdate()
        {
            //if this is networked and the controlling player IDs don't match, don't accept input control.
            if (networkID >= 0 && networkID != Matchmaker.GetPlayerNum())
            {
                return;
            }
            //clear command queue for this frame
            //commandsThisFrame.Clear();
            updateMouseLook();
            //we can't do movement or weapon control if we're dead
            if (ActorCtrl.IsAlive)
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
            if (ActorCtrl != null)
            {
                //and get the actor's rigidbody
                actorRB = ActorCtrl.rigidbody;
                //also child our CAMERA to the actor
                if (ActorCamera != null)
                {
                    ActorCamera.transform.parent = ActorCtrl.transform;
                    //and set to the actor's position and heading
                    ActorCamera.transform.localPosition = -10.0f * Vector3.forward + 7.5f * Vector3.up;
                    ActorCamera.transform.localRotation = Quaternion.Euler(Vector3.zero);
                }
            }
            else
            {
                //if we didn't attach to a new actor, something's gone wrong
                Debug.Log("PlayerInput: can't find new actor!");
            }
        }

        protected void updateMouseLook()
        {
            //build a rotation vector from the mouse axis.
            rotVec = new Vector3(-Input.GetAxis("Mouse Y") * CameraSensitivity,
                                 Input.GetAxis("Mouse X") * CameraSensitivity,// * (1.0f - actorRB.angularDrag),
                                 0.0f);
            //The Y component applies to the physics object; X applies to the camera to create tilt
            if (ActorCamera != null)
            {
                //ActorCamera.PitchCamera (rotVec.x);
                //ActorCamera.YawCamera(rotVec.y);
            }
            if (MaxYawRate >= 0.0f)
            {
                rotVec.y = Mathf.Clamp(rotVec.y, -MaxYawRate, MaxYawRate);
            }
            ActorCtrl.Rotate(rotVec);
        }

        protected void updateMovementInput()
        {
            rotatedPrev = rotatedCurr;
            movedPrev = movedCurr;
            jumpPrev = jumpCurr;
            //compose the velocity vector first
            //...a little easier than I thought it'd be
		// Prevent double jumping
            Vector3 moveVector = new Vector3(	Input.GetAxis("Horizontal"),
		                     		Input.GetButtonDown("Jump") && Mathf.Abs(ActorCtrl.VertSpeed()) < 0.2f ? 1.0f : 0.0f,
	                                        Mathf.Clamp(Input.GetAxis("Vertical"), 0.0f, 1.0f));
            //if needed, renormalize the movement vector
            if (moveVector.sqrMagnitude > 1.0f)
            {
                moveVector.Normalize();
            }

            //only thing that matters is the camera's yaw
            Vector3 cameraYaw = Vector3.forward;
            if (ActorCamera != null)
            {
                cameraYaw = new Vector3(0.0f, ActorCamera.transform.localEulerAngles.y, 0.0f);
            }
            else
            {
                cameraYaw = new Vector3(0.0f, ActorCtrl.transform.localEulerAngles.y, 0.0f);
            }
            Quaternion cameraRot = Quaternion.Euler(cameraYaw);
            moveVector = cameraRot * moveVector;
            Vector3 horizMove = new Vector3(moveVector.x, 0.0f, moveVector.z);
            Vector3 playerHeading = transform.rotation * Vector3.forward;
            float rot = Mathf.Asin(Vector3.Dot(horizMove.normalized, playerHeading.normalized));
            //and then give the movement order to the actor
            ActorCtrl.Move(moveVector);
            if (Input.GetButtonDown("Jump"))
            {
                ActorCtrl.Jump();
            }
            //ActorCtrl.Rotate(new Vector3(0, rot, 0));
            //also add movement command to command queue
            if (moveVector.sqrMagnitude > 0.0f)
            {
                movedCurr = true;
            }
            else
            {
                movedCurr = false;
            }
            if (Mathf.Abs(moveVector.y) > 0.0f)
            {
                jumpCurr = true;
            }
            else
            {
                jumpCurr = false;
            }
            if (cameraYaw.sqrMagnitude > 0.0f)
            {
                rotatedCurr = true;
            }
            else
            {
                rotatedCurr = false;
            }
        }

        //TODO
        protected void updateShivInput()
        {
            shivPrev = shivCurr;
            if (Input.GetButtonDown("Shiv"))
            {
		if (tsystem.HasTarget())
                {
			this.ActorCtrl.Jump(); return;
			// temp ^^^
			targetActor = tsystem.Victim();
                    {
                        //target.Kill(this.gameObject);
                    }
                    //notify world that someone got shivved
                    EventManager.TriggerNetworkEventAll(new ShivEvent());
                }
            }
        }

        //TODO
        protected void updateHandsUpInput()
        {
            handsUpPrev = handsUpCurr;
        }
    }
}