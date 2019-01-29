using UnityEngine;
using System.Collections;

// The GameManager class handles the overall game state and the player score etc.

public class GameManager : MonoBehaviour 
{
	public PlayerControl player;			// the player object
	public LevelManager levelManager;		// the level manager
	public StartMenu startMenu;				// the start menu object
	public PauseMenu pauseMenu;				// the pause menu object
	public FinishMenu finishMenu;			// the finish menu object
	public Font GUIFont;					// the font to use for the GUI

	public LayerMask GUILayerMask;			// a layer mask that identifies the GUI elements

	public static GameManager Game;			// a singleton instance to allow classes to access the global GameManager

	private bool isPaused = true;			// is the game currently paused
	private bool isGameOver = false;		// are we on the finish screen
	private int lastScore = 0;				// the player's score for the previous attempt
	private int bestScore = 0;				// the player's best ever score

	// get the score for the last completed attempted
	public int LastScore
	{
		get { return lastScore; }
	}

	// get the player's best score of all time
	public int BestScore
	{
		get { return bestScore; }
	}

	// get the player score for the current (in progress) attempt
	public int InProgressScore
	{
		get { return (int)levelManager.TotalDistance; }
	}

	void Awake () 
	{
		// check there isn't more than one instance of the GameManager in the scene
		if(Game != null)
		{
			Debug.LogError("More than one GameManager found in the scene");
			return;
		}

		// set the global instance
		Game = this;

		// set the font to be point filtered so it's nice and sharp
		GUIFont.material.mainTexture.filterMode = FilterMode.Point;

		// retrieve the player's best score
		bestScore = PlayerPrefs.GetInt("BestScore", 0);
	}

	public bool IsPaused
	{
		get { return isPaused; }
		set 
		{ 
			isPaused = value;
			Time.timeScale = isPaused ? 0.0f : 1.0f;
		}
	}

	public bool IsGameOver
	{
		get { return isGameOver; }
	}

	public void OnPlayerDeath()
	{
		// update best score
		lastScore = (int)levelManager.TotalDistance;
		if(lastScore > bestScore)
		{
			bestScore = lastScore;
			PlayerPrefs.SetInt("BestScore", bestScore);
		}

		isGameOver = true;
		finishMenu.Activate();
	}

	public void ShowTitleScreen()
	{
		// reset everything to initial state
		levelManager.Restart();
		IsPaused = true;
		// show the title screen
		startMenu.Activate();
	}

	public void Restart()
	{
		// reset everything to initial state
		levelManager.Restart();
		player.Restart();
		// start playing immediately
		IsPaused = false;
		isGameOver = false;

		// play the in-game music track
		SoundManager.PlayMusic("GameLoop");
	}

	private void UpdateInput()
	{
		bool tapped = false;
		Vector3 tapPos = Vector3.zero;

		// check for mouse input
		if(Input.GetMouseButtonDown(0))
		{
			tapped = true;
			tapPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		}

		// check for touch input
		foreach (Touch touch in Input.touches) 
		{
			if (touch.phase == TouchPhase.Began)
			{
				tapped = true;
				tapPos = Camera.main.ScreenToWorldPoint(touch.position);
			}
		}

		// respond to any mouse-down or touch events
		if(tapped)
		{
			// check if there's any gui elements at the tap position
			RaycastHit2D hit = Physics2D.Raycast(tapPos, Vector2.zero, 1.0f, GUILayerMask);
			if (hit.collider != null)
			{
				MenuItem menuItem = hit.collider.gameObject.GetComponent<MenuItem>();
				if(menuItem != null)
				{
					// play the sound effect if specified
					if(menuItem.sfxName != "")
						SoundManager.PlaySfx(menuItem.sfxName);

					// call the menu item's handler function, we use SendMessageUpwards because the handler
					// might be declared inside the parent object's script
					menuItem.SendMessageUpwards(menuItem.handler);
				}
			}
			else if(!IsPaused)
			{
				// no gui element under the tap position, so send it to the player to handle
				player.HandleTap();
			}
		}
	}

	void Update () 
	{
		UpdateInput ();
	}
}
