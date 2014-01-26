using UnityEngine;
using System.Collections;

//a little bare bones right now,
//but at least it can be expanded.
namespace KopyKat
{
    public enum EventType 
    {
	    Shiv,
	    HandsUp,
	    Move,
	    Stop,
	    Jump,
	    PanicStart,
	    PanicEnd,
	    ActorKilled,
	    CorpseGone,
	    INVALID_EVENT
    };
}