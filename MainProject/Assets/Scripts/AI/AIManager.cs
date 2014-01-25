using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Global AI manager.
/// </summary>
public class AIManager : MonoBehaviour {
	
	private static AIManager instance;
	private Dictionary<int, MonoBehaviour> aiAgents;
	private List<SpawnPoint> spawners;
	private bool agentsEnabledStatus;
	//by default, does nothing
	private IAIMgrBehaviour managerBehaviour = new DefaultBehaviour();
	public enum BehaviourMode { Default, Horde };
	public BehaviourMode behaviourMode = BehaviourMode.Default;
	
	int nextID = 1;
	int numRemoved = 0;
	
	public bool AgentsEnabled
	{
		get { return agentsEnabledStatus; }
	}
	
	public static AIManager GetInstance()
	{
		if(instance == null)
		{
			Debug.Log("!AIManager: error: no instance found! Is an AI_MANAGER object in the scene hierarchy?");
		}
		return instance;
	}
	
	public void SetManagerBehaviour(IAIMgrBehaviour behaviour)
	{
		managerBehaviour = behaviour;
	}
	
	public int AgentCount()
	{
		return aiAgents.Count;
	}
	
	/// <summary>
	/// Adds the agent and return the agent's ID in the AIManager.
	/// </summary>
	public int AddAgent(MonoBehaviour agent)
	{
		Debug.Log("AIManager: added agent " + agent);
		int id = nextID;
		aiAgents.Add(id, agent);
		nextID++;
		//List.Add() always adds to the end of a list, so the new agent's ID will just be its index, I guess?
		return id;
	}
	
	public void RemoveAgent(int agentID)
	{
		if(aiAgents.ContainsKey(agentID))
		{
			Debug.Log("Removing " + agentID.ToString());
			aiAgents.Remove(agentID);
			numRemoved++;
			Debug.Log ("Removed " + numRemoved.ToString());
		}
	}
	
	public MonoBehaviour GetAgent(int agentID)
	{
		if(aiAgents.ContainsKey(agentID))
		{
			return aiAgents[agentID];
		}
		return null;
	}
	
	/// <summary>
	/// Enables or disables all agents.
	/// </summary>
	public void SetAgentsEnabled (bool val)
	{
		foreach(MonoBehaviour agent in aiAgents.Values)
		{
			agent.enabled = val;
		}
		agentsEnabledStatus = val;
	}
	
	public void ToggleAgentsEnabled()
	{
		SetAgentsEnabled(!agentsEnabledStatus);
	}
	
	public int SpawnerCount()
	{
		return spawners.Count;
	}
	
	public int AddSpawner(SpawnPoint spawnPoint)
	{
		Debug.Log("AIManager: added spawner " + spawnPoint);
		spawners.Add(spawnPoint);
		return spawners.Count - 1;
	}
	
	public void RemoveSpawner(int spawnID)
	{
		if(spawnID < spawners.Count)
		{
			spawners.RemoveAt(spawnID);
		}
	}
	
	public SpawnPoint GetSpawner(int spawnID)
	{
		if(spawnID < spawners.Count)
		{
			return spawners[spawnID];
		}
		return null;
	}
	
	// Init the instance regardless of whether the script is enabled.
	void Awake()
	{
		aiAgents = new Dictionary<int, MonoBehaviour>();
		spawners = new List<SpawnPoint>();
		agentsEnabledStatus = true;
		switch(behaviourMode)
		{
			case BehaviourMode.Default:
			{
				SetManagerBehaviour(new DefaultBehaviour());
				break;
			}
			case BehaviourMode.Horde:
			{
				SetManagerBehaviour(new HordeSurvivalBehaviour());
				break;
			}
		}
		instance = this;
		//also attach to event instance
	}
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		managerBehaviour.Update(this);
	}
	
	private bool checkPlayerAttacking()
	{
		foreach(MonoBehaviour agent in aiAgents.Values)
		{
			EnemyInput recastAgent = (EnemyInput)agent;
			if(recastAgent.NumMissilesToIntercept > 0)
			{
				return true;
			}
		}
		return false;
	}
}