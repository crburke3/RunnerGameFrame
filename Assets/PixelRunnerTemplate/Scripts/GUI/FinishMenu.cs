using UnityEngine;
using System.Collections;

// Finish menu class
// This handles the display and logic for the end screen.

public class FinishMenu : MonoBehaviour 
{
	public GameObject gameOverText;						// "Game Over" text object
	public GameObject score;							// Score object
	public GameObject exitButton;						// Menu item to return to the title screen
	public GameObject restartButton;					// Menu item to restart the game
	public GameObject screenFade;						// global screen fade object
	public TextMesh distanceText;						// Distance text for the last attempt
	public TextMesh bestText;							// Distance text for the best ever score

	private Color fadeCol = new Color(0,0,0, 0.5f);		// 50% transparent black to dim the screen

	public void Activate()
	{
		// fill in score text
		distanceText.text = GameManager.Game.LastScore.ToString() + "m";
		bestText.text = GameManager.Game.BestScore.ToString() + "m";

		// animate all the elements onto the screen
		MenuAnim.FadeTo(screenFade, fadeCol, 0.3f, 0.0f);
		MenuAnim.MoveTo(gameOverText, new Vector3(0, 0.5f, 0), 0.4f, 0.0f);
		MenuAnim.MoveTo(score, new Vector3(0, 0, 0), 0.4f, 0.1f);
		MenuAnim.MoveTo(exitButton, new Vector3(-1.35f, -1.5f, 0), 0.4f, 0.3f);
		MenuAnim.MoveTo(restartButton, new Vector3(1.35f, -1.5f, 0), 0.4f, 0.3f);

		// fade out the music
		SoundManager.PauseMusic(0.5f);
	}

	public void OnRestart()
	{
		// animate all the elements out again
		MenuAnim.MoveTo(gameOverText, new Vector3(0, 5, 0), 0.4f, 0.1f);
		MenuAnim.MoveTo(score, new Vector3(0, -4, 0), 0.4f, 0.1f);
		MenuAnim.MoveTo(exitButton, new Vector3(-10, -1.5f, 0), 0.4f, 0.0f);
		MenuAnim.MoveTo(restartButton, new Vector3(10, -1.5f, 0), 0.4f, 0.0f);
		// fade to black
		MenuAnim.FadeTo(screenFade, Color.black, 0.3f, 0.1f);
		// restart the game once the screen is black
		GameManager.Game.Invoke("Restart", 0.7f);
		// fade back in as the game starts
		MenuAnim.FadeTo(screenFade, Color.clear, 0.3f, 0.7f);
	}

	public void OnBackToMenu()
	{
		// animate the elements out
		MenuAnim.MoveTo(gameOverText, new Vector3(0, 5, 0), 0.4f, 0.1f);
		MenuAnim.MoveTo(score, new Vector3(0, -4, 0), 0.4f, 0.1f);
		MenuAnim.MoveTo(exitButton, new Vector3(-10, -1.5f, 0), 0.4f, 0.0f);
		MenuAnim.MoveTo(restartButton, new Vector3(10, -1.5f, 0), 0.4f, 0.0f);
		// fade to black (the title screen handles fading back in)
		MenuAnim.FadeTo(screenFade, Color.black, 0.3f, 0.1f);
		// show the title screen once the screen is black
		GameManager.Game.Invoke("ShowTitleScreen", 0.7f);
	}
}
