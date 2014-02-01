using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KopyKat
{
    public class Targeting : MonoBehaviour
    {
        public ActorController owner;
        SphereCollider szone;
        private float player_distance = 0.6f;
        bool validtarget;
        bool has_target;
        GameObject current_target;
        Vector3 height = new Vector3(0f, 0.7f, 0f);

        void Start()
        {
            has_target = false;
            current_target = null;
            szone = GetComponent<SphereCollider>();
        }

        void Update()
        {
            this.transform.position = owner.transform.position + height + owner.transform.forward * player_distance;
            /*
            if (has_target)
                Debug.Log("TARGET!");
            else
                Debug.Log ("no target...");
            //Debug.Log (owner.transform.forward*player_distance);*/
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "ActorPhysics")
            {
		//	Debug.Log ("OnTriggerEnter -> FOUND TARGET");
                has_target = true;
                current_target = other.gameObject;
            }
		//else Debug.Log("OnTriggerEnter -> None");

        }

        void OnTriggerExit(Collider other)
        {
		//Debug.Log ("OnTriggerExit");

            has_target = false;
            current_target = null;
        }

        public bool HasTarget()
        {
            return has_target;
        }

        public GameObject Victim()
        {
            {
                return current_target;
            }
        }
    }
}