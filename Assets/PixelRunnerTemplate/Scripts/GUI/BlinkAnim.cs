using UnityEngine;
using System.Collections;

// Simple class for blinking the visibility of an item on and off

[RequireComponent(typeof(Renderer))]
public class BlinkAnim : MonoBehaviour 
{
	public float blinkTime = 0.6f;

	void Update()
	{
		// blink the item on and off using the material alpha.
		// use realtimeSinceStartup because Time.time doesn't increase when the game is paused.
		bool showTapToStart = Mathf.Repeat(Time.realtimeSinceStartup, 3*blinkTime) > blinkTime;
		Color col = GetComponent<Renderer>().material.color;
		col.a = showTapToStart ? 1.0f : 0.0f;
		GetComponent<Renderer>().material.color = col;
	}
}
