using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages all the tasks the player has to complete
/// </summary>
public class TaskManager : MonoBehaviour
{
	[System.Serializable]
	public struct Level
	{
		public int numberOfTasks;
		public float timeInDay;

		public TaskBase[] tasksEnabled;
	}

	[Header("UI")]
	// Task list display
	public Text taskListDisplay;

	public Text timeDisplay;

	public GameObject endDayButton;

	[Header("Level")]
	public Level[] levels;
	public int currentLevel;
	
	// Number of tasks to choose
	public int numberOfTasks;

	// Amount of time left in the day
	public float timeLeft;

	// All the tasks that can be chosen
	public TaskBase[] possibleTasks;

	// Tasks that were chosen for today
	private List<TaskBase> tasksChosen;

	private TaskBase currentTask;

	private CatController _cat;

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
		if (currentTask != null)
			return currentTask.GetType() == task.GetType();
		else
			return false;
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

	private void SetupLevel()
	{
		// Fetch the current level
		Level level = levels[currentLevel];

		// Set possible tasks and count
		numberOfTasks = level.numberOfTasks;
		possibleTasks = level.tasksEnabled;

		// Set time remaining to the time in this level
		timeLeft = level.timeInDay;
	}

	// Ends the day when the time is up
	public void EndDay()
	{
		// If the player successfully completed the level (ie there are no more tasks),
		// set level to level + 1. Otherwise reset to zero
		if (tasksChosen.Count > 0)
		{
			PlayerData.Level = 0;
			PlayerData.LevelSuccess = false;
		}
		else
		{
			PlayerData.Level += 1;
			PlayerData.LevelSuccess = true;

			// If we have completed all levels, set the flag in 'PlayerData'
			if (PlayerData.Level >= levels.Length)
				PlayerData.GameFinished = true;
		}

		// Switch to the menu screen
		SceneManager.LoadScene("Level Overview");
	}

	// Start is called before the first frame update
	void Start()
    {
		// Fetch the cat
		_cat = FindObjectOfType<CatController>();

		// Fetch current level
		currentLevel = PlayerData.Level;

		// Set up the level
		SetupLevel();

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
		else
			currentTask = null;

		// If there are no remaining tasks, and the cat is not hungry, allow the player to end the day
		if (currentTask == null && !_cat.IsHungry())
			endDayButton.SetActive(true);
		else
			endDayButton.SetActive(false);

		UpdateTaskList();

		// Update the 'time left' display
		timeLeft -= Time.deltaTime;
		timeDisplay.text = ((int)timeLeft).ToString() + " seconds";

		// If time runs out, stop
		if (timeLeft < 0)
			EndDay();
    }
}
