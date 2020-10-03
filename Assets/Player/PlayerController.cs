using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the player's movement
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
	// Movement speed - units/second
	public float xSpeed;
	public float ySpeed;

	// Reference to the rigidybody
	private Rigidbody2D _rigidbody;

	// Whether the player can move
	public bool canMove;

	// The animator for the player
	public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
		_rigidbody = GetComponent<Rigidbody2D>();

		canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
		// Get movement vector
		float moveX = Input.GetAxis("Horizontal") * xSpeed;
		float moveY = Input.GetAxis("Vertical") * ySpeed;

		// Update position
		//transform.Translate(new Vector2(moveX, moveY) * Time.deltaTime);

		// Set rigidbody's speed
		if (canMove)
			_rigidbody.velocity = new Vector2(moveX, moveY);
		else
			_rigidbody.velocity = Vector2.zero;

		// Update animator
		if (_rigidbody.velocity.sqrMagnitude > 0.01f)
			animator.SetBool("IsRunning", true);
		else
			animator.SetBool("IsRunning", false);

		// Flip if necessary
		if (_rigidbody.velocity.x < 0.0)
			animator.transform.localScale = new Vector3(-1, 1, 1);
		if (_rigidbody.velocity.x > 0.0)
			animator.transform.localScale = Vector3.one;
    }
}
