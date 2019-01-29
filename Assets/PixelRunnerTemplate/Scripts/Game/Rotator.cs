using UnityEngine;
using System.Collections;

// Simple class to rotate an object at a constant speed

public class Rotator : MonoBehaviour 
{
	public float speed = 1.0f;			// speed in revolutions per second
	
	void Update () 
	{
		float angle = Time.time * speed * 360.0f;
		transform.localRotation = Quaternion.Euler(Vector3.forward * angle);
	}
}
