using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages all the tasks the player has to complete
/// </summary>
public class TaskManager : MonoBehaviour
{
	// All the tasks that can be chosen
	public TaskBase[] possibleTasks;

	// Tasks that were chosen for today
	private List<TaskBase> tasksChosen;
	private List<bool> tasksCompleted;

	// Number of tasks to choose
	public int numberOfTasks;

	private TaskBase currentTask;

	/// <summary>
	/// Sets the task currently in progress
	/// </summary>
	/// <param name="task">The task to do</param>
	public void SetCurrentTask(TaskBase task)
	{
		currentTask = task;
	}

	/// <summary>
	/// Marks a task as completed
	/// </summary>
	/// <param name="task">The task to complete</param>
	public void CompleteTask(TaskBase task)
	{
		// Find its ID in the tasks chosen list and set the flag in 'tasks completed'
		int id = tasksChosen.IndexOf(task);

		if (id >= 0)
		{
			tasksCompleted[id] = true;
			SetCurrentTask(null);
		}
	}

	/// <summary>
	/// Interrupts the player's current task
	/// </summary>
	public void InterruptCurrentTask()
	{
		if (currentTask != null)
			currentTask.InterruptTask();
	}

	/// <summary>
	/// Checks whether a specific task has been completed yet
	/// </summary>
	/// <param name="task">The task to check</param>
	/// <returns>Whether it has been completed</returns>
	public bool IsTaskAvailable(TaskBase task)
	{
		int id = tasksChosen.IndexOf(task);

		if (id >= 0)
		{
			// First check if any other tasks at the specific station need to be completed first
			for (int i = 0; i < id; i++)
			{
				if (tasksChosen[i].transform.parent == tasksChosen[id].transform.parent && !tasksCompleted[i])
					return false;
			}

			return !tasksCompleted[id];
		}
		else
			// Seems weird but this will disable any tasks that weren't chosen
			return false;
	}

	// Start is called before the first frame update
	void Start()
    {
		// Pick the tasks for the day
		tasksChosen = new List<TaskBase>();
		tasksCompleted = new List<bool>();

		for (int i = 0; i < numberOfTasks; i++)
		{
			// Pick next task in list for now
			//TODO: implement possibility for random tasks with varying counts
			TaskBase nextTask = possibleTasks[i];

			tasksChosen.Add(nextTask);
			tasksCompleted.Add(false);
		}
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
