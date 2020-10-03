using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls an object in the game world which can be interacted with to begin a task
/// </summary>
[RequireComponent(typeof(TaskBase))]
public class TaskObject : MonoBehaviour
{
	// The UI to open
	public GameObject taskUI;

	// The actual tasks to do
	public TaskBase task;

	// Whether it is highlighted by the player
	public bool highlighted;

	// Whether the this task is activated
	public bool taskActivated;

	// The sprite overlay that highlights the object
	public GameObject highlightOverlay;

	// The key to open the task
	public KeyCode openTaskKey;

	// Reference to the player
	PlayerController _player;

	// Reference to the task manager
	TaskManager _taskManager;

    // Start is called before the first frame update
    void Start()
    {
		_player = FindObjectOfType<PlayerController>();

		// Fetch the task
		task = GetComponent<TaskBase>();

		// Fetch the task manager
		_taskManager = FindObjectOfType<TaskManager>();
    }

    // Update is called once per frame
    void Update()
    {
		// Check if the task should be completed
		if (task.progress == task.MaxProgress && taskActivated)
		{
			int performanceChange = task.End();

			_taskManager.CompleteTask(task);

			// Update performance rating
			FindObjectOfType<PerformanceTracker>().performanceRating += performanceChange;
		}

		// Whether this task is available to do (need to check if current task is valid first)
		bool taskAvailable = _taskManager.IsTaskAvailable(task);

		// Enable/disable highlight overlay
		if (taskAvailable)
		{
			// Start/stop task on pressing the key while highlighted
			// Do not start the task if it has been completed already
			if (Input.GetKeyDown(openTaskKey) && highlighted && taskAvailable)
			{
				// Swap state of 'task activated'
				taskActivated = !taskActivated;

				// If we've just activated the task, call its begin method
				if (taskActivated && !task.inProgress)
					task.Begin();
			}
		}
		else
		{
			// Make sure these are set to false to prevent any weird behaviour
			taskActivated = false;
			highlighted = false;
		}

		// We want the player to be unable to move when doing a task
		_player.canMove = !taskActivated;

		highlightOverlay.SetActive(highlighted);

		// Show/hide the task
		taskUI.SetActive(taskActivated);
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (collision.tag == "Player")
		{
			highlighted = true;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.tag == "Player")
		{
			highlighted = false;
		}
	}
}
