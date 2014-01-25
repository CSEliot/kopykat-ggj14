using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Global AI manager.
/// </summary>
public class AIManager : MonoBehaviour, IEventListener
{
	
	private static AIManager instance;
	private List<PlayerInput> masters;
	private List<CivilianInput> civilians;
	
	private int numCorpses;
    //The amount of time all AI should continue to hold up their hands.
    private float handsUpTimer = 0;
    private const float HANDS_UP_MAX_TIME = 5;

    public bool ShouldPanic { get { return numCorpses > 0; } }
    public bool ShouldHandsUp { get { return handsUpTimer > 0; } }

	private void subscribeToEventMgr()
	{
		EventManager.AddListener(EventType.ActorKilled, this);
		EventManager.AddListener(EventType.CorpseGone, this);
		EventManager.AddListener(EventType.Shiv, this);
	}

	// Init the instance regardless of whether the script is enabled.
    void Awake()
    {
		masters = new List<PlayerInput>();
		civilians = new List<CivilianInput>();
		subscribeToEventMgr();
        instance = this;
	}

	public static AIManager GetInstance()
	{
		if(instance == null)
		{
			Debug.Log("Couldn't find AI manager! Is an AI_MANAGER object in the scene?");
		}
		return instance;
	}

	public void AddMaster(PlayerInput pMaster)
	{
		if (masters.Contains(pMaster))
		{
			masters.Add(pMaster);
		}
	}

	public void RemoveMaster(PlayerInput pMaster)
	{
		if (masters.Contains(pMaster))
		{
			masters.Remove(pMaster);
		}
	}

	public void AddCivilian(CivilianInput pCiv)
	{
		if (civilians.Contains(pCiv))
		{
			civilians.Add(pCiv);
		}
	}
	
	public void RemoveCivilian(CivilianInput pCiv)
	{
		if (civilians.Contains(pCiv))
		{
			civilians.Remove(pCiv);
		}
	}

	public PlayerInput GetNearestMaster(Vector3 pos)
	{
		PlayerInput result = null;
		float closestPlayerDistance = -1;
		float nextDistance;
		foreach (PlayerInput player in masters)
		{
			nextDistance = Vector3.SqrMagnitude(player.transform.position - pos);
			if (closestPlayerDistance < 0 || nextDistance < closestPlayerDistance)
			{
				result = player;
				closestPlayerDistance = nextDistance;
			}
		}
		return result;
	}
	
	public bool OnEvent(IEvent eventInstance)
	{
		switch(eventInstance.Type)
		{
            case EventType.ActorKilled:
            {
                numCorpses++;
                break;
            }
            case EventType.CorpseGone:
            {
                numCorpses--;
                break;
            }
            case EventType.Shiv:
            {
                //restart the hands up timer
                handsUpTimer = HANDS_UP_MAX_TIME;
                break;
            }
            default:
            {
                break;
            }
		}
        return true;
	}

    // Update is called once per frame
    void Update()
    {
        //count down any timers
        if (handsUpTimer > 0)
        {
            handsUpTimer -= Time.deltaTime;
        }
        else if (handsUpTimer < 0)
        {
            handsUpTimer = 0;
        }
    }
}