using UnityEngine;
using System.Collections;

//holds global commands on the game engine, as well as game state.
public static class GameSystem {
	
	public static int MainMenuSceneIndex = 0;
	public enum GameState { Menu, Active, Dead };
	private static GameState currState = GameState.Menu;
	
	public static GameState State
	{
		get { return currState; }
		set { currState = value; }
	}
	
	public static bool GamePaused
	{
		get { return Time.timeScale == 0.0f; }
		set
		{
			if(value != GamePaused)
			{
				ToggleGamePaused();
			}
		}
	}
	
	public static void ToggleGamePaused()
	{
		//toggle gametime
		float newTime = 1.0f - Time.timeScale;
		Time.timeScale = newTime;
		//find audio listener, and toggle it
		AudioListener audioDev = (AudioListener)Object.FindObjectOfType(typeof(AudioListener));
		if(newTime == 0.0f)
		{
			audioDev.enabled = false;
		}
		else
		{
			audioDev.enabled = true;
		}
		//AudioListener.volume = 1.0f - AudioListener.volume;
	}
}
