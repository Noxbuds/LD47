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

	// Timer for the player to answer a question
	private float questionAnswerTimer;
	public float timeToAnswerQuestion;
	
	private int[] question; // The keys the player must press to answer the question
	private int questionStep; // The current step in the question

	// Next key text
	public GameObject nextKeySprite;
	public Text nextKeyText;

	public override void Begin()
	{
		base.Begin();

		questionStep = 1;

		// Reset speaker timers
		speakerTimer = Random.Range(minSpeakDelay, maxSpeakDelay);

		// Set a meeting time
		MaxProgress = Random.Range(minMeetingQuestions, maxMeetingQuestions);
		progress = 0;

		// The next person to ask a question
		speaker = 0;
	}

	/// <summary>
	/// Generates a question
	/// </summary>
	/// <returns></returns>
	private int[] GenerateQuestion(int difficulty)
	{
		int[] newQuestion = new int[difficulty];

		// Generate a random sequence
		for (int i = 0; i < difficulty; i++)
		{
			newQuestion[i] = Random.Range(0, numberOfKeys);
		}

		return newQuestion;
	}

	private void HandleTimers()
	{
		questionAnswerTimer -= Time.deltaTime;

		speakerTimer -= Time.deltaTime;

		// If the time is up or the player is 'speaking', stop the AI speaker
		if (speakerTimer < 0 || questionAnswerTimer > 0)
			speakerOverlays[speaker].SetActive(false);
		else
			speakerOverlays[speaker].SetActive(true);

		// If the speaking time is up, ask a new question
		if (speakerTimer < 0 && questionStep > 0)
		{
			questionAnswerTimer = timeToAnswerQuestion;
			question = GenerateQuestion(3);
			questionStep = 0;

			speakerTimer = questionAnswerTimer + Random.Range(minSpeakDelay, maxSpeakDelay);
		}
	}

	private void HandleQuestion()
	{
		// If we have reached the last step of the question, add to progress
		if (questionStep >= question.Length)
		{
			progress += 1;
			nextKeySprite.SetActive(false);
			questionAnswerTimer = -1;

			// Pick a new speaker for next question
			speaker++;
			speakerTimer = Random.Range(minSpeakDelay, maxSpeakDelay);
		}
		else
		{
			// Show the next key to press
			nextKeySprite.SetActive(true);
			nextKeyText.text = keys[question[questionStep]].ToString();

			// Check key press
			for (int i = 0; i < keys.Count; i++)
			{
				if (Input.GetKeyDown(keys[i]))
				{
					if (i == question[questionStep])
						performanceRating += 1;

					questionStep++;
				}
			}
		}
	}

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (inProgress)
		{
			HandleTimers();

			// 

			if (questionAnswerTimer > 0)
				HandleQuestion();
		}
    }
}
