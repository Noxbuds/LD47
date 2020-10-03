using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskBase : MonoBehaviour
{
	// A task within the game
	// This should be overridden to create new tasks

	// The progress of the current task
	public int progress;

	// The maximum progress for this task
	public int MaxProgress = 10;

	/// <summary>
	/// Begins the task
	/// </summary>
	public virtual void Begin()
	{

	}

	/// <summary>
	/// Used to reset a task once it is complete or has been cancelled
	/// </summary>
	public void End()
	{
		// Check if successful
		if (progress == MaxProgress)
		{
			FindObjectOfType<TaskManager>().CompleteTask(this);
		}
		else
		{

		}
	}

	/// <summary>
	/// Interrupts the task, removing 1 progress
	/// </summary>
	public void InterruptTask()
	{
		progress -= 1;
	}

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
