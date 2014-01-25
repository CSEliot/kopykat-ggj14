using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VehicleSeats : MonoBehaviour {
	
	//the seats in the vehicle
	public List<ActorController> Seats = new List<ActorController>();
	
	// Use this for initialization
	void Start () {
		//instance any prefab actors aboard
		for(int i = 0; i < Seats.Count; ++i)
		{
			if(Seats[i] != null)
			{
				//we want to instance passengers, but we have to instance the prefab's GameObject to instance all of the prefab's scripts
				//instance the ActorController's GameObject, but give the seat the instance of the ActorController
				GameObject actorInstance = (GameObject)Instantiate(Seats[i].gameObject);
				Seats[i] = actorInstance.GetComponent<ActorController>();
				//disable the instance so it's not affected by the world
				actorInstance.SetActiveRecursively(false);
				actorInstance.transform.parent = transform;
				//make any meshes in the actor invisible
				//setMeshVisibilityInActor(actorInstance, false);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	/// <summary>
	/// Ejects the passenger in seatNum.
	/// </summary>
	/// <param name='seatNum'>
	/// Seat number. Seat 0 is the pilot's seat.
	/// </param>
	public GameObject Eject(int seatNum)
	{
		//if the seat number's in the list's index range and the seat's occupied, continue
		if(seatNum >= 0 && seatNum < Seats.Count && Seats[seatNum] != null)
		{
			//spawn the pilot outside of the actor
			GameObject passengerPrefab = Seats[seatNum].gameObject;
			//note to self, make the vert displacement a variable
			GameObject passenger = (GameObject)Instantiate(passengerPrefab, rigidbody.position + (Vector3.up * 10), rigidbody.rotation);
			//and null the pilot that's in the seat; we want to preserve seat positions
			Seats[seatNum] = null;
			//if necessary, destroy the previous pilot
			Destroy(passengerPrefab);
			//make sure the passenger is visible!
			passenger.SetActiveRecursively(true);
			//setMeshVisibilityInActor(passenger, true);
			//return the ejected passenger so input scripts can control it
			Debug.Log ("ejecting");
			return passenger;
		}
		Debug.Log ("eject failed");
		return null;
	}
	
	/// <summary>
	/// Enter the vehicle at the specified seatNum.
	/// </summary>
	/// <returns>
	/// True if the vehicle was boarded, false otherwise.
	/// </returns>
	public bool Enter(int seatNum, ActorController actor)
	{
		//only allow infantry to board!
		if(actor.IsInfantry)
		{
			//if the seat number's in the list's index range and the seat's UNoccupied, continue
			if(seatNum >= 0 && seatNum < Seats.Count && Seats[seatNum] == null)
			{
				Debug.Log("boarding seat " + seatNum);
				//put the actor controller in the seat
				Seats[seatNum] = actor;//actorInstance.GetComponent<ActorController>();
				//and disable the actor so it can't be affected by the game world
				actor.gameObject.SetActiveRecursively(false);
				actor.transform.parent = transform;
				//actor.gameObject.active = false;
				//setMeshVisibilityInActor(actor.gameObject, false);
				return true;
			}
			Debug.Log("could not board seat " + seatNum);
			return false;
		}
		Debug.Log(actor + "can't board, it's not infantry");
		return false;
	}
	
	protected void setMeshVisibilityInActor(GameObject actor, bool isVisible)
	{
		Renderer[] actorMeshes = actor.GetComponentsInChildren<Renderer>();
		foreach(Renderer r in actorMeshes)
		{
			r.enabled = isVisible;
		}
	}
}
