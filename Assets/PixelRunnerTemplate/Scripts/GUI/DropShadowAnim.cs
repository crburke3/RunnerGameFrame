using UnityEngine;
using System.Collections;

// Simple animation for the drop shadow items, so they swing back and forth.

public class DropShadowAnim : MonoBehaviour 
{
	public float radius	= 0.5f;		// distance of the drop shadow away from the parent item
	public float maxAngle = 45.0f;		// maximum angle the shadow will swing to
	public float period = 2.0f;			// time for one complete swing in seconds
	
	void Update () 
	{
		// update the angle of the drop shadow
		// use realtimeSinceStartup because Time.time doesn't increase when the game is paused.
		float phase = Time.realtimeSinceStartup * (2.0f * Mathf.PI) / period;
		float angle = Mathf.Sin(phase) * (Mathf.Deg2Rad * maxAngle);

		// calculate the new position of the shadow
		float startX = 0.0f;
		float startY = -radius;
		float sinAngle = Mathf.Sin(angle);
		float cosAngle = Mathf.Cos(angle);
		float x = cosAngle * startX - sinAngle * startY;
		float y = sinAngle * startX + cosAngle * startY;

		// set the shadow position relative to our parent
		transform.localPosition = new Vector3(x,y,0.0f);
	}
}
