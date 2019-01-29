using UnityEngine;
using System.Collections.Generic;

// This script handles all of the level generation and layout things.
// The level is made up from multiple different scenery layers, which can each have their own parallax speed and set
// of blocks to generate the scenery from.
// These SceneryLayers should be children of the LevelManager object.

public class LevelManager : MonoBehaviour 
{
	// speed the world scrolls at. Be warned that changing this value will affect how far the character appears to
	// jump, and therefore may require alterations to the level layouts to ensure that everything remains playable (for 
	// example to make sure that all the jumps remain possible etc).
	private float worldScrollSpeed = 8.0f;		

	private float totalDistance = 0.0f;		// total distance that has scrolled (i.e. that the player has run)
	private SceneryLayer[] scenery;			// array of all SceneryLayers
	
	public float TotalDistance
	{
		get { return totalDistance; }
	}

	void Start()
	{
		// find all the SceneryLayer objects (they should be children of this object)
		scenery = GetComponentsInChildren<SceneryLayer>();
	}

	public void Restart()
	{
		// reset everything to starting values
		totalDistance = 0.0f;
		foreach(SceneryLayer layer in scenery)
			layer.Restart();

		// once each layer has been reset, we can then safely disable any layers that shouldn't be generating at the start.
		foreach(SceneryLayer layer in scenery)
		{
			if(!layer.isInfinite)
				layer.DisableNextLayer();
		}
	}

	void Update()
	{
		// update the total distance the player has covered so far
		float speed = (GameManager.Game.IsPaused || GameManager.Game.IsGameOver) ? 0.0f : worldScrollSpeed;
		totalDistance += speed * Time.deltaTime;

		// Scroll all the blocks by the distance we've travelled this frame.
		foreach(SceneryLayer layer in scenery)
		{
			layer.UpdateBlocks(speed);
		}
	}
}
