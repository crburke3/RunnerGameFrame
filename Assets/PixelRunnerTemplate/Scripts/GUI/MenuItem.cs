using UnityEngine;
using System.Collections;

// A simple menu item. This simply specifies the handler function to call when the item is clicked/tapped

[RequireComponent(typeof(Collider2D))]
public class MenuItem : MonoBehaviour 
{
	public string handler;		// the name of the handler function to be called when this item is tapped
	public string sfxName;		// the name of a sound clip to play when this item is tapped
}
