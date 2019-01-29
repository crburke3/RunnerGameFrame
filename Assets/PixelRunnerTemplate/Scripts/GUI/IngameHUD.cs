using UnityEngine;
using System.Collections;

// This class handles the display of the ingame HUD (currently just the pause button)

public class IngameHUD : MonoBehaviour 
{
	public GameObject pauseButton;		// the pause button object
	public PauseMenu pauseMenu;			// the pause menu

	void Start()
	{
		// position the pause button in the top right corner of the screen
		Vector3 pausePos = pauseButton.transform.position;
		float screenRightEdge = Camera.main.aspect * Camera.main.orthographicSize;
		pausePos.x = screenRightEdge - 0.5f;
		pauseButton.transform.position = pausePos;
	}

	void OnPause()
	{
		// pause button was tapped, pause the game and show the pause menu
		GameManager.Game.IsPaused = true;
		pauseMenu.Activate();
	}

	void Update()
	{
		// don't show the pause button when the game is already paused.
		bool showPauseButton = !GameManager.Game.IsPaused && !GameManager.Game.IsGameOver;
		if(showPauseButton != pauseButton.activeSelf)
			pauseButton.SetActive(showPauseButton);
	}
}
