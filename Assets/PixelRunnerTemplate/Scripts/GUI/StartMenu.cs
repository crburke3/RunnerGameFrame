using UnityEngine;
using System.Collections;

// Start menu class
// This handles the display and logic for the title screen.

public class StartMenu : MonoBehaviour 
{
	public GameObject title;			// the title graphic
	public GameObject playerImage;		// the large player graphic
	public GameObject tapToStart;		// the 'tap to start' text
	public GameObject screenFade;		// the screen fade object, so we can dim the screen

	// colour for the screen fade whilst the start menu is active
	private Color fadeCol = new Color(0,0,0, 0.25f);

	void Start()
	{
		// set the screen fade colour
		screenFade.GetComponent<SpriteRenderer>().color = Color.black;
		screenFade.GetComponent<Renderer>().material.color = fadeCol;

		SoundManager.PlayMusic("Intro");
	}

	public void Activate()
	{
		SoundManager.PlayMusic("Intro");

		// animate in all the elements, and dim the screen.
		MenuAnim.MoveTo(title, new Vector3(1, 0.7f, 0), 0.5f, 0.0f);
		MenuAnim.MoveTo(playerImage, new Vector3(-4, -1, 0), 0.5f, 0.0f);
		MenuAnim.MoveTo(tapToStart, new Vector3(0, -2.5f, 0), 0.3f, 0.5f);
		MenuAnim.FadeTo(screenFade, fadeCol, 0.3f, 0.2f);
	}

	void OnStartPressed()
	{
		// animate out all the elements
		MenuAnim.MoveTo(title, new Vector3(12, 0.7f, 0), 0.5f, 0.0f);
		MenuAnim.MoveTo(playerImage, new Vector3(-12, -1, 0), 0.5f, 0.0f);
		MenuAnim.MoveTo(tapToStart, new Vector3(0, -10, 0), 0.3f, 0.0f);
		MenuAnim.FadeTo(screenFade, Color.clear, 0.3f, 0.2f);
		// start the actual game
		GameManager.Game.Restart();
	}
}
