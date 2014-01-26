using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KopyKat
{
    public class Targeting : MonoBehaviour
    {
        public PlayerInput owner;
        SphereCollider szone;
        private float playerdistance;
        bool validtarget;
        bool has_target;
        GameObject current_target;

        void OnStart()
        {
            szone.radius = 5.0f;
            has_target = false;
            current_target = null;
            playerdistance = szone.radius;
        }

        void Update()
        {
            this.transform.position = owner.transform.position + owner.transform.forward * playerdistance;
            szone.center = this.transform.position;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.tag.Equals("Actor"))
            {
                has_target = true;
                current_target = other.gameObject;
            }
        }

        void OnTriggerExit(Collider other)
        {
            has_target = false;
            current_target = null;
        }

        public bool HasTarget
        {
            get { return has_target; }
        }
        public GameObject Target
        {
            get
            {
                if (has_target)
                    return current_target;
                else
                    return null;
            }
        }
    }
}