using UnityEngine;
using System.Collections.Generic;

// Simple animation class
// Check out the various menu scripts (e.g. StartMenu.cs) to see example usage

public class MenuAnim : MonoBehaviour 
{
	private enum AnimType
	{
		Position,
		Colour
	}

	private AnimType type;					// whether the animation is affecting the position or the colour
	private Vector3 startPos;				// the starting position of the item
	private Vector3 endPos;					// the end position to animate to
	private Color startCol;					// the start colour of the item
	private Color endCol;					// the end colour to animate to
	private float time = 1.0f;				// the length of the animation in seconds
	private float delay = 0.0f;				// a delay in seconds before the animation should start
	private float percentage = 0.0f;		// the percentage through the animation we currently are

	private float prevFrameTime = 0.0f;		// we calculate our own delta time between frames because the menus need
											// to animate whilst the game is paused (i.e. when Time.timeScale = 0)
	/// <summary>
	/// Animate the position of an object, starting from its current position
	/// </summary>
	/// <param name="target">The object to animate</param>
	/// <param name="pos">The desired ending position for the object</param>
	/// <param name="time">The time in seconds to reach the end position</param>
	/// <param name="delay">The delay in seconds before the animation should start</param>
	public static void MoveTo(GameObject target, Vector3 pos, float time, float delay)
	{
		// create new position animation and attach it to the given object
		MenuAnim anim = target.AddComponent<MenuAnim>();
		anim.type = AnimType.Position;
		anim.startPos = target.transform.position;
		anim.endPos = pos;
		anim.time = time;
		anim.delay = delay;
	}

	/// <summary>
	/// Animate the color of an object, starting from its current color
	/// </summary>
	/// <param name="target">The object to animate</param>
	/// <param name="col">The desired final color for the object</param>
	/// <param name="time">he time in seconds to reach the final color</param>
	/// <param name="delay">The delay in seconds before the animation should start</param>
	public static void FadeTo(GameObject target, Color col, float time, float delay)
	{
		// create new colour animation and attach it to the given object
		MenuAnim anim = target.AddComponent<MenuAnim>();
		anim.type = AnimType.Colour;
		anim.startCol = target.GetComponent<Renderer>().material.color;
		anim.endCol = col;
		anim.time = time;
		anim.delay = delay;
	}

	void Start()
	{
		// initialise our frame timer
		prevFrameTime = Time.realtimeSinceStartup;
	}

	void Update () 
	{
		// calculate our own delta time, since Time.deltaTime is 0 when the game is paused.
		float deltaTime = Time.realtimeSinceStartup - prevFrameTime;
		prevFrameTime = Time.realtimeSinceStartup;

		// check if we've reached the end of the delay time yet
		delay -= deltaTime;
		if(delay > 0.0f)
			return;

		bool complete = false;
		percentage += deltaTime / time;
		if(percentage > 1.0f)
		{
			percentage = 1.0f;
			complete = true;
		}

		// Simple ease-in for the animations. This could be extended to allow for other ease-in/out shapes
		float t = percentage*percentage*percentage;

		// apply the animation
		switch(type)
		{
		case AnimType.Position:
			transform.position = Vector3.Lerp(startPos, endPos, t);
			break;
		case AnimType.Colour:
			GetComponent<Renderer>().material.color = Color.Lerp(startCol, endCol, t);
			break;
		default:
			break;
		}

		// once the animation has completed, this component can be removed.
		if(complete)
			Object.Destroy(this);
	}
}
