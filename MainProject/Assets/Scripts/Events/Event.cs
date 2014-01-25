using UnityEngine;
using System.Collections;

//A generic event.
//Must hold a type at the very least, subclass to hold data.
//Since C# doesn't easily let you reinterpret raw data,
//you'll need to cast the event to the desired type instead.
//Use IEvent.Type to determine what to cast to.
public interface IEvent
{
	EventType Type { get; }
};


public class ActorKilledEvent : IEvent {
	private GameObject victim, killer;
	
	public EventType Type { get { return EventType.ActorKilled; } }
	public GameObject Victim { get { return victim; } }
	public GameObject Killer { get { return killer; } }
	
	public ActorKilledEvent(GameObject victimParam, GameObject killerParam)
	{
		
		victim = victimParam;
		killer = killerParam;
	}
}
