using UnityEngine;
using System.Collections;

public class LifeTime : MonoBehaviour {

	public float MaxLifeTime = 1.0f;

	private float lifeTime = 0.0f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		lifeTime += Time.deltaTime;
		if(lifeTime >= MaxLifeTime)
		{
			Destroy (gameObject);
			Destroy(this);
		}
	}
}
