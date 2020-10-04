using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CodeReviewTask : TaskBase
{
	// Valid code snippets that can be inserted
	// 0 = opening line, 1 = curly bracket, 2+ = body
	public GameObject[] codeSnippets;

	// The code to be reviewed
	private List<int[]> codeToReview;
	private List<bool[]> codeValid;
	private bool[] codeApproved; // The pieces of code that were approved
	private int currentCode; // The ID of the code currently being reviewed

	// Amount of code to review
	public int numCodeToReview;
	public int minCodeLength;
	public int maxCodeLength;

	// The root for code snippets
	public Transform codeRoot;
	private GameObject currentPage;
	public Vector2 codeStartPos;
	public Vector2 codeStepPos;

	// Colors for code
	public Color correctColor;
	public Color incorrectColor;

	public Text codeAcceptedText;

	/// <summary>
	/// Places some code on the screen in the UI
	/// </summary>
	/// <param name="code">The list of code snippets to place</param>
	/// <param name="correct">Whether code snippets in code[] are correct</param>
	private void PlaceCode(int[] code, bool[] correct)
	{
		GameObject newCode = new GameObject();
		newCode.transform.SetParent(codeRoot);

		newCode.transform.localScale = Vector3.one;
		newCode.transform.localPosition = Vector3.zero;

		for (int cursor = 0; cursor < code.Length; cursor++)
		{
			// Instantiate this snippet
			GameObject newSnippet = Instantiate(codeSnippets[code[cursor]]);

			// Fetch 'incorrect color' if this code piece is an error
			Color color = correct[cursor] ? correctColor : incorrectColor;
			newSnippet.GetComponent<Image>().color = color;

			// Parent it
			newSnippet.transform.SetParent(newCode.transform);

			// Reset scale and position correctly
			newSnippet.transform.localScale = Vector3.one;
			newSnippet.transform.localPosition = new Vector2(codeStartPos.x, codeStartPos.y + codeStepPos.y * cursor);
		}

		if (currentPage != null)
			Destroy(currentPage);
		currentPage = newCode;
	}

	/// <summary>
	/// Counts the number of correct lines in a piece of code; if there are any incorrect, return false
	/// </summary>
	/// <param name="linesCorrect">A list of lines' correctness</param>
	/// <returns>Whether the code is correct</returns>
	private bool IsCodeCorrect(bool[] linesCorrect)
	{
		for (int i = 0; i < linesCorrect.Length; i++)
			if (!linesCorrect[i])
				return false;

		return true;
	}

	public override void Begin()
	{
		base.Begin();

		// Pick number of pieces of code
		progress = 0;
		MaxProgress = numCodeToReview;

		// Generate all the code we will show
		codeToReview = new List<int[]>();
		codeValid = new List<bool[]>();
		codeApproved = new bool[MaxProgress];
		currentCode = 0;

		for (int i = 0; i < MaxProgress; i++)
		{
			// Choose whether this should be correct
			bool correct = Random.Range(0, 10) > 5;

			// Pick code length
			int codeLength = Random.Range(minCodeLength, maxCodeLength);
		
			// Generate the code
			int[] code = correct ? ProgrammingTask.GetSnippetOrder(codeLength, codeSnippets.Length) : ProgrammingTask.GetRandomSnippets(codeLength, codeSnippets.Length);
			codeToReview.Add(code);

			// Decide which pieces of code are correct
			bool[] codeCorrect = new bool[codeLength];
			for (int j = 0; j < code.Length; j++)
			{
				// If it is supposed to be correct, just set it to true
				if (correct)
				{
					codeCorrect[j] = true;
				}
				else
				{

					// For first 2 and last, they should be a fixed thing
					if (j == 0 && code[j] != 0)
						codeCorrect[j] = false;
					else if ((j == 1 || j == code.Length - 1) && code[j] != 1)
						codeCorrect[j] = false;
					else
						codeCorrect[j] = Random.Range(0, 10) > 5;
				}
			}

			codeValid.Add(codeCorrect);
			codeApproved[i] = false;
		}

		// Place the first piece of code
		PlaceCode(codeToReview[0], codeValid[0]);
	}

	/// <summary>
	/// Called from UI to approve the current piece of code
	/// </summary>
	public void ApproveCode()
	{
		codeApproved[currentCode] = true;
		
		if (IsCodeCorrect(codeValid[currentCode]))
			MovePage(1);
	}

	/// <summary>
	/// Called from UI to reject the current code
	/// </summary>
	public void RejectCode()
	{
		codeApproved[currentCode] = false;

		if (!IsCodeCorrect(codeValid[currentCode]))
			MovePage(1);
	}

	/// <summary>
	/// Checks that all the approvals are correct
	/// </summary>
	private void CheckApprovals()
	{
		progress = 0;

		for (int i = 0; i < codeApproved.Length; i++)
		{
			if (codeApproved[i] && IsCodeCorrect(codeValid[i]) || !codeApproved[i] && !IsCodeCorrect(codeValid[i]))
			{
				progress++;
			}
		}
	}

	/// <summary>
	/// Changes the current piece of code being viewed
	/// </summary>
	/// <param name="change">1 to move to next, -1 to move to previous</param>
	public void MovePage(int direction)
	{
		currentCode += direction;

		if (currentCode < 0)
			currentCode = 0;
		if (currentCode >= codeToReview.Count)
			currentCode = codeToReview.Count - 1;

		if (currentCode == codeToReview.Count - 1)
			CheckApprovals();

		// Place new code
		PlaceCode(codeToReview[currentCode], codeValid[currentCode]);
	}

	public override void InterruptTask()
	{
		base.InterruptTask();

		if (codeValid != null)
		{
			// Close the menu
			taskObject.taskActivated = false;
			SetPaused(true);
		}
	}

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

	public override string CatInterruptMessage()
	{
		return "Cat closed the window";
	}

	public override string ToString()
	{
		return "Review Code";
	}
}
