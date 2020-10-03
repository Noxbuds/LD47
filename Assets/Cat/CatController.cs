using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the cat's hunger and actions
/// </summary>
public class CatController : MonoBehaviour
{
	private PlayerController _player;
	private Rigidbody2D _rigidbody;

	[Header("Hunger")]
	public float hungerGainRate;

	// 0 - 100
	// 0: happy
	// 50: hungry
	// 100: cannot do any tasks
	private float hunger;

	// Threshold for interrupting from hunger
	public float hungerThresholdInterrupting;

	// Timers for interrupting
	private float interruptTimer;
	private float interruptMaxTime; // Time between interruptions. Decreases as hunger gets to 100
	public Vector2 interruptTimeRange;

	// The cat's animator
	public Animator animator;

	// Reference to the task manager
	private TaskManager _taskManager;

	[Header("AI")]
	public Vector2 moveSpeed;

	[Header("UI")]
	public GameObject hungerBar;
	public float hungerBarSize;

	// Adjusts the position and width of the hunger bar
	private void UpdateHungerBar()
	{
		float hungerPercent = hunger / 100.0f;

		float newX = (hungerPercent * hungerBarSize) / 2f - hungerBarSize / 2f;

		hungerBar.transform.localPosition = new Vector2(newX, 0);
		hungerBar.transform.localScale = new Vector2(hungerPercent, 1);
	}

	private void ManageHunger()
	{
		// Increase hunger
		hunger += hungerGainRate * Time.deltaTime;
		if (hunger > 100)
			hunger = 100;

		// Adjust interrupt max time
		interruptMaxTime = Mathf.Lerp(interruptTimeRange.y, interruptTimeRange.x, hunger / 100.0f);

		// If above the hunger threshold, handle interrupts
		if (hunger >= hungerThresholdInterrupting)
		{
			// Decrease interrupt timer
			interruptTimer -= Time.deltaTime;

			// Interrupt when time runs out
			if (interruptTimer < 0)
			{
				_taskManager.InterruptCurrentTask();
				interruptTimer = interruptMaxTime;
			}
		}
	}

	// Makes the cat move towards the player
	private void CatAI()
	{
		// Move towards player if hunger is above threshold
		if (hunger > hungerThresholdInterrupting)
		{
			Vector2 dirToPlayer = _player.transform.position - transform.position;

			// Start moving
			if (dirToPlayer.sqrMagnitude > 0.5)
			{
				dirToPlayer.Normalize();
				_rigidbody.velocity = dirToPlayer * moveSpeed;
			}
			else
				_rigidbody.velocity = Vector2.zero;

			// If moving, update animator
			bool moving = _rigidbody.velocity.magnitude > 0.1f;
			animator.SetBool("IsWalking", moving);
		}
	}

    // Start is called before the first frame update
    void Start()
    {
		_taskManager = FindObjectOfType<TaskManager>();
		_player = FindObjectOfType<PlayerController>();
		_rigidbody = GetComponent<Rigidbody2D>();

	}

    // Update is called once per frame
    void Update()
    {
		ManageHunger();

		UpdateHungerBar();

		CatAI();

		// Flip sprite if necessary
		if (_rigidbody.velocity.x < 0.0)
			transform.localScale = new Vector2(-1, 1);
		if (_rigidbody.velocity.x > 0.0)
			transform.localScale = Vector2.one;

		// Update sprite ordering
		if (_player.transform.position.y > transform.position.y)
			GetComponentInChildren<SpriteRenderer>().sortingOrder = 2;
		else
			GetComponentInChildren<SpriteRenderer>().sortingOrder = 1;
	}
}
