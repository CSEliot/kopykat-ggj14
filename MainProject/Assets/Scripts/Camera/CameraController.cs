using UnityEngine;
using System.Collections;

namespace KopyKat
{
    public class CameraController : MonoBehaviour
    {

        public float DisplaceFactor = 2.0f; //magnitude of each component of camera's offset
        public float DisplaceHorizFactor = 1.0f;
        public float DisplaceVertFactor = 0.5f;
        public float DisplaceDepthFactor = 1.0f;
        public float MaxPitch = 85.0f;
        public float MinPitch = -45.0f;
        public float DisplaceAngle = 33.3f;
        private Vector3 DisplaceVec;
        private Vector3 yawPivot;
        private Camera cam;

        public Camera ControlledCamera
        {
            get { return cam; }
        }

        // Use this for initialization
        void Start()
        {
            cam = GetComponentInChildren<Camera>();
            if (cam == null)
            {
                Debug.Log("!CameraController: fatal error, camera rig couldn't find camera!");
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// Parents (target) to this camera, and offsets the camera a size-dependent distance from the target.
        /// </summary>
        public void BindTo(Transform target)
        {
            Debug.Log("CameraController: binding to target " + target.ToString());
            Renderer targetMesh = target.GetComponentInChildren<Renderer>();
            //make the target our parent
            transform.parent = target;
            //if the target has a mesh, we can setup a displacement vector
            if (targetMesh != null)
            {
                Debug.Log("CameraController.BindTo: found mesh " + targetMesh.name);
                //since we want the target on the bottom left side of the screen,
                //we want a displacement vector with positive x, positive y, and negative z
                //Debug.Log ("CameraController.BindTo: mesh bounds = " + targetMesh.bounds.size);
                //calculate a number representing the horiz to vert proportions
                Vector3 boundsSize = targetMesh.bounds.size;
                float meshAspectRatio = boundsSize.x / boundsSize.y;
                Vector3 scaledMeshSize = boundsSize * DisplaceFactor;
                DisplaceVec = Vector3.zero;//targetMesh.bounds.extents * displaceFactor;
                //bounds should always be positive, so we only need to negate the z component
                DisplaceVec.x = scaledMeshSize.x * DisplaceHorizFactor;
                //DisplaceVec.z = scaledMeshSize.z * -DisplaceDepthFactor;//scaledMeshSize.z * -DisplaceDepthFactor;
                //DisplaceVec.z = scaledMeshSize.z * -DisplaceDepthFactor;//scaledMeshSize.z * -DisplaceDepthFactor;
                //the x displacement of the camera should put the camera at a 33 degree angle to give a nice offset to the 3rd person view
                DisplaceVec.z = -Mathf.Abs(DisplaceVec.x / (Mathf.Tan(DisplaceAngle * Mathf.Deg2Rad))) * DisplaceDepthFactor;
                DisplaceVec.y = scaledMeshSize.y * DisplaceVertFactor;//(DisplaceVertFactor / Mathf.Pow(scaledMeshSize.y, 2)); /// 2.0f;// * meshAspectRatio * DisplaceVertFactor;
                //Debug.Log ("CameraController.BindTo: displacement vec = " + DisplaceVec.ToString());
            }
            //otherwise, just place the camera at the target's center
            else
            {
                DisplaceVec = Vector3.zero;
            }
            //setup our new positions - first reset the camera's local position so rotations don't distort the new displacement,
            cam.transform.localPosition = Vector3.zero;
            transform.localPosition = Vector3.zero;
            //then set our rotation and displace the camera by the vector we calculated
            transform.rotation = target.rotation;
            cam.transform.localPosition = DisplaceVec;
        }

        /// <summary>
        /// Pitches the camera by (pitchDegs) degrees.
        /// </summary>
        public void PitchCamera(float pitchDegs)
        {
            //apply the rotation, but clamp it to [-90.0, 90] degrees
            //check the potential rotation
            float max = MaxPitch + 90.0f;
            float min = MinPitch + 90.0f;
            //normally, directly behind the target = 0.0 degrees, 
            //which jumps to 360-(angle) when we angle below the target, then falls off to 270 at 90 degrees below the target.
            //to handle going below 0 degrees, add 90 to the current heading and modulus it to get the correct angle
            float currX = Mathf.Repeat(transform.rotation.eulerAngles.x + 90.0f, 360.0f);
            float finalPitch = pitchDegs;
            if (currX + pitchDegs > max)
            {
                finalPitch = currX - max;
            }
            else if (currX + pitchDegs < min)
            {
                finalPitch = currX - min;
            }
            transform.Rotate(finalPitch, 0.0f, 0.0f);
        }

        public void YawCamera(float yawDegs)
        {
            transform.RotateAround(transform.parent.position, Vector3.up, yawDegs);
        }
    }
}