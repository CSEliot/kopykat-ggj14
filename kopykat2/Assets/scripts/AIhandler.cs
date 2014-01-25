using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AIhandler : MonoBehaviour {

	private ArrayList<Civilian> civilianlist;
	private ArrayList<FirstPersonController> playerlist;
	private int bodycount;
	public bool ShouldPanic = false;
	public bool ShouldHandsUp = false;

	private float TimerHandsUpMax = 1.0f;
	private float TimerHandsUp = TimerHandsUpMax;

	// Use this for initialization
	void Start () {

	}

	// Player uses knife or does hands up
	public void Scare()
	{
		if (!ShouldHandsUp){
			ShouldHandsUp = true;
			TimerHandsUp = TimerHandsUpMax;
			Signal();
		}
	}

	public void AddCivilian(Civilian c)
	{
		civilianlist.Add(c);
	}

	public void RemoveCivilian(Civilian c)
	{
		civilianlist.Remove(c);
	}
	 
	// Called by civilians
	public void ModBodycount(int change)
	{
		bodycount += change;
		Signal();
	}
	
	// What is this doing???
	void Update () {
		GameObject.Find ("PlayerA");
		// During hands up time, only the civilians are forced
		// to stand still
		if (ShouldHandsUp && TimerHandsUp > 0){
			TimerHandsUp -= GameSystem.Tick;
		}
		else
		{
			ShouldHandsUp = false;
			Signal ();
		}
	}

	// Called when civilian uses ModBodycount
	// or when player changes state
	public void Signal()
	{
		float nearestDist;
		float nextDist;
		// Determine panic mode
		if (bodycount > 0)
			ShouldPanic = true;
		else
			ShouldPanic = false;
		// then loop through civilians and determine
		// their action in the next frame
		foreach (Civilian c in civilianlist)
		{
			nearestDist = -100f;
			nextDist = 0;
			// Panic or hands up = do not copy player
			// Otherwise mimic closest player
			if (!ShouldPanic && !ShouldHandsUp){
				nearestDist = -100f;
				nextDist = 0;
				foreach (FirstPersonController p in playerlist)
				{
					nextDist = (c.transform.position - p.transform.position).magnitude;
					if (nearestDist < 0 || nextDist < nearestDist){
						nearestDist = nextDist;
						c.SetMasterPlayer(p);
					}
				}
			}
			// Set next state
			c.NextState(IsPanic);
		}
	}
}
