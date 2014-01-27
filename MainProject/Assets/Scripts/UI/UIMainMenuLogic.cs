using UnityEngine;
using System.Collections;

namespace KopyKat
{
    public class UIMainMenuLogic : MonoBehaviour
    {

        public GUIStyle Style;
        public string LevelToLoad;

        // Use this for initialization
        void Start()
        {
            GameSystem.GamePaused = false;
        }

        void startDemoLevel()
        {
            Application.LoadLevel(LevelToLoad);
        }

        void showOptionMenu()
        {
        }

        void quitGame()
        {
            Application.Quit();
        }

        void OnGUI()
        {

            //add the menu buttons
            //remember to make independent of resolution
            if (GUI.Button(new Rect(Screen.width * 0.05f, Screen.height * 0.5f, UIConstants.ButtonWidth, UIConstants.ButtonHeight), "Start Demo", Style))
            {
                GameSystem.State = GameSystem.GameState.Active;
                //GameSystem.GamePaused = false;
                startDemoLevel();
            }

            if (GUI.Button(new Rect(Screen.width * 0.05f, Screen.height * 0.55f, UIConstants.ButtonWidth, UIConstants.ButtonHeight), "Options", Style))
            {
                showOptionMenu();
                Debug.Log("Clicked option button");
            }

            if (GUI.Button(new Rect(Screen.width * 0.05f, Screen.height * 0.6f, UIConstants.ButtonWidth, UIConstants.ButtonHeight), "Quit Game", Style))
            {
                quitGame();
            }
        }
    }
}