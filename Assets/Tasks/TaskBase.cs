using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskBase : MonoBehaviour
{
	// A task within the game
	// This should be overridden to create new tasks

	// The progress of the current task
	public int progress;

	// The maximum progress for this task
	public int MaxProgress = 10;

	// Whether it is in progress or not
	public bool inProgress;

	// Performance rating for the task (positive = increase performance, negative = decrease, 0 = no change)
	public int performanceRating;

	// The task object this is attached to
	public TaskObject taskObject;

	// Keys to press and their respective sprites
	[Header("Keys")]
	public List<KeyCode> keys; // the keys chosen
	public Sprite keySprite;

	public AudioSource[] keySounds;

	public int numberOfKeys; // How many keys to choose

	private static KeyCode[] allKeys = { KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, /*KeyCode.E,*/ KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J, KeyCode.K, KeyCode.L,
		KeyCode.M, KeyCode.N, KeyCode.O, KeyCode.P, KeyCode.Q, KeyCode.R, KeyCode.S, KeyCode.T, KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X, KeyCode.Y, KeyCode.Z };


	[Header("UI")]
	public Animator interruptMessage;
	public Text interruptText;

	public bool paused;
	/// <summary>
	/// Pauses/unpauses this task
	/// </summary>
	/// <param name="paused">Whether the task is paused</param>
	public void SetPaused(bool paused)
	{
		this.paused = paused;
	}

	/// <summary>
	/// Begins the task
	/// </summary>
	public virtual void Begin()
	{
		inProgress = true;
		performanceRating = 0;

		// Set the current task
		//FindObjectOfType<TaskManager>().SetCurrentTask(this);

		// Choose the keys
		keys = new List<KeyCode>();
		for (int i = 0; i < numberOfKeys; i++)
		{
			// Pick a unique key
			KeyCode chosenKey;
			do
			{
				chosenKey = allKeys[Random.Range(0, allKeys.Length)];
			} while (keys.Contains(chosenKey));

			keys.Add(chosenKey);
		}
	}

	/// <summary>
	/// Used to reset a task once it is complete or has been cancelled
	/// </summary>
	/// <returns>The performance rating change from this task</returns>
	public virtual int End()
	{
		inProgress = false;

		return performanceRating;
	}

	/// <summary>
	/// Interrupts the task
	/// </summary>
	public virtual void InterruptTask()
	{
		interruptMessage.SetTrigger("Show");
		interruptText.text = CatInterruptMessage();
	}

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	// Tasks are equal if their sub type is the same
	public override bool Equals(object other)
	{
		return other.GetType() == this.GetType();
	}

	public virtual string CatInterruptMessage()
	{
		return "Cat Interrupted";
	}

	public override string ToString()
	{
		return "Empty Task";
	}
}
