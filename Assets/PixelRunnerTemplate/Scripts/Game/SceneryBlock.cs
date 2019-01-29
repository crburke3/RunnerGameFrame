using UnityEngine;
using System.Collections;

// The SceneryBlock class represents a single piece of scenery (which may be made up of many individual tiles or elements).
// All the indiviual elements should be children of this object.
// It is important that the elements all fit within the width of this object (shown with a black wire outline). If
// elements extend outside that area (to the left or right), then popping will occur when the block is first visible.

public class SceneryBlock : MonoBehaviour 
{
	public float width = 16.0f;			// The width of this block. All the child elements which make up the block should fit within this width.

	void OnDrawGizmos()
	{
		// Draw a bounding rectangle to show the extents of this block.
		Vector3 size;
		size.x = width;
		size.y = 2.0f * Camera.main.orthographicSize;
		size.z = 1.0f;

		Gizmos.color = Color.black;
		Gizmos.DrawWireCube(transform.position, size);
	}
}
