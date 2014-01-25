using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Global AI manager.
/// </summary>
public class AIManager : MonoBehaviour {
	
	private static AIManager instance;
	private List<PlayerInput> commanders;
	//by default, does nothing
	
	int nextID = 1;
	int numRemoved = 0;
	
	// added by Mike D 12:40 am sat
	private List<CivilianInput> botList;
	private List<PlayerInput> playerList;
	
	public void ON_SIGNAL(int SignalType)
	{
		float closestPlayerDistance = -1;
		float nextDistance;
		// Set each civilian's following player
		foreach (CivilianInput bot in botList){
		if (bot.State==bot.State.Walk || bot.State==bot.State.Idle) {
			foreach (PlayerInput player in playerList){
				nextDistance = Vector3.Distance(bot.transform.position,player.transform.position);
				if (closestPlayerDistance < 0 || nextDistance < closestPlayerDistance){
					bot.SetMasterPlayer(player);
					closestPlayerDistance = nextDistance;
				}
			}
			closestPlayerDistance = -1;
			switch (SignalType){
			case 0: // walk, jump
				bot.State = bot.State.Walk;
				bot.Jump(); // jump after, so it is a directional jump
				break;
			case 1: // walk, no jump
				bot.State = bot.State.Walk;
				break;
			case 2: // no walk, jump
				bot.Jump();
				bot.State = bot.State.Idle;
				break;
			case 3: // no walk, no jump
				bot.State = bot.State.Idle;
				break;
			default: // hands-up or stab
				bot.State = bot.State.HandsUp;
				break;
			}
		}	
		}
	}
	
	
	public static AIManager GetInstance()
	{
		if(instance == null)
		{
			Debug.Log("!AIManager: error: no instance found! Is an AI_MANAGER object in the scene hierarchy?");
		}
		return instance;
	}

	public void AddCommander(PlayerInput pCommander)
	{
		if (commanders.Contains(pCommander))
		{
			commanders.Add(pCommander);
		}
	}

	public void RemoveCommander(PlayerInput pCommander)
	{
		if (commanders.Contains(pCommander))
		{
			commanders.Remove(pCommander);
		}
	}

	// Init the instance regardless of whether the script is enabled.
	void Awake()
	{
		instance = this;
		//also attach to event instance
	}
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		//no need for this, really
	}
}