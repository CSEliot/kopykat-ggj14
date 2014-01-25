using UnityEngine;
using System.Collections;

public class PlayerLogic : MonoBehaviour {
	
	public GameObject Actor;
	public CameraController CameraRig = null;
	private PlayerInput input;
	private AnimationProcessor processor;
	private ActorController actorCtrl;
	//ejection system members
	private VehicleSeats vehSeats;
	private int seatNum = 0;
	
	public Transform LockOnTarget
	{
		get { return input.LockOnTarget; }
	}
	
	public Transform MissileTarget
	{
		get { return input.MissileTarget; }
	}
	
	public int MissileAmmo
	{
		get { return actorCtrl.MissileAmmo; }
	}
	
	//[ExposeProperty]
	public int CannonAmmo
	{
		get { return actorCtrl.CannonAmmo; }
	}
	
	public ActorController ActorCtrl
	{
		get { return (actorCtrl != null) ? actorCtrl : Actor.GetComponent<ActorController>(); }
	}
	
	// Use this for initialization
	void Start () {
		input = GetComponent<PlayerInput>();
		SetActor(Actor);
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	public bool SetActor (GameObject newActor)
	{
		Debug.Log("setting actor");
		//pop off the previous actor
		if(Actor != null)
		{
			transform.parent = null;
			Actor.transform.parent = null;
			CameraRig.transform.parent = transform;
		}
		Actor = newActor;
		//if the new actor's invalid, just quit here
		if(newActor == null)
		{
			return false;
		}
		//get the actor's health
		Debug.Log("reading health from " + Actor);
		processor = Actor.GetComponentInChildren<AnimationProcessor>();
		//get the actor's controller
		actorCtrl = Actor.GetComponent<ActorController>();
		//get the actor's vehicle seats if possible
		vehSeats = Actor.GetComponent<VehicleSeats>();
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
	
	public bool SetActorAndSeat (GameObject newActor, int newSeatNum)
	{
		Debug.Log("trying to board " + newActor + "at seat " + newSeatNum);
		VehicleSeats newSeats = newActor.GetComponent<VehicleSeats>();
		//if the new actor can be boarded...
		if(newSeats != null)
		{
			Debug.Log ("found vehicle seats");
			//temporarily pop us off our actor to avoid cloning the camera
			transform.parent = null;
			CameraRig.transform.parent = transform;
			//try to board the vehicle
			if(newSeats.Enter(newSeatNum, actorCtrl))
			{
				Debug.Log ("boarded vehicle at seat " + newSeatNum);
				//if we're aboard, reference the new actor's seats
				vehSeats = newSeats;
				seatNum = newSeatNum;
				//and take control of the new actor
				return SetActor(newActor);
			}
			//if the vehicle couldn't be boarded, then keep control of our current actor
			Debug.Log("couldn't board vehicle");
			return false;
		}
		else
		{
			Debug.Log("no seat found");
			return SetActor(newActor);
		}
	}
	
	public bool Eject()
	{
		Debug.Log ("trying to eject from seat " + seatNum + "!");
		//simple enough, eject from our actor and take control of the ejected actor
		if(vehSeats != null)
		{
			GameObject ejectedActor = vehSeats.Eject(seatNum);
			//reset the seat number to a default value
			seatNum = 0;
			return SetActor(ejectedActor);
		}
		return false;
	}
}
