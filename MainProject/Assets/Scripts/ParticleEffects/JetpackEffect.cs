using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//given a set of particle emitters and a ActorController,
//controls the emission rate of the emitters by the force being imposed on the actor by the ActorController.
public class JetpackEffect : MonoBehaviour, IActorControllerListener {
	
	public List<ParticleSystem> Emitters;
	public ActorController ActorCtrl;
	
	protected Dictionary<ParticleSystem, float> defaultEmissionRates;
	
	// Use this for initialization
	void Start () {
		if(ActorCtrl != null)
		{
			ActorCtrl.AttachListener(this);
		}
		
		defaultEmissionRates = new Dictionary<ParticleSystem, float>();
		foreach(ParticleSystem emitter in Emitters)
		{
			defaultEmissionRates.Add(emitter, emitter.emissionRate);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void OnActorMove(Vector3 forceVec)
	{
		foreach(ParticleSystem emitter in Emitters)
		{
			//scale by our acceleration up or down; clamp to a nonnegative value
			emitter.emissionRate = Mathf.Max((defaultEmissionRates[emitter] * forceVec.normalized.y), 0.0f);
		}
	}
	
	protected void onRemove()
	{
		if(ActorCtrl != null)
		{
			ActorCtrl.RemoveListener(this);
		}
	}
}
