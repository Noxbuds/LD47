using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeetingTask : TaskBase
{
	// Timer for the speaker
	private float speakerTimer;
	private int speaker;

	// Range of times for speakers
	public float minSpeakDelay;
	public float maxSpeakDelay;

	// Range for number of meeting questions
	public int minMeetingQuestions;
	public int maxMeetingQuestions;

	// Overlays for each speaker
	public GameObject[] speakerOverlays;
	public GameObject playerSpeakingOverlay;

	// Time to interrupt meeting for
	private float interruptTimer;
	
	private List<int> question; // The keys the player must press to answer the question

	// Next key text
	public GameObject nextKeySprite;
	public Text nextKeyText;

	public override void Begin()
	{
		base.Begin();

		question = new List<int>();

		// Set a meeting time
		MaxProgress = Random.Range(minMeetingQuestions, maxMeetingQuestions);
		progress = 0;
		
		SelectNewSpeaker();
	}

	/// <summary>
	/// Generates a question
	/// </summary>
	/// <returns></returns>
	private List<int> GenerateQuestion(int difficulty)
	{
		List<int> newQuestion = new List<int>();

		// Generate a random sequence
		for (int i = 0; i < difficulty; i++)
		{
			newQuestion.Add(Random.Range(0, numberOfKeys));
		}

		return newQuestion;
	}

	private void HandleTimers()
	{
		interruptTimer -= Time.deltaTime;

		// Don't advance timers while there is a question
		if (question.Count == 0 && interruptTimer < 0)
		{
			speakerTimer -= Time.deltaTime;

			// If the time is up or the player is 'speaking', stop the AI speaker
			if (speakerTimer < 0 || question.Count > 0)
				speakerOverlays[speaker].SetActive(false);
			else
				speakerOverlays[speaker].SetActive(true);

			// If the speaking time is up, ask a new question
			if (speakerTimer < 0)
			{
				question = GenerateQuestion(3);

				speakerTimer = Random.Range(minSpeakDelay, maxSpeakDelay);
			}
		}
	}

	private void SelectNewSpeaker()
	{
		// Hide current speaker
		speakerOverlays[speaker].SetActive(false);

		// Pick a new speaker for next question
		speaker++;
		speaker = speaker % speakerOverlays.Length;
		speakerTimer = Random.Range(minSpeakDelay, maxSpeakDelay);
	}

	private void HandleQuestion()
	{
		// If we have reached the last step of the question, add to progress
		if (question.Count == 0)
		{
			nextKeySprite.SetActive(false);
		}
		else
		{
			// Show the next key to press
			nextKeySprite.SetActive(true);
			nextKeyText.text = keys[question[0]].ToString();

			// Check key press
			for (int i = 0; i < keys.Count; i++)
			{
				if (Input.GetKeyDown(keys[i]))
				{
					if (i == question[0])
					{
						performanceRating += 1;

						// Remove the key if it was correct
						question.RemoveAt(0);

						// If there are no more questions, increase progress
						if (question.Count == 0)
						{
							progress++;
							SelectNewSpeaker();
						}
					}
					else
					{
						// Re-generate and make it longer as if to correct yourself
						question = GenerateQuestion(question.Count + 1);
					}
				}
			}
		}
	}

	public override void InterruptTask()
	{
		base.InterruptTask();

		// Interrupt the question and add 2 more keys
		if (question != null)
			question = GenerateQuestion(question.Count + 3);
		else
			question = GenerateQuestion(3);
		interruptTimer = 2.0f;
	}

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (inProgress && !paused)
		{
			HandleTimers();

			HandleQuestion();

			playerSpeakingOverlay.SetActive(interruptTimer > 0 || question.Count > 0);
		}
    }

	public override string CatInterruptMessage()
	{
		return "Cat interrupted your meeting";
	}

	public override string ToString()
	{
		return "Work Meeting";
	}
}
