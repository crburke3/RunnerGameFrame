using UnityEngine;
using System.Collections;

// This class handles the player character
// It is responsible for the physics and animation of the character and detecting when the player
// has failed.

public class PlayerControl : MonoBehaviour 
{
	// These two parameters control how the character will jump. 
	// Decreasing the gravity will make the player fall more slowly and vice versa.
	// Increasing the jumpForce will make the character jump higher
	// Note that altering either of these will affect how far the character jumps, and so may require changes to the
	// level layouts to make sure that all of the elements remain playable.
	public float gravity = 50.0f;							// force of gravity
	public float jumpForce = 600.0f;						// upward force applied when the character jumps

	public bool isInvincible = false;						// make the player invincible to spikes, useful for debugging.
	public LayerMask groundMask;							// layer mask that identifies the ground elements
	public Transform groundPos;								// position to use for checking intersection with the ground
	public Transform deathParticles;						// particle prefab to use when the player dies

	private bool isGrounded = false;						// whether the player is on the ground
	private bool isDead = false;							// whether the player has died
	private float groundCheckRadius = 0.2f;					// size of circle around the feet to check for ground intersections
	private Collider2D[] tmpCollider = new Collider2D[1];	// collider to use when checking collisions (so we don't allocate on the fly)
	private RaycastHit2D[] tmpHit = new RaycastHit2D[1];	// raycast hit result (so we don't allocate on the fly)
	private Vector3 startPos;								// starting position of the character (the player will be locked to this x-position)
	private Animator anim;									// the animator component

	private float playerSize = 0.5f;						// size of a box around the player to use when detecting if they are off-screen

	void Start () 
	{
		isGrounded = false;
		startPos = transform.position;
		anim = GetComponent<Animator>();

		// start the game invisible
		gameObject.SetActive(false);
	}

	public void Restart()
	{
		// reset to initial values
		transform.position = startPos;
		isGrounded = false;
		isDead = false;

		// make the player visible
		gameObject.SetActive(true);
	}

	bool IsPlayerOffscreen()
	{
		// calculate the position of the screen edges
		float screenBottomEdge = -Camera.main.orthographicSize;
		float ScreenLeftEdge = -Camera.main.aspect * Camera.main.orthographicSize;

		// check if the player is completely outside the screen area
		return (transform.position.x + playerSize < ScreenLeftEdge ||
		        transform.position.y + playerSize < screenBottomEdge);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(isDead)
			return;

		// check if the player has hit some spikes
		if(other.CompareTag("Spikes") && !isInvincible)
		{
			KillPlayer();
		}
	}

	void KillPlayer()
	{
		// intantiate explosion particles
		Transform particles = (Transform)Instantiate(deathParticles, transform.position, transform.rotation);
		// destroy them again after 2 seconds (i.e. once the particle affect has finished)
		Destroy(particles.gameObject, 2.0f);
		
		// kill the player
		OnPlayerDeath();
	}

	public void HandleTap()
	{
		// handle touch/mouse input
		DoJump ();
	}

	void DoJump()
	{
		if(isGrounded)
		{
			// clear out any current y velocity
			Vector2 vel = GetComponent<Rigidbody2D>().velocity;
			vel.y = 0.0f;
			GetComponent<Rigidbody2D>().velocity = vel;
			// add in jump force
			GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpForce));
		}
	}

	void OnPlayerDeath()
	{
		if(!isDead)
		{
			isDead = true;
			// hide the player sprite
			gameObject.SetActive(false);
			// play our death sound effect
			SoundManager.PlaySfx("Death");
			// notify the game manager to trigger the finish screen
			GameManager.Game.OnPlayerDeath();
		}
	}

	void Update ()
	{
		if(GameManager.Game.IsPaused || isDead)
			return;

		// handle keyboard/gamepad input in the Update() function, because this guaranteed to run every frame
		if(Input.GetButtonDown("Jump"))
			DoJump();

		// kill the player if they ever fall off the screen
		if(IsPlayerOffscreen())
		{
			OnPlayerDeath();
		}
	}
	
	void FixedUpdate () 
	{
		if(GameManager.Game.IsPaused || isDead)
			return;

		// handle physics stuff in the FixedUpdate() function because this is run with a fixed time-step

		// check if a small circle around the player's feet overlaps with the ground
		isGrounded = Physics2D.OverlapCircleNonAlloc(groundPos.position, groundCheckRadius, tmpCollider, groundMask) > 0;

		// check if we hit a wall
		bool hitWall = Physics2D.RaycastNonAlloc(transform.position + 0.3f*Vector3.up, Vector3.right, tmpHit, 0.1f, groundMask) > 0;
		if(hitWall)
		{
			KillPlayer();
			return;
		}

		// apply the force of gravity
		GetComponent<Rigidbody2D>().AddForce(-gravity * Vector2.up);
		
		// make sure the player doesn't drift in the x-axis.
		Vector3 pos = transform.position;
		pos.x = startPos.x;
		transform.position = pos;

		// update player animation state
		anim.SetFloat("ySpeed", GetComponent<Rigidbody2D>().velocity.y);
		anim.SetBool("IsGrounded", isGrounded);
	}
}
