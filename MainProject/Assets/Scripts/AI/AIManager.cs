using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Global AI manager.
/// </summary>
public class AIManager {
	
	private static AIManager instance;
	private List<PlayerInput> masters;
	//by default, does nothing
	
	int nextID = 1;
	int numRemoved = 0;
	
	// added by Mike D 12:40 am sat
	private List<CivilianInput> civilians;
	
	// Init the instance regardless of whether the script is enabled.
	public AIManager()
	{
		masters = new List<PlayerInput>();
		civilians = new List<CivilianInput>();
		//also attach to event instance
	}
	
	public static AIManager GetInstance()
	{
		if(instance == null)
		{
			Debug.Log("Creating AIManager instance...");
			instance = new AIManager();
		}
		return instance;
	}

	public void AddMaster(PlayerInput pMaster)
	{
		if (masters.Contains(pMaster))
		{
			masters.Add(pMaster);
		}
	}

	public void RemoveMaster(PlayerInput pMaster)
	{
		if (masters.Contains(pMaster))
		{
			masters.Remove(pMaster);
		}
	}

	public void AddCivilian(CivilianInput pCiv)
	{
		if (civilians.Contains(pCiv))
		{
			civilians.Add(pCiv);
		}
	}
	
	public void RemoveCivilian(CivilianInput pCiv)
	{
		if (civilians.Contains(pCiv))
		{
			civilians.Remove(pCiv);
		}
	}

	public PlayerInput GetNearestMaster(Vector3 pos)
	{
		PlayerInput result = null;
		float closestPlayerDistance = -1;
		float nextDistance;
		foreach (PlayerInput player in masters)
		{
			nextDistance = Vector3.SqrMagnitude(player.transform.position - pos);
			if (closestPlayerDistance < 0 || nextDistance < closestPlayerDistance)
			{
				result = player;
				closestPlayerDistance = nextDistance;
			}
		}
		return result;
	}
}