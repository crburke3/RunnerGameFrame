using UnityEngine;
using System.Collections.Generic;

// The SceneryLayer class handles placing SceneryBlocks to fill the screen, and scrolling them left with a given
// parallax speed. All the available SceneryBlocks should be children of the SceneryLayer object.

public class SceneryLayer : MonoBehaviour 
{
	public float parallaxSpeed = 1.0f;			// relative speed that this layer scrolls at
	public bool isInfinite = true;				// whether to continue generating blocks from this layer forever
	public int maxBlockCount = 0;				// maxmimum number of blocks that will be generated before moving to next layer
	public SceneryLayer nextLayer = null;		// layer to activate after this layer has generated its maximum count
	
	private float ScreenLeftEdge;				// the position of the screen's left edge in world space (depends on the device aspect ratio)
	private float ScreenRightEdge;				// the position of the screen's right edge in world space

	private List<SceneryBlock> blocksPool;		// list of scenery blocks we can use for this layer
	private List<SceneryBlock> visibleBlocks;	// list of scenery blocks that are currently in use

	private GameObject inactiveBlocks;			// game object that acts as the parent for all the block in blocksPool
	private GameObject activeBlocks;			// game object that acts as the parent for all the block in visibleBlocks

	private bool blockCreationEnabled = true;	// whether this layer is currently supposed to be generating blocks
	private int numBlocksCreated = 0;			// the number of blocks this layer has created in total

	void Start () 
	{
		// calculate the position of the screen edges, so we know the size the scenery should be covering
		ScreenRightEdge = Camera.main.aspect * Camera.main.orthographicSize;
		ScreenLeftEdge = -ScreenRightEdge;

		if(!isInfinite)
		{
			// don't generate blocks in the next layer until this layer has finished.
			DisableNextLayer();
		}

		// setup the parent game objects for our block pools
		activeBlocks = new GameObject("ActiveBlocks");
		inactiveBlocks = new GameObject("InactiveBlocks");
		activeBlocks.transform.parent = transform;
		inactiveBlocks.transform.parent = transform;

		// inactive blocks should be invisible, so set the parent to be inactive.
		inactiveBlocks.SetActive(false);

		// initialise our block pool
		InitBlockPool();
	}

	public void Restart()
	{
		// reset our block count
		numBlocksCreated = 0;
		blockCreationEnabled = true;

		// remove all the currently visible blocks, and put them back into the block pool.
		// we do this in two steps because we can't modify the visibleBlocks list whilst iterating over it.
		List<SceneryBlock> allActive = new List<SceneryBlock>();
		foreach(SceneryBlock block in visibleBlocks)
			allActive.Add(block);
		foreach(SceneryBlock block in allActive)
			RemoveVisibleBlock(block);
	}

	public void DisableNextLayer()
	{
		if(!isInfinite)
		{
			// if we're only generating a finite number of blocks, make sure there is a valid next layer to activate once
			// this layer has finished generating.
			if(nextLayer == null)
			{
				Debug.LogWarning("Layer " + this.name + " is finite but has no nextLayer set. Player could reach the edge of the world!");
			}
			else
			{
				// don't generate blocks in the next layer until this layer has finished.
				nextLayer.DisableBlockCreation();
			}
		}
	}
	
	private void InitBlockPool()
	{
		blocksPool = new List<SceneryBlock>();
		visibleBlocks = new List<SceneryBlock>();

		// find all the SceneryBlock children of this node
		SceneryBlock[] blocks = GetComponentsInChildren<SceneryBlock>();

		foreach(SceneryBlock block in blocks)
		{
			// add it to the inactive pool
			blocksPool.Add(block);
			// make it invisible
			block.transform.parent = inactiveBlocks.transform;
		}
	}

	private void EnableBlockCreation(float startX)
	{
		// this layer will now start generating blocks, starting at the given x-position
		blockCreationEnabled = true;
		numBlocksCreated = 0;
		// generate initial set of blocks, from the start point to the edge of the screen
		GenerateNewBlocks(startX);
	}
	
	private void DisableBlockCreation()
	{
		// stop creating new blocks
		blockCreationEnabled = false;
	}

	private void StartNextLayer(float startX)
	{
		// this layer is now finished with generating blocks
		DisableBlockCreation();
		// kick off the next layer, starting at the position we got up to
		if(nextLayer != null)
			nextLayer.EnableBlockCreation(startX);
	}

	public void UpdateBlocks(float scrollSpeed)
	{
		float dT = Time.deltaTime;

		// start at the left side of the screen, and generate blocks until we've reached the right edge
		float blockX = ScreenLeftEdge;
		List<SceneryBlock> offscreen = new List<SceneryBlock>();

		// scroll all the visible blocks left
		foreach(SceneryBlock block in visibleBlocks)
		{
			Vector3 position = block.transform.position;
			position.x -= scrollSpeed * parallaxSpeed * dT;
			block.transform.position = position;
			
			float blockRightEdge = position.x + block.width/2;
			blockX = Mathf.Max(blockX, blockRightEdge);

			// check if the block is fully off the left side of the screen, and mark it for removal
			bool fullyOffScreen = blockRightEdge < ScreenLeftEdge;
			if(fullyOffScreen)
			{
				offscreen.Add(block);
			}
		}
		
		// remove any blocks that are now offscreen (done in a separate loop because we can't modify the visibleBlocks
		// list whilst iterating over it).
		foreach(SceneryBlock block in offscreen)
		{
			RemoveVisibleBlock(block);
		}

		// generate any new blocks that are required to reach the edge of the screen.
		if(blockX < ScreenRightEdge)
			GenerateNewBlocks(blockX);
	}

	private void GenerateNewBlocks(float blockX)
	{
		// add in new blocks to make sure the screen is full
		while(blockCreationEnabled && blockX < ScreenRightEdge)
		{
			if(isInfinite || numBlocksCreated < maxBlockCount)
			{
				SceneryBlock block = AddNewBlock(new Vector2(blockX, 0.0f));
				if(block == null)
					break;
				blockX += block.width;
			}
			else
			{
				// we've generated all the blocks for this layer, so now start on the next layer, starting
				// at the x-position we've reached.
				StartNextLayer(blockX);
			}
		} 
	}
	
	SceneryBlock AddNewBlock(Vector2 position)
	{
		// check if we have any unused blocks in the pool.
		if(blocksPool.Count == 0)
		{
			Debug.LogWarning("Not enough blocks in layer " + this.name);
			return null;
		}
		
		// pick random new block from the pool
		int idx = Random.Range(0, blocksPool.Count);
		SceneryBlock block = blocksPool[idx];
		
		// set its initial position
		position.x += block.width/2;
		block.transform.position = position;
		
		// remove from block pool so we can't pick it again
		blocksPool.Remove(block);
		// add to visible pool
		visibleBlocks.Add(block);
		// make it visible
		block.transform.parent = activeBlocks.transform;

		numBlocksCreated++;

		return block;
	}
	
	void RemoveVisibleBlock(SceneryBlock block)
	{
		// make the block invisible again
		block.transform.parent = inactiveBlocks.transform;
		// remove block from visible list
		visibleBlocks.Remove(block);
		// add back into block pool
		blocksPool.Add(block);
	}
}
