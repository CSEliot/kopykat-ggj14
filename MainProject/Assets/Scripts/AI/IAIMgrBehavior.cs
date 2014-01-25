using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Given an AIManager, can control agents and spawners.
/// </summary>
public interface IAIMgrBehaviour
{
	void Update(AIManager manager);
}

public class DefaultBehaviour : IAIMgrBehaviour
{
	//do nothing!
	public void Update(AIManager manager) {}
}

public class HordeSurvivalBehaviour : IAIMgrBehaviour
{
	const int MAX_ENEMIES = 7;
	const int MIN_ENEMIES = 3;
	int currWave = 0;
	int enemiesInWave = MIN_ENEMIES;
	float difficulty = 0.5f;
	
	private int getEnemiesInWave(int wave)
	{
		//not too sure about what kind of curve I want to use...
		//for now, just make it linear.
		//round down when possible.
		int newNumEnemies = Mathf.RoundToInt(difficulty*currWave + MIN_ENEMIES);
		return Mathf.Min(MAX_ENEMIES, newNumEnemies);
	}
	
	public void Update(AIManager manager)
	{
		//try to keep the number of enemies at a constant level
		if(manager.AgentCount() <= 0)
		{			
			//and update the number of enemies
			enemiesInWave = getEnemiesInWave(currWave);
			//wtf?
			//So it turns out Debug.Log() is part of MonoBehaviour.
			//Since this isn't a MB, we can't call Debug.Log().
			//From now on, I'm using this to be clear what the hell's going on.
			Debug.Log("On wave " + currWave.ToString() + ". Num enemies: " + enemiesInWave.ToString());
			//get a random spawner and spawn an enemy
			//for now, all other actors are enemies
			for(int i = 0; i < enemiesInWave; ++i)
			{
				manager.GetSpawner(Random.Range(0, manager.SpawnerCount())).Spawn();
			}
			
			//move onto the next wave
			currWave++;
		}
	}
}
