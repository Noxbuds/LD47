using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages all the tasks the player has to complete
/// </summary>
public class TaskManager : MonoBehaviour
{
	// All the tasks that can be chosen
	public TaskBase[] possibleTasks;

	// Tasks that were chosen for today
	private List<TaskBase> tasksChosen;

	// Number of tasks to choose
	public int numberOfTasks;

	private TaskBase currentTask;

	// Task list display
	public Text taskListDisplay;

	/// <summary>
	/// Marks the current task as completed
	/// </summary>
	public void CompleteTask()
	{
		tasksChosen.RemoveAt(0);
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
	/// Checks whether a task is available to do
	/// </summary>
	/// <param name="task">The task to check</param>
	/// <returns>Whether it has been completed</returns>
	public bool IsTaskAvailable(TaskBase task)
	{
		return currentTask.GetType() == task.GetType();

		/*int id = tasksChosen.IndexOf(task);

		if (id >= 0)
		{
			// First check if any other tasks at the specific station need to be completed first
			for (int i = 0; i < id; i++)
			{
				if (tasksChosen[i].transform.parent == tasksChosen[id].transform.parent && tasksCompleted[i] < taskCount[i])
					return false;
			}

			return tasksCompleted[id] < taskCount[id];
		}
		else
			// Seems weird but this will disable any tasks that weren't chosen
			return false;*/
	}

	private void UpdateTaskList()
	{

		// Set up a dictionary for each count
		Dictionary<TaskBase, int> taskCounts = new Dictionary<TaskBase, int>();

		foreach (TaskBase task in tasksChosen)
		{
			// Add this count
			if (taskCounts.ContainsKey(task))
				taskCounts[task] += 1;
			else
				taskCounts.Add(task, 1);
		}

		// Loop through dictionary and add to the written list
		taskListDisplay.text = "Work to do:";
		foreach (KeyValuePair<TaskBase, int> task in taskCounts)
		{
			taskListDisplay.text += "\n" + task.Key.ToString();

			if (task.Value > 1)
				taskListDisplay.text += " x" + task.Value.ToString();
		}
	}

	// Start is called before the first frame update
	void Start()
    {
		// Pick the tasks for the day
		tasksChosen = new List<TaskBase>();

		for (int i = 0; i < numberOfTasks; i++)
		{
			// Pick next task in list for now
			int taskId = Random.Range(0, possibleTasks.Length);
			TaskBase nextTask = possibleTasks[taskId];

			tasksChosen.Add(nextTask);
		}
    }

    // Update is called once per frame
    void Update()
    {
		if (tasksChosen.Count > 0)
			currentTask = tasksChosen[0];

		UpdateTaskList();
    }
}
