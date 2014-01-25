using UnityEngine;
using System.Collections;

//Interface class for scripts interested in events.
//To handle an event, implement OnEvent and return true if the event was handled by the script,
//false otherwise.
public interface IEventListener {
	bool OnEvent(IEvent eventInstance);
}
