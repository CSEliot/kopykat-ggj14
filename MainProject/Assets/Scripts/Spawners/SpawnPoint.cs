using UnityEngine;
using System.Collections;

/// <summary>
/// Point from which GameObjects can be instantiated on command ('spawned'). Can spawn either at the SpawnPoint's position or a position parameter.
/// </summary>
public class SpawnPoint : MonoBehaviour {
	
	public GameObject ObjectToSpawn;
	
	public void Spawn()
	{
		if(ObjectToSpawn != null)
		{
			Instantiate(ObjectToSpawn, transform.position, transform.rotation);
		}
	}
	
	public void Spawn(Vector3 spawnPos)
	{
		if(ObjectToSpawn != null)
		{
			Instantiate(ObjectToSpawn, spawnPos, Quaternion.identity);
		}
	}
	
	// Use this for initialization
	void Start () {
		//let the AI manager know the spawner exists
		AIManager.GetInstance().AddSpawner(this);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
