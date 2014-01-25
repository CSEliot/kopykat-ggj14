using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {
	
	//bits of stuff flying off
	public GameObject Shrapnel;
	public float MaxLifeTime = 1.0f;
	public float ShrapLaunchRate = 1.0f;
	public float ShrapVelocity = 1.0f;
	
	private float lastLaunchTime = 0.0f;
	private float lifeTime = 0.0f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		lastLaunchTime +=  Time.deltaTime;
		lifeTime += Time.deltaTime;
		if(lastLaunchTime >= ShrapLaunchRate)
		{
			//get the direction of a submunition
			Vector3 shrapVel = Random.insideUnitSphere;
			//needs to be going UP, not DOWN
			shrapVel.y = Mathf.Abs(shrapVel.y);
			GameObject newShrap = (GameObject)Instantiate(Shrapnel, transform.position, Quaternion.identity);
			newShrap.rigidbody.velocity = shrapVel * ShrapVelocity;
			lastLaunchTime = 0;
		}
		if(lifeTime >= MaxLifeTime)
		{
			Destroy (gameObject);
		}
	}
}
