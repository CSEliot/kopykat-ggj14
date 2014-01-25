using UnityEngine;
using System.Collections;

public class HealthInfo : MonoBehaviour {
	
	public float StartingHealth = 100.0f;
	public bool GodMode = false;
	
	public float Health
	{
		get { return currHealth; }
	}
	public bool IsAlive
	{
		get { return currHealth > 0.0f; }
	}
	
	private float currHealth;
	
	public void ApplyDamage(float damage, GameObject attacker)
	{
		if(!GodMode && IsAlive)
		{
			//we can't attack ourselves!
			if(attacker != gameObject)
			{
				currHealth -= damage;
	            //add a check afterward;
	            //this lets us check kills
	            if (!IsAlive)
	            {
	                //Events.ReportEvent(ACTOR_KILLED, gameObject.GetInstanceID(), attacker.GetInstanceID());
					ActorKilledEvent actKilled = new ActorKilledEvent(gameObject, attacker);
					EventManager.TriggerEvent(actKilled);
	            }
			}
		}
	}
	
	// Use this for initialization
	void Start () {
		currHealth = StartingHealth;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
