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