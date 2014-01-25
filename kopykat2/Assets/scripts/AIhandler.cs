using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AIhandler : MonoBehaviour {

	private ArrayList<Civilian> civilianlist;
	private ArrayList<FirstPersonController> playerlist;


	private int bodycount;
	// Use this for initialization
	void Start () {
		

	}
	 
	public int Bodycount {
		get {return  bodycount;}
	}
	
	public void ModBodycount(int change)
	{
		bodycount += change;
	}
	
	// Update is called once per frame
	void Update () {
		GameObject.Find ("PlayerA");
	}

	void Signal()
	{
		float nearestDist;
		float nextDist;
		foreach (Civilian c in civilianlist)
		{
			nearestDist = -100f;
			nextDist = 0;
			// Get nearest player
			foreach (Player p in playerlist)
			{
				nextDist = (c.transform.position - p.transform.position).magnitude;
				if (nearestDist < 0 || nextDist < nearestDist){
					nearestDist = nextDist;
					c.SetMasterPlayer(p);
				}
			}
			c.NextState();
		}
	}
}
