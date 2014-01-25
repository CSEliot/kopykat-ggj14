using UnityEngine;
using System.Collections;

public class UIGameplayMenuLogic : MonoBehaviour {
	
	public GUIStyle Style;
	
	private bool pauseMenuVisible = false;
	private bool deadMenuVisible = false;
	
	//properties
	public bool PauseMenuVisible
	{
		get { return pauseMenuVisible; }
		set { pauseMenuVisible = value; }
	}
	
	public bool DeadMenuVisible
	{
		get { return deadMenuVisible; }
		set { deadMenuVisible = value; }
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI()
	{
		switch(GameSystem.State)
		{
			case GameSystem.GameState.Dead:
			{
				DrawDeadMenu();
				break;
			}
			default:
			{
				if(GameSystem.GamePaused)
				{
					DrawPauseMenu();
				}
				break;
			}
		}
	}
	
	void DrawPauseMenu()
	{
		GameSystem.GamePaused = true;
		//we want to be able to click the menu buttons...
		Screen.lockCursor = false;
		//put a group in the center of the screen to orient the other UI elements
		GUI.BeginGroup(new Rect ((Screen.width - UIConstants.MenuBGWidth) / 2, (Screen.height - UIConstants.MenuBGHeight)/ 2, UIConstants.MenuBGWidth, UIConstants.MenuBGHeight));
		//draw menu background
		GUI.Box(new Rect(0, 0, UIConstants.MenuBGWidth, UIConstants.MenuBGHeight), "Paused");
		//then draw buttons
		if(GUI.Button(new Rect(UIConstants.ButtonSpacing, UIConstants.MenuBGHeight / 2.0f, UIConstants.ButtonWidth, UIConstants.ButtonHeight), "Resume"))
		{
			//GameSystem.ToggleGamePaused();
			GameSystem.GamePaused = false;
			//relock cursor so it's not moving around on the screen!
			Screen.lockCursor = true;
		}
		if(GUI.Button(new Rect(UIConstants.ButtonSpacing, (UIConstants.MenuBGHeight / 2.0f) + UIConstants.ButtonHeight, UIConstants.ButtonWidth, UIConstants.ButtonHeight), "Quit"))
		{
			GameSystem.State = GameSystem.GameState.Menu;
			Application.LoadLevel(GameSystem.MainMenuSceneIndex);
		}
		GUI.EndGroup();
	}
	
	void DrawDeadMenu()
	{
		//we want to be able to click the menu buttons...
		Screen.lockCursor = false;
		//put a group in the center of the screen to orient the other UI elements
		GUI.BeginGroup(new Rect ((Screen.width - UIConstants.MenuBGWidth) / 2, (Screen.height - UIConstants.MenuBGHeight)/ 2, UIConstants.MenuBGWidth, UIConstants.MenuBGHeight));
		//draw menu background
		GUI.Box(new Rect(0, 0, UIConstants.MenuBGWidth, UIConstants.MenuBGHeight), "You Died!");
		//then draw buttons
		if(GUI.Button(new Rect(UIConstants.ButtonSpacing, UIConstants.MenuBGHeight / 2.0f, UIConstants.ButtonWidth, UIConstants.ButtonHeight), "Restart"))
		{
			//reset state and reload level
			GameSystem.State = GameSystem.GameState.Active;
			Application.LoadLevel(Application.loadedLevel);
		}
		if(GUI.Button(new Rect(UIConstants.ButtonSpacing, (UIConstants.MenuBGHeight / 2.0f) + UIConstants.ButtonHeight, UIConstants.ButtonWidth, UIConstants.ButtonHeight), "Quit"))
		{
			GameSystem.State = GameSystem.GameState.Menu;
			Application.LoadLevel(GameSystem.MainMenuSceneIndex);
		}
		GUI.EndGroup();
	}
}
