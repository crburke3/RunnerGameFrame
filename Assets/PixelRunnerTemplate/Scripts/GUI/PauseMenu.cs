using UnityEngine;
using System.Collections;

// Start menu class
// This handles the display and logic for the pause screen.

public class PauseMenu : MonoBehaviour 
{
	public GameObject pausedText;						// 'Paused' text object
	public GameObject tapToResume;						// 'Tap to resume' object
	public GameObject screenFade;						// global screen fade object

	private Color fadeCol = new Color(0,0,0, 0.5f);		// 50% transparent black to dim the screen

	public void Activate()
	{
		// animate all the elements into place
		MenuAnim.MoveTo(pausedText, new Vector3(0, 1, 0), 0.4f, 0.0f);
		MenuAnim.MoveTo(tapToResume, new Vector3(0, -1, 0), 0.4f, 0.0f);
		// dim the screen
		MenuAnim.FadeTo(screenFade, fadeCol, 0.3f, 0.0f);
		// pause the music
		SoundManager.PauseMusic(0.5f);
	}

	void OnResume()
	{
		if(GameManager.Game.IsPaused)
		{
			// animate everything offscreen again
			MenuAnim.MoveTo(pausedText, new Vector3(-12, 1, 0), 0.3f, 0.0f);
			MenuAnim.MoveTo(tapToResume, new Vector3(12, -1, 0), 0.3f, 0.0f);
			// undim the screen
			MenuAnim.FadeTo(screenFade, Color.clear, 0.3f, 0.1f);
			// start the music again
			SoundManager.UnpauseMusic();
			// resume the game
			GameManager.Game.IsPaused = false;
		}
	}
}
