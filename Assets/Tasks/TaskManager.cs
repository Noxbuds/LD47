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

	/// <summary>
	/// Marks a task as completed
	/// </summary>
	/// <param name="task">The task to complete</param>
	public void CompleteTask(TaskBase task)
	{
		// Find its ID in the tasks chosen list and set the flag in 'tasks completed'
		int id = tasksChosen.IndexOf(task);

		if (id >= 0)
			tasksCompleted[id] = true;
	}

	/// <summary>
	/// Checks whether a specific task has been completed yet
	/// </summary>
	/// <param name="task">The task to check</param>
	/// <returns>Whether it has been completed</returns>
	public bool IsTaskCompleted(TaskBase task)
	{
		int id = tasksChosen.IndexOf(task);

		if (id >= 0)
			return tasksCompleted[id];
		else
			// Seems weird but this will disable any tasks that weren't chosen
			return true;
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
