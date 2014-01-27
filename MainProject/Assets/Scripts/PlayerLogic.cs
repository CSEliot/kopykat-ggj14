using UnityEngine;
using System.Collections;

namespace KopyKat
{
    public class PlayerLogic : MonoBehaviour
    {

        public GameObject Actor;
        public CameraController CameraRig = null;
        private PlayerInput input;
        private ActorController actorCtrl;
        //ejection system members
        private int seatNum = 0;

        public ActorController ActorCtrl
        {
            get { return (actorCtrl != null) ? actorCtrl : Actor.GetComponent<ActorController>(); }
        }

        // Use this for initialization
        void Start()
        {
            input = GetComponent<PlayerInput>();
            SetActor(Actor);
        }

        // Update is called once per frame
        void Update()
        {
        }

        public bool SetActor(GameObject newActor)
        {
            Debug.Log("setting actor");
            //pop off the previous actor
            if (Actor != null)
            {
                transform.parent = null;
                Actor.transform.parent = null;
                CameraRig.transform.parent = transform;
            }
            Actor = newActor;
            //if the new actor's invalid, just quit here
            if (newActor == null)
            {
                return false;
            }
            //get the actor's health
            Debug.Log("reading health from " + Actor);
            //get the actor's controller
            actorCtrl = Actor.GetComponent<ActorController>();
            //for now, set us as controlling the pilot
            seatNum = 0;
            //move and orient us to the new client and parent us to the client
            //CameraRig.transform.position = Actor.transform.position;
            //CameraRig.transform.rotation = Actor.transform.rotation;
            Actor.transform.parent = transform;
            //parent the client to our camera so mouselook centers on the client
            CameraRig.BindTo(Actor.transform);
            //CameraRig.transform.parent = Actor.transform;
            //notify the input script of our new actor
            input.ReloadActor();
            return true;
        }
    }
}