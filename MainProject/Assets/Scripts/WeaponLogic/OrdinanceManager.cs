using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class OrdinanceManager {
	
	private static List<GameObject> missiles = new List<GameObject>();
	//also keep a map linking missiles to their targets
	private static Dictionary<int, Dictionary<int, GameObject>> targetsToMissiles = new Dictionary<int, Dictionary<int, GameObject>>();
	//private static List<GameObject> bullets = new List<GameObject>(); //not sure we need to track every bullet at the moment
	private static List<GameObject> miscRounds = new List<GameObject>();
	
	public static GameObject AddRound(Ordinance ordinance, Vector3 firingPoint, Quaternion heading, GameObject target, GameObject launcher)
	{
		GameObject newRound = (GameObject)GameObject.Instantiate(ordinance.gameObject, firingPoint, heading);
		switch (ordinance.Type)
		{
			case Ordinance.OrdinanceType.Seeker:
			{
				missiles.Add(newRound);
				if(target != null)
				{
					//target-to-missile lists won't exist at first, so build them as needed
					int targetID = target.GetInstanceID();
					if(!targetsToMissiles.ContainsKey(targetID))
					{
						targetsToMissiles.Add(targetID, new Dictionary<int, GameObject>());
					}
					targetsToMissiles[targetID].Add(newRound.GetInstanceID(), newRound);
				}
				newRound.GetComponent<Ordinance>().Launcher = launcher;
				break;
			}
			case Ordinance.OrdinanceType.Dumbfire:
			{
				//bullets.Add(newRound);
				break;
			}
			default:
			{
				miscRounds.Add(newRound);
				newRound.GetComponent<Ordinance>().Launcher = launcher;
				break;
			}
		}
		return newRound;
	}
	
	public static GameObject AddRound(Ordinance ordinance, Vector3 firingPoint, Quaternion heading, GameObject launcher)
	{
		return AddRound(ordinance, firingPoint, heading, null, launcher);
	}
	
	//removes and destroys a round from tracking
	public static void DestroyRoundOfType(Ordinance.OrdinanceType type, GameObject ordinance)
	{
		switch (type)
		{
			case Ordinance.OrdinanceType.Seeker:
			{
				missiles.Remove(ordinance);
				//also find the target and remove from targetToMissile dictionary
				GameObject target = ordinance.GetComponent<Ordinance>().Target;
				if(target != null)
				{
					int targetID = target.GetInstanceID();
					if(targetsToMissiles.ContainsKey(targetID))
					{
						targetsToMissiles[targetID].Remove(ordinance.GetInstanceID());
					}
				}
				break;
			}
			case Ordinance.OrdinanceType.Dumbfire:
			{
				//ordinanceList = bullets;
				//break;
				return;
			}
			default:
			{
				miscRounds.Remove(ordinance);
				break;
			}
		}
		GameObject.Destroy(ordinance);
	}
	
	public static void DestroyRound(GameObject ordinance)
	{
		//first search missiles, then misc rounds
		if(missiles.Contains(ordinance))
		{
			missiles.Remove(ordinance);
			//also find the target and remove from targetToMissile dictionary
			GameObject target = ordinance.GetComponent<Ordinance>().Target;
			if(target != null)
			{
				int targetID = target.GetInstanceID();
				if(targetsToMissiles.ContainsKey(targetID))
				{
					targetsToMissiles[targetID].Remove(ordinance.GetInstanceID());
				}
			}
		}
		else if(miscRounds.Contains(ordinance))
		{
			miscRounds.Remove(ordinance);
		}
		GameObject.Destroy(ordinance);
	}
	
	public static Dictionary<int, GameObject> GetMissilesTrackingTarget(GameObject target)
	{
		if(target != null)
		{
			int targetID = target.GetInstanceID();
			if(!targetsToMissiles.ContainsKey(targetID))
			{
				targetsToMissiles[targetID] = new Dictionary<int, GameObject>();
			}
			return targetsToMissiles[targetID];
		}
		return new Dictionary<int, GameObject>();
	}
	
	public static List<GameObject> GetMissilesOfLauncher(GameObject launcher)
	{
		List<GameObject> launcherMissiles = new List<GameObject>();
		foreach(GameObject mssl in missiles)
		{
			if(mssl.GetComponent<Ordinance>().Launcher.GetInstanceID() == launcher.GetInstanceID())
			{
				launcherMissiles.Add(mssl);
			}
		}
		return launcherMissiles;
	}
	
	public static void ChangeMissileTarget(GameObject mssl, GameObject newTarget)
	{
		Ordinance msslProps = mssl.GetComponent<Ordinance>();
		int origTargetID = msslProps.Target != null ? msslProps.Target.GetInstanceID() : -1;
		int newTargetID = newTarget.GetInstanceID();
		if(msslProps != null) //&& newTargetID != origTargetID)
		{
			int msslID = mssl.GetInstanceID();
			//find the missle's entry in the targets to missiles,
			//and then move it to the dict for the new target
			if(origTargetID != -1)
			{
				targetsToMissiles[origTargetID].Remove(msslID);
			}
			if(!targetsToMissiles.ContainsKey(newTargetID))
			{
				targetsToMissiles[newTargetID] = new Dictionary<int, GameObject>();
			}
			//if(!targetsToMissiles[newTargetID].ContainsKey(new
			targetsToMissiles[newTargetID].Add(msslID, mssl);
			msslProps.Target = newTarget;
		}
	}
	
	//returns 0.0 if there's no missiles tracking target
	public static float GetMissileVolumeTrackingTarget(GameObject target)
	{
		float volume = 0.0f;
		if(target != null)
		{
			float minX = float.MaxValue;
			float minY = float.MaxValue;
			float minZ = float.MaxValue;
			float maxX = -1.0f;
			float maxY = -1.0f;
			float maxZ = -1.0f;
			int targetID = target.GetInstanceID();
			if(!targetsToMissiles.ContainsKey(targetID))
			{
				return 0.0f;
			}
			foreach(GameObject mssl in targetsToMissiles[targetID].Values)
			{
				Vector3 msslPos = mssl.transform.position;
				minX = Mathf.Min(msslPos.x, minX);
				minY = Mathf.Min(msslPos.y, minY);
				minZ = Mathf.Min(msslPos.z, minZ);
				maxX = Mathf.Max(msslPos.x, maxX);
				maxY = Mathf.Max(msslPos.y, maxY);
				maxZ = Mathf.Max(msslPos.z, maxZ);
			}
			float boundsX = maxX - minX;
			float boundsY = maxY - minY;
			float boundsZ = maxZ - minZ;
			volume = boundsX * boundsY * boundsZ;
		}
		return volume;
	}
	
	public static GameObject FindMissileByTargetAndID(GameObject target, int msslID)
	{
		Dictionary<int, GameObject> msslList = GetMissilesTrackingTarget(target);
		if(msslList.Count > 0 && msslList.ContainsKey(msslID))
		{
			return msslList[msslID];
		}
		return null;
	}
}
