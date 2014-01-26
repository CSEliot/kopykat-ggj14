using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class EventManager {
	
	private static Dictionary<EventType, List<IEventListener>> eventListenerMap = new Dictionary<EventType, List<IEventListener>>();
	
	//event queues?
	
	public static void AddListener(EventType type, IEventListener listener)
	{
		//build the listener list if it doesn't exist yet
		if(!eventListenerMap.ContainsKey(type))
		{
			eventListenerMap[type] = new List<IEventListener>();
		}
		List<IEventListener> listenerList = eventListenerMap[type];
		//ensure this listener's not already subscribed
		if(!listenerList.Contains(listener))
		{
			//now add the listener
			listenerList.Add(listener);
		}
	}
	
	public static void RemoveListener(EventType type, IEventListener listener)
	{
		//no need to do anything if there's no listeners for this type
		if(!eventListenerMap.ContainsKey(type))
		{
			return;
		}
		eventListenerMap[type].Remove(listener);
	}
	
	//searches ALL lists and removes listener from all of them.
	//pretty slow op; remove from individual lists if you can
	public static void RemoveListener(IEventListener listener)
	{
		foreach(EventType type in eventListenerMap.Keys)
		{
			eventListenerMap[type].Remove(listener);
		}
	}
	
    [RPC]
	public static void TriggerEvent(IEvent eventInstance)
	{
		//first check that there's a list of listeners
		if(eventListenerMap.ContainsKey(eventInstance.Type))
		{
			foreach(IEventListener listener in eventListenerMap[eventInstance.Type])
			{
				listener.OnEvent(eventInstance);
			}
		}
	}

    //Sends an event to all connected systems via RPC.
    public static void TriggerNetworkEventAll(IEvent eventInstance)
    {
        //TODO
    }

	//TODO?
	/*
	void QueueEvent(IEvent eventInstance)
	{
	}*/
}
